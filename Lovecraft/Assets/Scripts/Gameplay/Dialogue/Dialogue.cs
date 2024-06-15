using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speakerName;
    public string dialogueText;
    public string portraitName; // Name of the portrait image in Resources/Portraits
    [HideInInspector] public Sprite portrait; // Loaded portrait sprite
}
[System.Serializable]
public class Dialogue
{
    public List<DialogueLine> lines;
}