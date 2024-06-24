using UnityEngine;

public class BuildingObject : MonoBehaviour
{
    public enum BuildingType { ProjectileShooter, Trap }
    public BuildingType buildingType = BuildingType.ProjectileShooter;

    public Quaternion DirectionToFire = Quaternion.identity;
    public BuildingNode ParentNode = null;
    public GameObject ProjectilePrefab = null;

    public float ProjectileSpeed = 10f;
    public float FiringFrequency = 1f;
    private float FiringTimer = 0.0f;

    public float TrapCooldown = 1f; // Cooldown for the trap
    private float TrapTimer = 0.0f;

    public float MovementSpeedAdjuster = 0f;
    public bool IsInPreviewMode = false;

    private Collider buildingCollider;

    public int placementCost = 0;

    public void Awake()
    {
        buildingCollider = GetComponentInChildren<Collider>();
    }

    public void Update()
    {
        if (!IsInPreviewMode)
        {
            float modifier = AbilityUpgradesManager.Instance.AbilityUpgrades[Abilities.TRAP_COOLDOWN].GetModifiedValue(FiringFrequency);

            if (buildingType == BuildingType.ProjectileShooter)
            {
                HandleProjectileShooter();
            }
            else if (buildingType == BuildingType.Trap)
            {
                HandleTrap();
            }
        }
    }

    private void HandleProjectileShooter()
    {
        if (FiringTimer >= FiringFrequency)
        {
            FiringTimer = 0.0f;
            FireProjectile();
        }
        else
        {
            FiringTimer += Time.deltaTime;
        }
    }

    private void FireProjectile()
    {
        if (ProjectilePrefab != null)
        {
            Vector3 spawnPosition = transform.position + new Vector3(0, 0.5f, 0) + transform.forward * 1.0f; // Slightly in front of the building
            GameObject projectile = Instantiate(ProjectilePrefab, spawnPosition, DirectionToFire);
            var ProjectileController = projectile.GetComponent<ProjectileController>();
            if (ProjectileController != null)
            {
                ProjectileController.Initialize(buildingCollider);
            }
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = transform.forward * ProjectileSpeed; // Adjust speed as needed
            }
        }
    }

    private void HandleTrap()
    {
        if (TrapTimer < TrapCooldown)
        {
            TrapTimer += Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (buildingType == BuildingType.Trap && TrapTimer >= TrapCooldown && other.CompareTag("Enemy"))
        {
            LaunchEnemy(other);
            TrapTimer = 0.0f; // Reset the cooldown timer
        }

        if (MovementSpeedAdjuster != 0f && other.CompareTag("Enemy"))
        {
            // Adjust movement speed
            EnemyAI eAi = other.GetComponent<EnemyAI>();
            if (eAi != null)
            {
                eAi.AddAdjuster(this.gameObject, MovementSpeedAdjuster);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (MovementSpeedAdjuster != 0f && other.CompareTag("Enemy"))
        {
            // Return enemy movement rate to normal
            EnemyAI eAi = other.GetComponent<EnemyAI>();
            if (eAi != null)
            {
                eAi.RemoveAdjuster(this.gameObject);
            }
        }
    }

    private void LaunchEnemy(Collider enemy)
    {
        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.HandleLaunch(DirectionToFire, ProjectileSpeed);
        }
    }
}
