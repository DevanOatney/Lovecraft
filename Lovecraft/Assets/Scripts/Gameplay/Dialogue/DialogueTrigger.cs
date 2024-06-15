using System;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public static DialogueTrigger Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GameEventSystem.Instance.RegisterListener(GameEvent.ENEMY_KILLED, OnEnemyKilled);
        // Register other events as needed
    }

    private void OnDestroy()
    {
        GameEventSystem.Instance.UnregisterListener(GameEvent.ENEMY_KILLED, OnEnemyKilled);
        // Unregister other events as needed
    }

    private void OnEnemyKilled(object param)
    {
        DialogueLoader.Instance.LoadDialogue("enemy_killed_dialogue.json");
    }

    // Add more event handlers as needed
}