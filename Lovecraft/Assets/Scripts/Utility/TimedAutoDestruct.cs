using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedAutoDestruct : MonoBehaviour
{

    [SerializeField] private float destructionTime;
    private float timerBucket;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timerBucket += Time.deltaTime;
        if( timerBucket >= destructionTime)
        {
            Destroy(gameObject);
        }
    }
}
