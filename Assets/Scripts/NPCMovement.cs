using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterController))]
public class NPCMovement : MonoBehaviour
{
    public float stopDistance = 2f;
    public float moveSpeed = 3.5f;
    public Transform target;

    private NavMeshAgent agent;
    private CharacterController controller;
    private NPCCombat combat;

    public bool IsMoving => agent.hasPath && agent.velocity.sqrMagnitude > 0.05f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        controller = GetComponent<CharacterController>();
        combat = GetComponent<NPCCombat>();

        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.speed = moveSpeed;

        stopDistance = combat.AttackDistance - 0.1f; // walk a bit closer than required
    }

    void Update()
    {
        if (target == null)
        {
            agent.ResetPath();
            return;
        }

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > stopDistance)
        {
            agent.SetDestination(target.position);
            MoveAlongPath();
        }
        else
        {
            StopMoving();
        }
    }

    void MoveAlongPath()
    {
        Vector3 move = agent.desiredVelocity;
        move.y = 0;

        if (move.sqrMagnitude > 0.01f)
        {
            Vector3 direction = agent.desiredVelocity;
            direction.y = 0;

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
                targetRotation *= Quaternion.Euler(0f, 180f, 0f); // spin 180° on Y
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }
        }

        controller.Move(move * Time.deltaTime);
        agent.nextPosition = transform.position;
    }

    void StopMoving()
    {
        agent.ResetPath();
        agent.velocity = Vector3.zero;
    }
}
