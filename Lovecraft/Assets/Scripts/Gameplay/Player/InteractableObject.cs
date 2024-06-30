using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] private InputAction InteractionAction;
    public UnityEvent ActionEvent;
    public GameObject Indicator;
    public UnityEvent WalkAwayEvent;

    // Start is called before the first frame update
    void Start()
    {
        InteractionAction.performed += PerformAction;
    }
    private void OnDestroy()
    {
        InteractionAction.Disable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Indicator.SetActive(true);
            InteractionAction.Enable();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Indicator.SetActive(false);
        InteractionAction.Disable();
        WalkAwayEvent.Invoke();
    }

    protected void PerformAction(InputAction.CallbackContext context)
    {
        ActionEvent.Invoke();
    }
 }
