using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameSate
{
    BUILD_PHASE,
    COMBAT_PHASE,
    GAME_STATE_COUNT,
}

public class RoundController : MonoBehaviour
{
    #region Singleton
    private static RoundController _instance;
    public static RoundController Instance
    {
        get
        {
            if (_instance == null)
            {
                // Find existing instance
                _instance = FindObjectOfType<RoundController>();

                // If no instance was found, create one
                if (_instance == null)
                {
                    GameObject singleton = new GameObject("RoundController");
                    _instance = singleton.AddComponent<RoundController>();
                }
            }
            return _instance;
        }
    }
    #endregion


    [SerializeField] private bool allowTimedPhases = false;

    [SerializeField] private RectTransform phaseClockHand;
    [SerializeField] private float timeForEachRound = 45f;
    [SerializeField] private GameObject combatPhaseIndicator;
    [SerializeField] private GameObject buildingPhaseIndicator;
    [SerializeField] private Canvas buildingPhaseCanvas;
    [SerializeField] private Canvas combatPhaseCanvas;
    [SerializeField] private GameObject nextPhaseButton;

    private float roundTimerBucket = 0f;
    private GameSate gameState = GameSate.BUILD_PHASE; //'true' for Day/combat; 'false' for Night/build

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
            if (gameState == GameSate.COMBAT_PHASE) //daytime 
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
        gameState = GameSate.COMBAT_PHASE;
        TriggerPhaseChange();
    }

    public void TriggerCombatPhase()
    {
        // set the state to the 'wrong' state so the 'TriggerPhaseChange' method will set it to the correct state with it's logic
        gameState = GameSate.BUILD_PHASE;
        TriggerPhaseChange();
    }

    private void OnPhaseChange(object obj)
    {

        if (gameState == GameSate.BUILD_PHASE)
        {
            gameState = GameSate.COMBAT_PHASE;

            combatPhaseIndicator.SetActive(true);
            combatPhaseCanvas.gameObject.SetActive(true);

            buildingPhaseIndicator.SetActive(false);
            buildingPhaseCanvas.gameObject.SetActive(false);
        }
        else if( gameState == GameSate.COMBAT_PHASE)
        {
            gameState = GameSate.BUILD_PHASE;

            combatPhaseIndicator.SetActive(false);
            combatPhaseCanvas.gameObject.SetActive(false);

            buildingPhaseIndicator.SetActive(true);
            buildingPhaseCanvas.gameObject.SetActive(true);
        }
    }
}
