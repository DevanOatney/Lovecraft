using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speakerName;
    public string dialogueText;
    public string portraitName; // Name of the portrait image in Resources/Portraits
    [Tooltip("Pretty sure this should always be set to RandomBark")]
    public SFXType voiceClip; // SFX type for the voice clip... if we want random ones I guess? this is a terrible idea lol
    [Tooltip("Set to true for speech bubble, false for main story UI dialogue")]
    public bool isRandomBark;
    public AudioClip storyAudioClip; // Specific AudioClip for StoryDialogue
    [HideInInspector] public Sprite portrait; // Loaded portrait sprite
}

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue System/Dialogue")]
public class Dialogue : ScriptableObject
{
    public string dialogueName;
    public List<DialogueLine> lines;
}