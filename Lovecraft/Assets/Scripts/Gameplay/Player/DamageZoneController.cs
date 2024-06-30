using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZoneController : MonoBehaviour
{
    [SerializeField] public float damageToOpponent = 1;
    [SerializeField] LayerMask interactionWithUnitsOnLayer;

    private bool isSelfDestructing = true;

    // Start is called before the first frame update
    void Start()
    {
        if (isSelfDestructing)
            Invoke("DestroyMe", 0.75f);
    }

    void DestroyMe()
    {
        Destroy(gameObject);
    }

    public void Initialize(int targetLayer, float dmg, bool selfDestruct = true)
    {
        interactionWithUnitsOnLayer = 1 << targetLayer;
        damageToOpponent = dmg;
        isSelfDestructing = selfDestruct;
    }

    private void OnTriggerEnter(Collider other)
    {
        int layerMaskValue = 1 << other.gameObject.layer;
        if ((interactionWithUnitsOnLayer.value & layerMaskValue) != 0)
        {
            EnemyAI eAI = other.GetComponent<EnemyAI>();
            if (eAI != null)
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
