using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAppliedPhysics : MonoBehaviour
{
    [SerializeField] private Rigidbody RB;
    float BucketTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        BucketTimer -= Time.deltaTime;
        if( BucketTimer <= 0 )
        {
            BucketTimer = 3.5f;
            float eX = Random.Range(-3, 3) + RB.transform.position.x;
            float eY = Random.Range(-3, 3) + RB.transform.position.y;
            float eZ = Random.Range(-3, 3) + RB.transform.position.z;
            RB.AddExplosionForce(14f, new Vector3(eX, eY, eZ), 50);
        }
    }
}
