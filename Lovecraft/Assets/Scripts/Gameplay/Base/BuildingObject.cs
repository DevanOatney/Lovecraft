using UnityEngine;

public class BuildingObject : MonoBehaviour
{
    public Quaternion DirectionToFire = Quaternion.identity;
    public BuildingNode ParentNode = null;
    public GameObject ProjectilePrefab = null;

    public float ProjectileSpeed = 10f;
    public float FiringFrequency = 1f;
    private float FiringTimer = 0.0f;

    public float MovementSpeedAdjuster = 0f;
    public bool IsInPreviewMode = false;

    private Collider buildingCollider;

    public void Awake()
    {
        buildingCollider = GetComponentInChildren<Collider>();
    }

    public void Update()
    {
        if (!IsInPreviewMode)
        {
            float modifier = AbilityUpgradesManager.Instance.AbilityUpgrades[Abilities.TRAP_COOLDOWN].GetModifiedValue(FiringFrequency);
            Debug.Log(modifier);
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

    private void OnTriggerEnter(Collider other)
    {
        if (MovementSpeedAdjuster != 0f && other.CompareTag("Enemy"))
        {
            //Adjust movement speed
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
            if(eAi != null)
            {
                eAi.RemoveAdjuster(this.gameObject);
            }
        }
    }
}