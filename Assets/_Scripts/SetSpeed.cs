using UnityEngine;
using UnityEngine.AI;

using Pada1.BBCore;           // Code attributes
using Pada1.BBCore.Tasks;     // TaskStatus
using Pada1.BBCore.Framework; // BasePrimitiveAction

[Action("MyActions/SetSpeed")]
public class SetSpeed : BasePrimitiveAction
{
    [InParam("speed", DefaultValue = 2f)]
    public float speed;

    [InParam("animator")]
    public Animator animator;

    [InParam("navMeshAgent")]
    public NavMeshAgent navMeshAgent;

    public override TaskStatus OnUpdate()
    {
        navMeshAgent.speed = speed;
        animator.SetFloat("Speed", speed);
        animator.SetFloat("MotionSpeed", 1);
        return TaskStatus.COMPLETED;
    }

    public void OnFootstep()
    {
        // Add your logic here for what should happen on a footstep.
    }
}
