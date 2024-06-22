using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingObjectSelfDestruct : MonoBehaviour
{
    public bool isTimedSelfDestruct = false;
    public float lifetime = 1f;
    private float lifetimebucket = 0f;

    public bool isCollisionSelfDestruct = false;
    public bool isTimedAfterCollisionSelfDestruct = false;

    private bool selfDestructActivated = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if( isTimedSelfDestruct && !selfDestructActivated && !GetComponent<BuildingObject>().IsInPreviewMode)
        {
            lifetimebucket += Time.deltaTime;

            float newLifetime = AbilityUpgradesManager.Instance.AbilityUpgrades[Abilities.TRAP_DURATION].GetModifiedValue(lifetime);

            if( lifetimebucket >= newLifetime)
            {
                ActivateSelfDestruct();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if( (isTimedAfterCollisionSelfDestruct || isCollisionSelfDestruct) && !selfDestructActivated && !GetComponent<BuildingObject>().IsInPreviewMode)
        {
            if(other.gameObject.layer == LayerMask.NameToLayer("EnemyUnitLayer"))
            {

                if( isTimedAfterCollisionSelfDestruct )
                {
                    isTimedSelfDestruct = true;
                }else
                {
                    ActivateSelfDestruct();
                }
            }
        }
    }

    private void ActivateSelfDestruct()
    {
        selfDestructActivated = true;

        Destroy(gameObject);
    }
}
