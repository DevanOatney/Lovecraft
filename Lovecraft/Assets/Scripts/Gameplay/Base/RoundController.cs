using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundController : MonoBehaviour
{
    [SerializeField] private RectTransform phaseClockHand;
    [SerializeField] private float timeForEachRound = 45f;
    [SerializeField] private GameObject combatPhaseIndicator;
    [SerializeField] private GameObject buildingPhaseIndicator;
    [SerializeField] private Canvas buildingPhaseCanvas;
    [SerializeField] private Canvas combatPhaseCanvas;

    private float roundTimerBucket = 0f;
    private bool cycleState = true; //'true' for Day/combat; 'false' for Night/build

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        roundTimerBucket += Time.deltaTime;

        if( roundTimerBucket >= timeForEachRound)
        {
            roundTimerBucket = 0f;
            cycleState = !cycleState;

            if( cycleState )
            {
                combatPhaseIndicator.SetActive(true);
                combatPhaseCanvas.gameObject.SetActive(true);

                buildingPhaseIndicator.SetActive(false);
                buildingPhaseCanvas.gameObject.SetActive(false);
            } else
            {
                combatPhaseIndicator.SetActive(false);
                combatPhaseCanvas.gameObject.SetActive(false);

                buildingPhaseIndicator.SetActive(true);
                buildingPhaseCanvas.gameObject.SetActive(true);
            }
        }

        //clockface rotation;
        // 0 -> -180 == daytime
        // -180 -> -360 == nighttime

        float percent = roundTimerBucket / timeForEachRound;
        float rotationAmount = -180 * percent;
        if( !cycleState ) //daytime 
        {
            rotationAmount -= 180f;
        }
        phaseClockHand.rotation = Quaternion.Euler(0, 0, rotationAmount);


    }
}
