using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MonsterLove.StateMachine;
using System;

public class Hostage : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField, Range(0f, 15f)] private float maxFollowDistance = 10f;
    
    private Transform target;
    private bool isAtHQ;

    public class Driver
    {
        public StateEvent Update;
        public StateEvent<Collider> OnTriggerEnter;
    }

    public enum States
    {
        Init,
        Idle,
        FollowTarget
    }

    StateMachine<States, Driver> fsm;

    private void Awake()
    {
        fsm = new StateMachine<States, Driver>(this);
        fsm.ChangeState(States.Init);

        animator.SetFloat("MotionSpeed", 1);
        lineRenderer.positionCount = 0;
    }

    private void Update()
    {
        fsm.Driver.Update.Invoke();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if ((collider.CompareTag("Enemy") && !isAtHQ) || collider.CompareTag("Player") || collider.CompareTag("HQ"))
        {
            fsm.ChangeState(States.FollowTarget);
            target = collider.transform;

        }

        if (collider.CompareTag("HQ"))
        {
            isAtHQ = true;
        }

        fsm.Driver.OnTriggerEnter.Invoke(collider);
    }

    private void FollowTarget_Enter()
    {
        if (target == null) return;
        if (target.CompareTag("Player")) isAtHQ = false;
    }

    private void FollowTarget_Update()
    {
        if (target == null) return;

        agent.SetDestination(target.position);

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, target.position);

        if (agent.remainingDistance < 1)
            animator.SetFloat("Speed", 0);
        else
            animator.SetFloat("Speed", agent.speed);

        if (agent.remainingDistance > maxFollowDistance)
            fsm.ChangeState(States.Idle);
    }

    private void Idle_Enter()
    {
        target = null;
        agent.ResetPath();
        animator.SetFloat("Speed", 0);
        lineRenderer.positionCount = 0;
    }

    private void OnDrawGizmos()
    {
        if (target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, target.position);
        }
    }
}
