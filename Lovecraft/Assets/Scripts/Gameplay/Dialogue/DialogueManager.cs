using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    public TextMeshProUGUI speakerNameTextRef;
    public TextMeshProUGUI dialogueTextRef;
    public Image portraitImageRef;
    public GameObject dialoguePanelRef;
    public Button nextLineButtonRef;
    public List<Dialogue> StoryPoints = new List<Dialogue>();
    public int CurrentStoryPointIndex = 0;

    private Queue<DialogueLine> dialogueLines;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            dialogueLines = new Queue<DialogueLine>();
            dialoguePanelRef.SetActive(false);

            nextLineButtonRef.onClick.AddListener(DisplayNextLine);

            //Register all of the listeners...
            GameEventSystem.Instance.RegisterListener(GameEvent.TEST_DIALOGUE, OnTestDialogue);
            GameEventSystem.Instance.RegisterListener(GameEvent.CREATURE_SPAWNED_DIALOGUE_BARK, OnCreatureSpawnedDialogueBark);
            GameEventSystem.Instance.RegisterListener(GameEvent.GAME_OVER, OnGameOver);
            GameEventSystem.Instance.RegisterListener(GameEvent.WAVE_COMPLETED, OnWaveComplete);
            GameEventSystem.Instance.RegisterListener(GameEvent.GAME_STARTED, OnGameStart);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        //Unregister all of the listeners...
        GameEventSystem.Instance.UnregisterListener(GameEvent.TEST_DIALOGUE, OnTestDialogue);
        GameEventSystem.Instance.UnregisterListener(GameEvent.CREATURE_SPAWNED_DIALOGUE_BARK, OnCreatureSpawnedDialogueBark);
        GameEventSystem.Instance.UnregisterListener(GameEvent.WAVE_COMPLETED, OnWaveComplete);
        GameEventSystem.Instance.UnregisterListener(GameEvent.GAME_STARTED, OnGameStart);
    }

    private void OnGameStart(object data)
    {
        CurrentStoryPointIndex = 0;
        StartDialogue(StoryPoints[CurrentStoryPointIndex].dialogueName);
    }

    private void OnWaveComplete(object data)
    {
        CurrentStoryPointIndex++;
        StartDialogue(StoryPoints[CurrentStoryPointIndex].dialogueName);
    }

    private void OnTestDialogue(object data)
    {
        StartDialogue("enemy_killed_dialogue");
    }

    private void OnCreatureSpawnedDialogueBark(object data)
    {
        if (data is EnemyAI enemyAI)
        {
            Dialogue dialogue = DialogueLoader.Instance.GetDialogue("creature_spawned");
            if (dialogue != null && dialogue.lines.Count > 0)
            {
                DialogueLine line = dialogue.lines[0];
                enemyAI.OnSpeechBubble(line.dialogueText, line.sfxType);
            }
        }
    }

    private void OnGameOver(object data)
    {
        StartDialogue("player_death_ending_story_dialogue");
    }

    public void StartDialogue(string dialogueName)
    {
        Dialogue dialogue = DialogueLoader.Instance.GetDialogue(dialogueName);

        if (dialogue != null)
        {
            dialogueLines.Clear();

            foreach (var line in dialogue.lines)
            {
                dialogueLines.Enqueue(line);
            }

            var firstLine = dialogue.lines[0];
            if (firstLine.isRandomBark)
            {
                Debug.LogError("This... shouldn't play this way");
            }
            else
            {
                dialoguePanelRef.SetActive(true);
                Time.timeScale = 0; // Pause the game
                DisplayNextLine();
            }
        }
    }

    //(can be called from the UI)
    public void DisplayNextLine()
    {
        if (dialogueLines.Count == 0)
        {
            EndDialogue();
            return;
        }

        var line = dialogueLines.Dequeue();
        speakerNameTextRef.text = line.speakerName;
        dialogueTextRef.text = line.dialogueText;
        portraitImageRef.sprite = line.portrait;

        if (line.storyAudioClip != null)
        {
            AudioManager.Instance.PlayAudioClip(line.storyAudioClip);
        }
    }

    private void EndDialogue()
    {
        dialoguePanelRef.SetActive(false);
        Time.timeScale = 1; // Resume the game
        GameEventSystem.Instance.TriggerEvent(GameEvent.DIALOGUE_COMPLETE);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            GameEventSystem.Instance.TriggerEvent(GameEvent.TEST_DIALOGUE);
        }

        //TEMP code for progressing dialogue.. eventually make this UI driven?
        if (Input.GetKeyDown(KeyCode.Space) && dialoguePanelRef.activeSelf)
        {
            DisplayNextLine();
        }
    }
}