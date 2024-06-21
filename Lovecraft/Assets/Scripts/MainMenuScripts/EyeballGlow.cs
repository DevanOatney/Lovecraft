using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeballGlow : MonoBehaviour
{
    public Material eyeMat;
    public float timer = 2f;
    private float bucket = 0;
    public bool ascending = true;

    // Update is called once per frame
    void Update()
    {

        if (ascending == true)
        {
            bucket += Time.deltaTime;
            if (bucket > timer) { ascending = false; }
        } else
        {
            bucket -= Time.deltaTime;
            if( bucket < 0 ) { ascending = true; }
        }

        Color color = eyeMat.GetColor("_BaseColor");
        color.a = bucket / timer;
        eyeMat.SetColor("_BaseColor", color);
    }
}
