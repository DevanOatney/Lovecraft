using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicCameraManager : MonoBehaviour
{
    public static CinematicCameraManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Camera MainGamePlayCamera;
    private List<Camera> cinematicCameras;

    // Start is called before the first frame update
    void Start()
    {
        if (cinematicCameras == null)
            InitializeCameras();
    }

    public void ActivateCamera(string _name)
    {
        MainGamePlayCamera.gameObject.SetActive(false);
        if(cinematicCameras == null)
            InitializeCameras();
        foreach(Camera cam in cinematicCameras)
        {
            if (cam.name.Equals(_name))
            {
                cam.gameObject.SetActive(true);
            } else
            {
                cam.gameObject.SetActive(false);
            }
        }
    }

    private void InitializeCameras()
    {
        cinematicCameras = new List<Camera>();
        foreach (Camera cam in GetComponentsInChildren<Camera>())
        {
            cinematicCameras.Add(cam);
            cam.gameObject.SetActive(false);
        }
    }

    public void BackToGameplay()
    {
        MainGamePlayCamera.gameObject.SetActive(true);
        foreach (Camera cam in cinematicCameras)
        {
            cam.gameObject.SetActive(false);
        }
    }
}
