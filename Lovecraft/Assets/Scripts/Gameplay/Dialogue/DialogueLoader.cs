using System.Collections.Generic;
using UnityEngine;

public class DialogueLoader : MonoBehaviour
{
    public static DialogueLoader Instance { get; private set; }

    private Dictionary<string, Dialogue> dialogues;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            dialogues = new Dictionary<string, Dialogue>();
            LoadAllDialogues();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadAllDialogues()
    {
        Dialogue[] loadedDialogues = Resources.LoadAll<Dialogue>("Dialogue/");

        foreach (Dialogue dialogue in loadedDialogues)
        {
            foreach (var line in dialogue.lines)
            {
                line.portrait = Resources.Load<Sprite>("Portraits/" + line.portraitName);
                if (!line.isRandomBark)
                {
                    if(line.storyAudioClip != null)
                        line.storyAudioClip = Resources.Load<AudioClip>("StoryDialogue/" + line.storyAudioClip.name);
                }
            }
            dialogues[dialogue.dialogueName] = dialogue;
        }
    }

    public Dialogue GetDialogue(string dialogueName)
    {
        if (dialogues.TryGetValue(dialogueName, out Dialogue dialogue))
        {
            return dialogue;
        }
        else
        {
            Debug.LogError("Dialogue not found: " + dialogueName);
            return null;
        }
    }
}