using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RoundController : MonoBehaviour
{
    [SerializeField] private bool allowTimedPhases = false;

    [SerializeField] private RectTransform phaseClockHand;
    [SerializeField] private float timeForEachRound = 45f;
    [SerializeField] private GameObject combatPhaseIndicator;
    [SerializeField] private GameObject buildingPhaseIndicator;
    [SerializeField] private Canvas buildingPhaseCanvas;
    [SerializeField] private Canvas combatPhaseCanvas;
    [SerializeField] private GameObject nextPhaseButton;

    private float roundTimerBucket = 0f;
    private bool cycleState = true; //'true' for Day/combat; 'false' for Night/build

    // Start is called before the first frame update
    void Start()
    {
        GameEventSystem.Instance.RegisterListener(GameEvent.PHASE_CHANGE, OnPhaseChange);
    }

    private void OnDestroy()
    {
        GameEventSystem.Instance.UnregisterListener(GameEvent.PHASE_CHANGE, OnPhaseChange);
    }

    // Update is called once per frame
    void Update()
    {
        if (allowTimedPhases)
        {
            if( !phaseClockHand.parent.gameObject.activeSelf )
            {
                phaseClockHand.parent.gameObject.SetActive(true);
            }
            if( nextPhaseButton.activeSelf)
            {
                nextPhaseButton.SetActive(false);
            }

            roundTimerBucket += Time.deltaTime;

            if (roundTimerBucket >= timeForEachRound)
            {
                roundTimerBucket = 0f;
                TriggerPhaseChange();
            }

            //clockface rotation;
            // 0 -> -180 == daytime
            // -180 -> -360 == nighttime

            float percent = roundTimerBucket / timeForEachRound;
            float rotationAmount = -180 * percent;
            if (!cycleState) //daytime 
            {
                rotationAmount -= 180f;
            }
            phaseClockHand.rotation = Quaternion.Euler(0, 0, rotationAmount);

        } else
        {
            if (phaseClockHand.parent.gameObject.activeSelf)
            {
                phaseClockHand.parent.gameObject.SetActive(false);
            }   
            if( !nextPhaseButton.activeSelf )
            {
                nextPhaseButton.SetActive(true);
            }
        }
    }

    public void TriggerPhaseChange()
    {
        GameEventSystem.Instance.TriggerEvent(GameEvent.PHASE_CHANGE, null);
    }

    public void TriggerBuildPhase()
    {
        // set the state to the 'wrong' state so the 'TriggerPhaseChange' method will set it to the correct state with it's logic
        cycleState = true;
        TriggerPhaseChange();
    }

    public void TriggerCombatPhase()
    {
        // set the state to the 'wrong' state so the 'TriggerPhaseChange' method will set it to the correct state with it's logic
        cycleState = false;
        TriggerPhaseChange();
    }

    private void OnPhaseChange(object obj)
    {
        cycleState = !cycleState;

        if (cycleState)
        {
            combatPhaseIndicator.SetActive(true);
            combatPhaseCanvas.gameObject.SetActive(true);

            buildingPhaseIndicator.SetActive(false);
            buildingPhaseCanvas.gameObject.SetActive(false);
        }
        else
        {
            combatPhaseIndicator.SetActive(false);
            combatPhaseCanvas.gameObject.SetActive(false);

            buildingPhaseIndicator.SetActive(true);
            buildingPhaseCanvas.gameObject.SetActive(true);
        }
    }
}
