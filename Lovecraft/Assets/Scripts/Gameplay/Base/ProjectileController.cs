using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ProjectileController : MonoBehaviour
{
    public string FilteredTag = string.Empty;
    public int DamageToDeal = 1;

    public void Initialize(Collider parentCol)
    {
        Physics.IgnoreCollision(parentCol, this.GetComponent<Collider>());
        Invoke("OnCollided", 5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        var eAi = collision.gameObject.GetComponent<EnemyAI>();
        if (eAi != null)
        {
            if (FilteredTag != string.Empty)
            {
                if (eAi.tag != FilteredTag)
                {
                    OnCollided();
                    return;
                }
            }

            float modDamage = AbilityUpgradesManager.Instance.AbilityUpgrades[Abilities.TRAP_EFFECTIVENESS].GetModifiedValue(DamageToDeal);

            eAi.TakeDamage(modDamage);
            OnCollided();
            return;
        }

        var player = collision.gameObject.GetComponent<PlayerSixWayDirectionalController>();
        if (player != null)
        {
            if(FilteredTag != string.Empty)
            {
                if (player.tag != FilteredTag)
                {
                    OnCollided();
                    return;
                }
            }
            player.TakeDamage(DamageToDeal);
            OnCollided();
            return;
        }
        OnCollided();
    }

    private void OnCollided()
    {
        Destroy(gameObject);
    }
}
