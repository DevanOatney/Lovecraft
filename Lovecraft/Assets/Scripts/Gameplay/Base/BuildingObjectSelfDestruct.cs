using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingObjectSelfDestruct : MonoBehaviour
{
    public bool isTimedSelfDestruct = false;
    public float lifetime = 1f;
    private float lifetimebucket = 0f;

    public bool isCollisionSelfDestruct = false;


    private bool selfDestructActivated = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if( isTimedSelfDestruct && !selfDestructActivated)
        {
            lifetimebucket += Time.deltaTime;
            if( lifetimebucket >= lifetime)
            {
                ActivateSelfDestruct();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if( isCollisionSelfDestruct && !selfDestructActivated)
        {

        }
    }

    private void ActivateSelfDestruct()
    {
        selfDestructActivated = true;

        foreach(MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
        {
            if(mr.GetComponent<Rigidbody>() == null)
            {
                mr.gameObject.AddComponent<Rigidbody>();
            }
            mr.GetComponent<Rigidbody>().AddExplosionForce(50, transform.position, 5);
        }
    }
}
