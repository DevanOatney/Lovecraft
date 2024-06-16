using UnityEngine;

public enum Scene
{
    MainMenu = 1, //starting at index 1 as index 0 is for the "on startup" load scene.
    Gameplay,//not sure if we'll have more scenes.. so for now i'mma just call the meat of the game "gameplay"
    GameOver
}

public class SceneManager : MonoBehaviour
{
    private static SceneManager instance;
    public static SceneManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject singletonObject = new GameObject("SceneManager");
                instance = singletonObject.AddComponent<SceneManager>();
                DontDestroyOnLoad(singletonObject);
            }
            return instance;
        }
        private set
        {
            instance = value;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
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