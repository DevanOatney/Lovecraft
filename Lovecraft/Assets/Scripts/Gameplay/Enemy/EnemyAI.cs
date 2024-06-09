using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    public enum Tactic
    {
        FocusTree,
        TreeOrPlayer,
        FocusPlayer
    }

    public Tactic enemyTactic; // The tactic this enemy will use
    public Transform treeTarget; // The target object (tree)
    public Transform playerTarget; // The player object
    public float attackRange = 5f; // Range within which the enemy can attack
    public float attackCooldown = 1f; // Time between attacks
    public int attackDamage = 10; // Damage per attack
    public float playerChaseRange = 10f; // Range within which the enemy will chase the player
    public float playerChaseReturnRange = 15f; // Range beyond which the enemy will stop chasing the player
    public Transform waypointRoot; // Root object for waypoints
    public List<List<Transform>> waypoints = new List<List<Transform>>(); // List of lists of waypoints

    private NavMeshAgent agent;
    private float lastAttackTime;
    private bool attackingPlayer;
    private List<Transform> treePositions = new List<Transform>();
    private Transform currentTreePosition;
    private int currentWaypointListIndex = 0;
    private float stuckTime;
    private float stuckCheckInterval = 1f; // Time interval to check if the AI is stuck

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        PopulateWaypoints();
        foreach (Transform child in treeTarget)
        {
            treePositions.Add(child);
        }
        PickRandomTreePosition();
        PickRandomWaypoint();
        stuckTime = Time.time + stuckCheckInterval;
    }

    void Update()
    {
        float distanceToTree = Vector3.Distance(transform.position, currentTreePosition.position);
        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

        switch (enemyTactic)
        {
            case Tactic.FocusTree:
                HandleFocusTree(distanceToTree);
                break;
            case Tactic.TreeOrPlayer:
                HandleTreeOrPlayer(distanceToTree, distanceToPlayer);
                break;
            case Tactic.FocusPlayer:
                HandleFocusPlayer(distanceToPlayer);
                break;
        }

        // Check if the AI is stuck
        if (Time.time > stuckTime)
        {
            stuckTime = Time.time + stuckCheckInterval;
            if (agent.velocity.sqrMagnitude < 0.1f) // AI is stuck
            {
                PickRandomWaypoint();
            }
        }
    }

    void HandleFocusTree(float distanceToTree)
    {
        if (distanceToTree <= attackRange)
        {
            agent.isStopped = true;
            AttackTarget(treeTarget);
        }
        else
        {
            agent.isStopped = false;
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                PickNextWaypointOrTree();
            }
        }
    }

    void HandleTreeOrPlayer(float distanceToTree, float distanceToPlayer)
    {
        if (attackingPlayer)
        {
            if (distanceToPlayer > playerChaseReturnRange)
            {
                attackingPlayer = false;
                PickRandomTreePosition();
                MoveToTarget(currentTreePosition);
            }
            else if (distanceToPlayer <= attackRange)
            {
                agent.isStopped = true;
                AttackTarget(playerTarget);
            }
            else
            {
                agent.isStopped = false;
                MoveToTarget(playerTarget);
            }
        }
        else
        {
            if (distanceToPlayer <= playerChaseRange)
            {
                attackingPlayer = true;
                MoveToTarget(playerTarget);
            }
            else if (distanceToTree <= attackRange)
            {
                agent.isStopped = true;
                AttackTarget(treeTarget);
            }
            else
            {
                agent.isStopped = false;
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    PickNextWaypointOrTree();
                }
            }
        }
    }

    void HandleFocusPlayer(float distanceToPlayer)
    {
        if (distanceToPlayer <= attackRange)
        {
            agent.isStopped = true;
            AttackTarget(playerTarget);
        }
        else
        {
            agent.isStopped = false;
            MoveToTarget(playerTarget);
        }
    }

    void MoveToTarget(Transform target)
    {
        agent.SetDestination(target.position);
    }

    void PickRandomWaypoint()
    {
        if (waypoints.Count > 0)
        {
            currentWaypointListIndex = 0;
            PickNextWaypointOrTree();
        }
    }

    void PickNextWaypointOrTree()
    {
        if (currentWaypointListIndex < waypoints.Count)
        {
            List<Transform> currentWaypoints = waypoints[currentWaypointListIndex];
            if (currentWaypoints.Count > 0)
            {
                Transform nextWaypoint = currentWaypoints[Random.Range(0, currentWaypoints.Count)];
                agent.SetDestination(nextWaypoint.position);
                currentWaypointListIndex++;
            }
        }
        else
        {
            PickRandomTreePosition();
            agent.SetDestination(currentTreePosition.position);
        }
    }

    void PickRandomTreePosition()
    {
        if (treePositions.Count > 0)
        {
            currentTreePosition = treePositions[Random.Range(0, treePositions.Count)];
        }
        else
            currentTreePosition = treeTarget;
    }

    void AttackTarget(Transform target)
    {
        if (Time.time - lastAttackTime > attackCooldown)
        {
            if (target == treeTarget)
            {
                target.GetComponent<TreeController>().TakeDamage(attackDamage);
            }
            else if (target == playerTarget)
            {
                target.GetComponent<PlayerController>().TakeDamage(attackDamage);
            }
            lastAttackTime = Time.time;
        }
    }

    void PopulateWaypoints()
    {
        if (waypointRoot != null)
        {
            foreach (Transform waypointGroup in waypointRoot)
            {
                List<Transform> waypointList = new List<Transform>();
                foreach (Transform waypoint in waypointGroup)
                {
                    waypointList.Add(waypoint);
                }
                waypoints.Add(waypointList);
            }
        }
    }
}