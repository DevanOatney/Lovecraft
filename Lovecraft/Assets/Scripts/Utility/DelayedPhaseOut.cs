using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DelayedPhaseOut : MonoBehaviour
{

    [SerializeField] private float phaseOutTime = 1f;
    [SerializeField] private Image imageToAlphaOut;
    private float phaseOutBucket = 0;

    private void OnEnable()
    {
        phaseOutBucket = 0;
    }

    // Update is called once per frame
    void Update()
    {
        phaseOutBucket += Time.deltaTime;
        if( phaseOutBucket >= phaseOutTime )
        {
            gameObject.SetActive(false);
        }

        if( imageToAlphaOut != null)
        {
            Color tempColor = imageToAlphaOut.color;
            tempColor.a = 1 - phaseOutBucket / phaseOutTime;
            imageToAlphaOut.color = tempColor;
        }
    }
}
