using System.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform targetDestination;
    float jumpHeight = 2f;
    float maxJumpDistance = 5f;
    public LayerMask navMeshLayer;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                targetDestination.position = hit.point;
                TryToMoveToTarget();
            }
        }
    }

    private void TryToMoveToTarget()
    {
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(targetDestination.position, path);

        if (path.status == NavMeshPathStatus.PathComplete)
        {
            agent.SetPath(path);
        }
        else
        {
            MoveToClosestNavMeshEdgeAndTryJump();
        }
    }

    private void MoveToClosestNavMeshEdgeAndTryJump()
    {
        Vector3 closestEdgePoint = GetClosestJumpableEdgePoint(agent.transform.position, targetDestination.position);

        if (closestEdgePoint != Vector3.zero)
        {
            agent.SetDestination(closestEdgePoint);
            StartCoroutine(WaitForAgentToReachDestination(() =>
            {
                Vector3 closestPointOnTargetNavMesh = GetClosestPointOnNavMeshEdge(targetDestination.position);
                TryJumpFromPosition(closestEdgePoint, closestPointOnTargetNavMesh);
            }));
        }
    }

    private Vector3 GetClosestJumpableEdgePoint(Vector3 startPosition, Vector3 targetPosition)
    {
        NavMeshHit navHit;
        float closestDistance = Mathf.Infinity;
        Vector3 closestPoint = Vector3.zero;

        for (float x = -maxJumpDistance; x <= maxJumpDistance; x += 1f)
        {
            for (float z = -maxJumpDistance; z <= maxJumpDistance; z += 1f)
            {
                Vector3 samplePosition = startPosition + new Vector3(x, 0, z);
                if (NavMesh.FindClosestEdge(samplePosition, out navHit, NavMesh.AllAreas))
                {
                    float distance = Vector3.Distance(navHit.position, targetPosition);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPoint = navHit.position;
                    }
                }
            }
        }

        return closestPoint;
    }

    private Vector3 GetClosestPointOnNavMeshEdge(Vector3 targetPosition)
    {
        NavMeshHit navHit;
        float closestDistance = Mathf.Infinity;
        Vector3 closestPoint = Vector3.zero;

        for (float x = -maxJumpDistance; x <= maxJumpDistance; x += 1f)
        {
            for (float z = -maxJumpDistance; z <= maxJumpDistance; z += 1f)
            {
                Vector3 samplePosition = targetPosition + new Vector3(x, 0, z);
                if (NavMesh.FindClosestEdge(samplePosition, out navHit, NavMesh.AllAreas))
                {
                    float distance = Vector3.Distance(navHit.position, targetPosition);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPoint = navHit.position;
                    }
                }
            }
        }

        return closestPoint;
    }

    private IEnumerator WaitForAgentToReachDestination(System.Action onDestinationReached)
    {
        while (agent.pathPending || !agent.isOnNavMesh || agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }

        onDestinationReached.Invoke();
    }

    private void TryJumpFromPosition(Vector3 startPosition, Vector3 targetPosition)
    {
        Vector3 shortestJumpEndPoint = Vector3.zero;
        float shortestJumpDistance = Mathf.Infinity;

        // Iterate through potential jump distances
        for (float distance = 1f; distance <= maxJumpDistance; distance += 1f)
        {
            Vector3 jumpDir = (targetPosition - startPosition).normalized;
            Vector3 jumpEndPoint = startPosition + jumpDir * distance;

            NavMeshHit navHit;
            if (NavMesh.SamplePosition(jumpEndPoint, out navHit, 1f, NavMesh.AllAreas))
            {
                float actualJumpDistance = Vector3.Distance(startPosition, navHit.position);
                if (actualJumpDistance <= distance + float.Epsilon && actualJumpDistance < shortestJumpDistance)
                {
                    shortestJumpEndPoint = navHit.position;
                    shortestJumpDistance = actualJumpDistance;
                }
            }
        }

        if (shortestJumpDistance < Mathf.Infinity)
        {
            JumpTo(shortestJumpEndPoint);
        }
        else
        {
            Debug.Log("Unable to find valid jump end point on navmesh");
        }
    }

    private void JumpTo(Vector3 targetPosition)
    {
        StartCoroutine(JumpCoroutine(targetPosition));
    }

    private IEnumerator JumpCoroutine(Vector3 targetPosition)
    {
        agent.enabled = false;

        float jumpDuration = 1f;
        float time = 0f;
        Vector3 startPos = transform.position;
        Vector3 peakPos = (startPos + targetPosition) / 2;
        peakPos.y += jumpHeight;

        while (time < jumpDuration)
        {
            if (time < jumpDuration * 0.5f)
            {
                // Ascend
                transform.position = Vector3.Lerp(startPos, peakPos, time / (jumpDuration * 0.5f));
            }
            else
            {
                // Descend
                transform.position = Vector3.Lerp(peakPos, targetPosition, (time - jumpDuration * 0.5f) / (jumpDuration * 0.5f));
            }
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        agent.enabled = true;

        agent.SetDestination(targetDestination.position);
    }
}
