using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyProjectileController : MonoBehaviour
{
    public string FilteredTag = string.Empty;
    public int DamageToDeal = 1;
    public float Speed = 10f; // Speed of the projectile

    private Vector3 direction;

    public void Initialize(Collider parentCol, Vector3 targetPosition)
    {
        Physics.IgnoreCollision(parentCol, GetComponent<Collider>());
        direction = (targetPosition - transform.position).normalized;
        Invoke("OnCollided", 3f); // Destroy projectile after 5 seconds if no collision
    }

    void Update()
    {
        transform.Translate(direction * Speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider col)
    {
        var eAi = col.gameObject.GetComponent<EnemyAI>();
        if (eAi != null)
        {
            if (FilteredTag != string.Empty)
            {
                if (eAi.tag == FilteredTag)
                {
                    OnCollided();
                    return;
                }
            }
            else
            {

                float modDamage = AbilityUpgradesManager.Instance.AbilityUpgrades[Abilities.TRAP_EFFECTIVENESS].GetModifiedValue(DamageToDeal);

                eAi.TakeDamage(modDamage);
                OnCollided();
                return;
            }
        }

        var player = col.gameObject.GetComponent<PlayerSixWayDirectionalController>();
        if (player != null)
        {
            if (FilteredTag != string.Empty)
            {
                if (player.tag == FilteredTag)
                {
                    player.TakeDamage(DamageToDeal);
                    OnCollided();
                    return;
                }
            }
            else
            {
                player.TakeDamage(DamageToDeal);
                OnCollided();
                return;
            }
        }
    }

    private void OnCollided()
    {
        Destroy(gameObject);
    }
}
