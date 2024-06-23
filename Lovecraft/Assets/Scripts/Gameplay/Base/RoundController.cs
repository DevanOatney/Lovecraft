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

    [SerializeField] private GameSate gameState = GameSate.BUILD_PHASE;
    [SerializeField] private bool allowTimedPhases = false;
    [SerializeField] private RectTransform phaseClockHand;
    [SerializeField] private float timeForEachRound = 45f;
    [SerializeField] private GameObject combatPhaseIndicator;
    [SerializeField] private GameObject buildingPhaseIndicator;
    [SerializeField] private Canvas buildingPhaseCanvas;
    [SerializeField] private Canvas combatPhaseCanvas;
    [SerializeField] private GameObject nextPhaseButton;
    [SerializeField] private WaveManager waveManager;

    private float roundTimerBucket = 0f;
    
    private int currentWaveIndex = 0;
    private bool isSceneInitialized = false;

    void Start()
    {
        GameEventSystem.Instance.RegisterListener(GameEvent.PHASE_CHANGE, OnPhaseChange);
        GameEventSystem.Instance.RegisterListener(GameEvent.WAVE_COMPLETED, OnWaveCompleted);
        GameEventSystem.Instance.TriggerEvent(GameEvent.GAME_STARTED);
        TriggerBuildPhase();
    }

    private void OnDestroy()
    {
        GameEventSystem.Instance.UnregisterListener(GameEvent.PHASE_CHANGE, OnPhaseChange);
        GameEventSystem.Instance.UnregisterListener(GameEvent.WAVE_COMPLETED, OnWaveCompleted);
    }

    void Update()
    {
        if (allowTimedPhases)
        {
            if (!phaseClockHand.parent.gameObject.activeSelf)
            {
                phaseClockHand.parent.gameObject.SetActive(true);
            }
            if (nextPhaseButton.activeSelf)
            {
                nextPhaseButton.SetActive(false);
            }

            roundTimerBucket += Time.deltaTime;

            if (roundTimerBucket >= timeForEachRound)
            {
                roundTimerBucket = 0f;
                TriggerPhaseChange();
            }

            float percent = roundTimerBucket / timeForEachRound;
            float rotationAmount = -180 * percent;
            if (gameState == GameSate.COMBAT_PHASE)
            {
                rotationAmount -= 180f;
            }
            phaseClockHand.rotation = Quaternion.Euler(0, 0, rotationAmount);
        }
        else
        {
            if (phaseClockHand.parent.gameObject.activeSelf)
            {
                phaseClockHand.parent.gameObject.SetActive(false);
            }
            if (!nextPhaseButton.activeSelf)
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
        gameState = GameSate.COMBAT_PHASE;
        TriggerPhaseChange();
    }

    public void TriggerCombatPhase()
    {
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

            StartCombatPhase();
        }
        else if (gameState == GameSate.COMBAT_PHASE)
        {
            gameState = GameSate.BUILD_PHASE;

            combatPhaseIndicator.SetActive(false);
            combatPhaseCanvas.gameObject.SetActive(false);

            buildingPhaseIndicator.SetActive(true);
            buildingPhaseCanvas.gameObject.SetActive(true);
        }
    }

    private void StartCombatPhase()
    {
        if (currentWaveIndex < waveManager.waves.Count)
        {
            waveManager.StartWave();
        }
    }

    private void OnWaveCompleted(object obj)
    {
        currentWaveIndex++;
        if (currentWaveIndex < waveManager.waves.Count)
        {
            OnPhaseChange(null);
        }
        else
        {
            Debug.Log("All waves completed!");
        }
    }

    public void SetSceneInitialized(bool sceneInitialized)
    {
        isSceneInitialized = sceneInitialized;
    }

    public bool IsSceneInitialized()
    {
        return isSceneInitialized;
    }
}
