using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Gun))]
[RequireComponent(typeof(Animator))]
public class EnemyWithGun : LivingEntity
{
    private NavMeshAgent pathfinder;
    private Transform target;
    private Gun _gunController;
    private Animator _animator;

    public enum State
    {
        Idle, 
        Chasing,
        Retreat
    };

    private State _currentState;
    private float _attackDistanceThreshold = 1.5f;
    private float _stoppingDistance = 10f;
    private float _retreatDistance = 5f;
    private float _myCollisionRadius;
    protected override void  Start()
    {
        base.Start();
        _currentState = State.Chasing;
        pathfinder = GetComponent<NavMeshAgent>();
        target = GameObject.FindWithTag("Player").transform;
        _myCollisionRadius = GetComponent<CapsuleCollider>().radius;
        _gunController = GetComponent<Gun>();
        _animator = GetComponent<Animator>();
        _animator.SetBool("Shoot", false);
        _animator.SetBool("Moving", true);
        StartCoroutine(UpdatePath());
    }

    private void Update()
    {
        if (target == null)
        {
            return;
        }
        float sqrtDistToTarget = (target.transform.position - transform.position).sqrMagnitude;
        if (sqrtDistToTarget > Mathf.Pow(_stoppingDistance, 2))
        {
            _currentState = State.Chasing;
            pathfinder.updateRotation = true;
        }
        else if(sqrtDistToTarget < Mathf.Pow(_retreatDistance, 2))
        {
            _currentState = State.Retreat;
            pathfinder.updateRotation = false;
            LookAt(target.position);
        }
        
        else if(sqrtDistToTarget <= Mathf.Pow(_stoppingDistance, 2) && sqrtDistToTarget > Mathf.Pow(_retreatDistance, 2))
        {
            var dirToTarget = (target.position - transform.position).normalized;
            if (Physics.Raycast(transform.position, dirToTarget, sqrtDistToTarget, LayerMask.GetMask("Wall")))
            {
                Debug.DrawRay(transform.position, dirToTarget, Color.red, sqrtDistToTarget);
                return;
            }
            _currentState = State.Idle;
            pathfinder.updateRotation = false;
            LookAt(target.position);
            _gunController.Shoot(AttackPower, LayerMask.GetMask("Player"));
            _animator.SetBool("Shoot", true);
        }
    }
    IEnumerator UpdatePath()
    {
        float tefrashRate = 0.25f;
        while (target != null)
        {
            if (_currentState == State.Chasing)
            {
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - dirToTarget * (_stoppingDistance - _myCollisionRadius);
                
                    if (!dead)
                        pathfinder.SetDestination(targetPosition);
            }
            
            if (_currentState == State.Retreat)
            {
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - dirToTarget * _stoppingDistance;
                
                if (!dead)
                    pathfinder.SetDestination(targetPosition);
            }
            yield return  new WaitForSeconds(tefrashRate);
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
    
    private void LookAt(Vector3 lookPoint)
    {
        var look = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        transform.LookAt(look);
    }
}

