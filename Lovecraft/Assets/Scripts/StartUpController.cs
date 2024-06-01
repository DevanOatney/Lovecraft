using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUpController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.Instance.LoadScene(Scene.MainMenu);
    }
}
