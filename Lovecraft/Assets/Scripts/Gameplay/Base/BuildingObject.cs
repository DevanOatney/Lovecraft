using UnityEngine;

public class BuildingObject : MonoBehaviour
{
    public Quaternion DirectionToFire = Quaternion.identity;
    public BuildingNode ParentNode = null;
    public GameObject ProjectilePrefab = null;

    public float ProjectileSpeed = 10f;
    public float FiringFrequency = 1f;
    private float FiringTimer = 0.0f;

    public float SlowDownRate = 0f;

    public void Update()
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
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = transform.forward * ProjectileSpeed; // Adjust speed as needed
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (SlowDownRate > 0f && other.CompareTag("Enemy"))
        {
            // Slow down enemy movement rate
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (SlowDownRate > 0f && other.CompareTag("Enemy"))
        {
            // Return enemy movement rate to normal
        }
    }
}