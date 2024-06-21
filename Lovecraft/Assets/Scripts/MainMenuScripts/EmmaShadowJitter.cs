using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmmaShadowJitter : MonoBehaviour
{
    public Vector3 originPoint;
    public Vector2 jitterMinMax;
    public float jitterTime = 1f;

    public Vector2 jitterDestination;
    float bucket = 0;

    private void Start()
    {
        originPoint = GetComponent<RectTransform>().anchoredPosition;
        bucket = jitterTime;
    }

    // Update is called once per frame
    void Update()
    {
        bucket += Time.deltaTime;
        if( bucket >= jitterTime)
        {
            bucket = 0;
            jitterDestination = new Vector2(originPoint.x, originPoint.y) + jitterMinMax - new Vector2(Random.Range(jitterMinMax.x*2, jitterMinMax.y*2), Random.Range(jitterMinMax.x*2, jitterMinMax.y*2));
        }

        Vector2 offset = new Vector2(Mathf.Lerp(GetComponent<RectTransform>().anchoredPosition.x, jitterDestination.x, bucket / jitterTime), Mathf.Lerp(GetComponent<RectTransform>().anchoredPosition.y, jitterDestination.y, bucket / jitterTime));
        GetComponent<RectTransform>().anchoredPosition = offset;
    }
}
