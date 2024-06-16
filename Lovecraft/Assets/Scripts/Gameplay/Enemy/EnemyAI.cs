using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
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
    public float playerChaseRange = 10f; // Range within which the enemy will chase the player
    public float playerChaseReturnRange = 15f; // Range beyond which the enemy will stop chasing the player
    public Transform waypointRoot; // Root object for waypoints
    public List<List<Transform>> waypoints = new List<List<Transform>>(); // List of lists of waypoints
    public bool IsDying = false;
    public float HP = 10;
    public float MaxHP;
    public float MovementSpeed = 5f;
    public List<AudioClip> hitSFXList;
    public AudioSource audioSource;
    public Image HealthBar;
    public GameObject bloodSplatterBase;
    public EnemyAttackData enemyAttack;

    private NavMeshAgent agent;
    private AttackHandler attackHandler;
    private bool attackingPlayer;
    private List<Transform> treePositions = new List<Transform>();
    private Transform currentTreePosition;
    private int currentWaypointListIndex = 0;
    private float stuckTime;
    private float stuckCheckInterval = 1f; // Time interval to check if the AI is stuck

    private Dictionary<GameObject, float> adjustments = new Dictionary<GameObject, float>();

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        attackHandler = GetComponent<AttackHandler>();
        PopulateWaypoints();
        foreach (Transform child in treeTarget)
        {
            treePositions.Add(child);
        }
        PickRandomTreePosition();
        PickRandomWaypoint();
        stuckTime = Time.time + stuckCheckInterval;
        HealthBar = GetComponentInChildren<Image>();
        MaxHP = HP;
    }

    void Update()
    {
        if (attackHandler.IsAttacking())
        {
            return;
        }

        float distanceToTree = currentTreePosition != null ? Vector3.Distance(transform.position, currentTreePosition.position) : 0;
        float distanceToPlayer = playerTarget != null ? Vector3.Distance(transform.position, playerTarget.position) : 0;

        float totalAdjustment = 1f;
        foreach (var adj in adjustments)
        {
            totalAdjustment += adj.Value;
        }

        if (totalAdjustment < 0.2f)
            totalAdjustment = 0.2f;

        agent.speed = MovementSpeed * totalAdjustment;

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
            attackHandler.AttackTree(treeTarget, enemyAttack.damageToDeal);
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
                attackHandler.StartAttack(playerTarget, enemyAttack);
            }
            else
            {
                agent.isStopped = false;
                MoveToTarget(playerTarget);
            }
        }
        else
        {
            if (distanceToPlayer <= playerChaseRange && distanceToTree > attackRange)
            {
                attackingPlayer = true;
                MoveToTarget(playerTarget);
            }
            else if (distanceToTree <= attackRange)
            {
                agent.isStopped = true;
                attackHandler.AttackTree(treeTarget, enemyAttack.damageToDeal);
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
            attackHandler.StartAttack(playerTarget, enemyAttack);
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

    public void OnAttackComplete()
    {
        agent.isStopped = false;
    }

    public void TakeDamage(float damageToTake)
    {

        audioSource.PlayOneShot(hitSFXList[Random.Range(0, hitSFXList.Count)]);

        if (IsDying)
            return;

        HP -= damageToTake;

        if (HealthBar != null)
        {
            HealthBar.fillAmount =  HP / MaxHP;
        }

        if (HP <= 0)
        {
            IsDying = true;
            StartDying();
        }
    }

    private void StartDying()
    {
        //Play animations?
        //Sink into the ground?
        //Spawn resources/loot?
        GameObject obj = Instantiate(bloodSplatterBase, new Vector3(transform.position.x, 0.51f, transform.position.z), Quaternion.identity);
        obj.transform.Rotate(90, Random.Range(0, 359), 0);

        FinishDying();
    }

    private void FinishDying()
    {
        GameEventSystem.Instance.TriggerEvent(GameEvent.ENEMY_KILLED);
        Destroy(this.gameObject);
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

    public void AddAdjuster(GameObject adjuster, float adjustment)
    {
        if(adjustments.ContainsKey(adjuster))
            adjustments.Remove(adjuster);
        adjustments.Add(adjuster, adjustment);
    }

    public void RemoveAdjuster(GameObject adjuster)
    {
        adjustments.Remove(adjuster);
    }
}