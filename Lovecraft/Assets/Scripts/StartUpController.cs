using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUpController : MonoBehaviour
{

    private void Start()
    {
        Invoke("LoadMainMenu", Random.Range(12, 18));    
    }

    // Start is called before the first frame update
    public void LoadMainMenu()
    {
        SceneManager.Instance.LoadScene(Scene.MainMenu);
    }
}
