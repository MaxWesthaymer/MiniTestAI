using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    private NavMeshAgent _pathfinder;
    private Transform _target;
    private enum State
    {
        Idle, 
        Chasing, 
        Attacking
    }
    private State _currentState;
    private float attackDistanceThreshold = 1.5f;
    private float timeBetweenAttacks = 1f;
    private float _attackCooldown;
    private float _myCollisionRadius;
    private float _targetCollisionRadius;
    private Animator _animator;
    private LivingEntity _targetEntity;
    private bool _hasTarget;
    protected override void  Start()
    {
        base.Start();
        _currentState = State.Chasing;
        _pathfinder = GetComponent<NavMeshAgent>();
        _target = GameObject.FindWithTag("Player").transform;
        _targetEntity = _target.GetComponent<LivingEntity>();
        _targetEntity.OnDeath += OnTargetDeath;
        _hasTarget = true;
        _myCollisionRadius = GetComponent<CapsuleCollider>().radius;
        _targetCollisionRadius = _target.GetComponent<CapsuleCollider>().radius;
        StartCoroutine(UpdatePath());
        _animator = GetComponent<Animator>();
        _animator.SetInteger("State", 2);
    }

    private void OnTargetDeath()
    {
        _hasTarget = false;
        _currentState = State.Idle;
    }
    void Update()
    {
        if (_attackCooldown > 0)
        {
            _attackCooldown -= Time.deltaTime;
        }
        if (!_hasTarget)
        {
            return;
        }
        
        var sqrtDistToTarget = (_target.transform.position - transform.position).sqrMagnitude;
        if (sqrtDistToTarget < Mathf.Pow(attackDistanceThreshold + _myCollisionRadius + _targetCollisionRadius, 2))
        {
            if (_attackCooldown <= 0)
            {
                _attackCooldown += timeBetweenAttacks;
                StartCoroutine(Attack());
                _targetEntity.TakeHit(1);
            }
        }
    }

    IEnumerator Attack()
    {
        _pathfinder.enabled = false;
        _currentState = State.Attacking;
        Vector3 originalPosition = transform.position;
        Vector3 dirToTarget = (_target.position - transform.position).normalized;
        Vector3 attackPosition = _target.position - dirToTarget * (_myCollisionRadius);

        float attackSpeed = 3;
        float percent = 0;

        while (percent <= 1)
        {
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent,2) * percent + percent)*4;
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);
            yield return null;
        }

        _pathfinder.enabled = true;
        _currentState = State.Chasing;
    }
    IEnumerator UpdatePath()
    {
        float tefrashRate = 0.25f;
        while (_hasTarget)
        {
            if (_currentState == State.Chasing)
            {
                Vector3 dirToTarget = (_target.position - transform.position).normalized;
                Vector3 targetPosition = _target.position - dirToTarget * (_myCollisionRadius + _targetCollisionRadius + attackDistanceThreshold/2);
                if (!dead)
                    _pathfinder.SetDestination(targetPosition);
            }

            yield return  new WaitForSeconds(tefrashRate);
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}

