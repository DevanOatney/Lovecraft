using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MouseInputManager : MonoBehaviour
{
    private static MouseInputManager _instance;

    public static MouseInputManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Try to find an existing instance in the scene
                _instance = FindObjectOfType<MouseInputManager>();

                // If no instance exists, create a new one
                if (_instance == null)
                {
                    GameObject obj = new GameObject("MouseInputManager");
                    _instance = obj.AddComponent<MouseInputManager>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else if (_instance != this)
        {
            Destroy(gameObject); // Enforce singleton pattern
        }
    }

    public bool IsPointerOverUIElement()
    {
        if( EventSystem.current == null ) { return false; }
        return EventSystem.current.IsPointerOverGameObject();
    }

    public Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point;
        }
        return Vector3.zero;
    }
}