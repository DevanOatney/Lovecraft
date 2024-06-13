using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZoneController : MonoBehaviour
{
    [SerializeField] int damageToOpponant = 1;
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

    private void OnTriggerEnter(Collider other)
    {
        if ((InteractionWithUnitsOnLayer & (1 << other.gameObject.layer)) != 0)
        {
            Debug.Log("Damage : " + other.name);
            EnemyAI eAI = other.GetComponent<EnemyAI>();
            if( eAI != null )
            {
                eAI.TakeDamage(damageToOpponant);
            }
            // if other.contains method for taking damage 
            {
                // take damage
            }
        }
    }
}
