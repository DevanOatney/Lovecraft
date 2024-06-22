using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    public Text speakerNameText;
    public Text dialogueText;
    public Image portraitImage;
    public GameObject dialoguePanel;

    private Queue<DialogueLine> dialogueLines;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            dialogueLines = new Queue<DialogueLine>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        dialoguePanel.SetActive(true);
        dialogueLines.Clear();

        foreach (var line in dialogue.lines)
        {
            dialogueLines.Enqueue(line);
        }

        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (dialogueLines.Count == 0)
        {
            EndDialogue();
            return;
        }

        var line = dialogueLines.Dequeue();
        speakerNameText.text = line.speakerName;
        dialogueText.text = line.dialogueText;
        portraitImage.sprite = line.portrait;
        if (line.voiceClip == SFXType.RandomBark)
        {
            AudioManager.Instance.PlaySFX(line.voiceClip);
        }
        else if (!string.IsNullOrEmpty(line.storyDialogueFileName))
        {
            DialogueLoader.Instance.LoadStoryDialogue(line.storyDialogueFileName);
        }
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && dialoguePanel.activeSelf)
        {
            DisplayNextLine();
        }
    }
}