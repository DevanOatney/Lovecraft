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
            eAi.TakeDamage(DamageToDeal);
            OnCollided();
            return;
        }

        var player = collision.gameObject.GetComponent<PlayerController>();
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
