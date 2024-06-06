using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PhaseIndicatorPhaseOut : DelayedPhaseOut
{
    [SerializeField] TMP_Text tmpText;

    // Update is called once per frame
    void Update()
    {
        PhaseOutUpdate();
        Color tempColor = tmpText.color;
        tempColor.a = 1 - phaseOutBucket / phaseOutTime;
        tmpText.color = tempColor;
    }
}
