using System.Collections.Generic;
using UnityEngine;

public class DialogueLoader : MonoBehaviour
{
    public static DialogueLoader Instance { get; private set; }

    private Dictionary<string, Dialogue> dialogues;
    private Dictionary<string, AudioClip> audioClips;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            dialogues = new Dictionary<string, Dialogue>();
            audioClips = new Dictionary<string, AudioClip>();
            LoadAllAudioClips();
            LoadAllDialogues();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadAllAudioClips()
    {
        AudioClip[] clips = Resources.LoadAll<AudioClip>("StoryDialogue/");
        foreach (AudioClip clip in clips)
        {
            if (!audioClips.ContainsKey(clip.name))
            {
                audioClips[clip.name] = clip;
            }
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
                    if (line.storyAudioClip != null)
                    {
                        if (audioClips.TryGetValue(line.storyAudioClip.name, out AudioClip clip))
                        {
                            line.storyAudioClip = clip;
                        }
                        else
                        {
                            Debug.LogWarning("AudioClip not found for name: " + line.storyAudioClip.name);
                        }
                    }
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
