# GameAISmartNPC
Mata Kuliah Game Artificial Intelligence Semester 5 Prodi Teknologi Permainan STMM MMTC Yogyakarta 2023

Gamenya seperti Counter Strike, player menjadi counter terorist yang menyelamatkan hostage.

## Behavior hostage (FSM):
```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MonsterLove.StateMachine;
using System;

public class Hostage : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float maxFollowDistance = 10f;
    
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
        if ((collider.CompareTag("Enemy") && !isAtHQ) || 
        	collider.CompareTag("Player") || 
            collider.CompareTag("HQ"))
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

        if (agent.remainingDistance > maxFollowDistance)
            fsm.ChangeState(States.Idle);
    }

    private void Idle_Enter()
    {
        target = null;
        agent.ResetPath();
    }
}
```

- Ketika berada di dekat markas pemain, mereka akan menuju markas pemain. 
- Ketika berada di dekat markas teroris, mereka akan menuju markas teroris. 
- Jika berada di dekat teroris, mereka akan mengikuti teroris. 
- Jika berada di dekat pemain, mereka akan mengikuti pemain. 
- Jika tidak ada pemain atau teroris di sekitar mereka, mereka akan menuju area perlindungan terdekat.


## Behavior terorist (Behavior Tree):

![](https://i.imgur.com/TGsuABs.png)

- Mereka akan menyerang pemain jika berada di dekat pemain. 
- Jika berada di dekat sandera, mereka akan menculik sandera dan membawanya kembali ke markas teroris. 
- Jika sudah mencapai markas teroris, mereka akan melakukan patroli.

## Gameplay
![](https://i.giphy.com/media/ByoGwL5kGQOyTD1a7E/source.gif)