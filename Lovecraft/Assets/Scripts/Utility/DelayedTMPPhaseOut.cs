using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DelayedTMPPhaseOut : MonoBehaviour
{

    [SerializeField] protected float phaseOutTime = 1f;
    [SerializeField] private TMP_Text tmpToAlphaOut;
    protected float phaseOutBucket = 0;

    private void OnEnable()
    {
        phaseOutBucket = 0;
    }

    // Update is called once per frame
    void Update()
    {
        PhaseOutUpdate();
    }

    public void PhaseOutUpdate()
    {
        phaseOutBucket += Time.deltaTime;
        if (phaseOutBucket >= phaseOutTime)
        {
            gameObject.SetActive(false);
        }

        if (tmpToAlphaOut != null)
        {
            Color tempColor = tmpToAlphaOut.color;
            tempColor.a = 1 - phaseOutBucket / phaseOutTime;
            tmpToAlphaOut.color = tempColor;
        }
    }
}
