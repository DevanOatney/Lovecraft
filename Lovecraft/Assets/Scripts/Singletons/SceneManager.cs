using UnityEngine;

public enum Scene
{
    MainMenu = 1, //starting at index 1 as index 0 is for the "on startup" load scene.
    Gameplay,//not sure if we'll have more scenes.. so for now i'mma just call the meat of the game "gameplay"
    GameOver
}

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance { get; private set; }

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

    public void LoadScene(Scene scene)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene((int)scene);
    }

    public void ReloadCurrentScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}