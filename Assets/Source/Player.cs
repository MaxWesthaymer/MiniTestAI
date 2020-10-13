using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Gun))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class Player : LivingEntity
{
    [SerializeField] private float viewRadius;
    private NavMeshAgent _pathfinder;
    private Transform _target;
    private Transform _globalTarget;
    private Gun _gunController;
    private Animator _animator;
    private enum State
    {
        Idle,
        Move,
        Retreat
    };

    private State _currentState;
    private float _attackDistanceThreshold = 1.5f;
    private float _retreatDistance = 8f;
    private float _timeBetweenAttacks = 0.2f;
    private float _nextAttackTime;

    protected override void Start()
    {
        SetupEntity(GameController.instance.entitySettings.playerConfig);
        base.Start();
        _gunController = GetComponent<Gun>();
        _animator = GetComponent<Animator>();
        _animator.SetBool("Shoot", false);
        _animator.SetBool("Moving", true);
        OnDeath += () =>
        {
            GameController.instance.EndOfGame(false);
        };
        
        _currentState = State.Move;
        _pathfinder = GetComponent<NavMeshAgent>();
        _globalTarget = GameObject.Find("FinishTrigger").transform;
        StartCoroutine(UpdatePath());
        StartCoroutine(FindTarget(0.2f));
        GameController.OnEndGame += PlayerStop;
    }
    
    private void PlayerStop(bool isWin)
    {
        _animator.SetBool("Moving", false);
        _globalTarget = null;
    }
    
    private void Update()
    {
        if (_target == null)
        {
            _currentState = State.Move;
            _pathfinder.updateRotation = true;
            _animator.SetBool("Shoot", false);
            
        }
        else
        {
            _currentState = State.Retreat;
            _pathfinder.updateRotation = false;
            LookAt(_target.position);
            _gunController.Shoot(GetAttackDamage(), LayerMask.GetMask("Enemy"));
            _animator.SetBool("Shoot", true);
        }
    }

    private float GetAttackDamage()
    {
        return AttackPower * Random.value;
    }

    private void LookAt(Vector3 lookPoint)
    {
        var look = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        transform.LookAt(look);
    }
    
    private IEnumerator UpdatePath()
    {
        float tefrashRate = 0.25f;
        while (_globalTarget != null)
        {
            if (_target == null)
            {
                _currentState = State.Move;
            }
            if (_currentState == State.Move)
            {
                Vector3 dirToTarget = (_globalTarget.position - transform.position).normalized;
                Vector3 targetPosition = _globalTarget.position - dirToTarget * (0.5f);
                
                if (!dead)
                    _pathfinder.SetDestination(targetPosition);
            }
            
            if (_currentState == State.Retreat)
            {
                Vector3 dirToTarget = (_target.position - transform.position).normalized;
                Vector3 targetPosition = _target.position - dirToTarget * _retreatDistance;
                
                if (!dead)
                    _pathfinder.SetDestination(targetPosition);
            }

            yield return  new WaitForSeconds(tefrashRate);
        }
    }

    private IEnumerator FindTarget(float delay)
    {
        while (true)
        {
            yield return  new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    private void FindVisibleTargets()
    {
        var closestEnemies = Physics.OverlapSphere(transform.position, viewRadius,
        LayerMask.GetMask("Enemy"), QueryTriggerInteraction.Collide);
        List<Collider> visibleTargets = new List<Collider>();

        foreach (var collider in closestEnemies)
        {
            var target = collider.transform;
            var dirToTarget = (target.position - transform.position).normalized;
            var distToTarget = Vector3.Distance(transform.position, target.position);

            if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, LayerMask.GetMask("Wall")))
            {
                visibleTargets.Add(collider);
            }
        }
        
        if (visibleTargets.Count > 0)
        {
            _target = visibleTargets[Random.Range(0, visibleTargets.Count)].transform;
        }
        else
        {
            _target = null;
        }
    }

    private void OnDestroy()
    {
        GameController.OnEndGame -= PlayerStop;
    }
}

