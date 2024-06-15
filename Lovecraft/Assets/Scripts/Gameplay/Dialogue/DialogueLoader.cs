using UnityEngine;

public class DialogueLoader : MonoBehaviour
{
    public static DialogueLoader Instance { get; private set; }

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

    public void LoadDialogue(string fileName)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Dialogues/" + fileName);
        if (jsonFile != null)
        {
            Dialogue dialogue = JsonUtility.FromJson<Dialogue>(jsonFile.text);
            foreach (var line in dialogue.lines)
            {
                line.portrait = Resources.Load<Sprite>("Portraits/" + line.portraitName);
            }
            DialogueManager.Instance.StartDialogue(dialogue);
        }
        else
        {
            Debug.LogError("Dialogue file not found: " + fileName);
        }
    }
}