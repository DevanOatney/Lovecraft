using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundController : MonoBehaviour
{
    [SerializeField] private RectTransform dayNightClockHand;
    [SerializeField] private float timeForEachRound = 45f;
    [SerializeField] private GameObject DayIndicator;
    [SerializeField] private GameObject NightIndicator;

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
                DayIndicator.SetActive(true);
                NightIndicator.SetActive(false);
            } else
            {
                DayIndicator.SetActive(false);
                NightIndicator.SetActive(true);
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
        dayNightClockHand.rotation = Quaternion.Euler(0, 0, rotationAmount);


    }
}
