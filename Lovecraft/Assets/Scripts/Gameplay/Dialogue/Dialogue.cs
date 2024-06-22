using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speakerName;
    public string dialogueText;
    public string portraitName; // Name of the portrait image in Resources/Portraits
    public SFXType voiceClip; // SFX type for the voice clip... if we want random ones I guess? this is a terrible idea lol
    public string storyDialogueFileName;
    [HideInInspector] public Sprite portrait; // Loaded portrait sprite
}
[System.Serializable]
public class Dialogue
{
    public List<DialogueLine> lines;
}