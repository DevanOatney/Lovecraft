using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZoneController : MonoBehaviour
{
    [SerializeField] public float damageToOpponent = 1;
    [SerializeField] LayerMask InteractionWithUnitsOnLayer;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroyMe", 0.75f);
    }

    void DestroyMe()
    {
        Destroy(gameObject);
    }

    public void Initialize(LayerMask targetMask, float dmg)
    {
        InteractionWithUnitsOnLayer = targetMask; 
        damageToOpponent = dmg;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((InteractionWithUnitsOnLayer & (1 << other.gameObject.layer)) != 0)
        {
            EnemyAI eAI = other.GetComponent<EnemyAI>();
            if( eAI != null )
            {
                eAI.TakeDamage(damageToOpponent);
                return;
            }
            PlayerSixWayDirectionalController pC = other.GetComponent<PlayerSixWayDirectionalController>();
            if (pC != null)
            {
                pC.TakeDamage((int)damageToOpponent);
                return;
            }
        }
    }
}
