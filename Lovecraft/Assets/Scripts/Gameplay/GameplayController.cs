using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    public Transform SelectedObject;

    // Start is called before the first frame update
    void Start()
    {
        RegisterEventsToListenTo();
    }

    private void OnDestroy()
    {
        UnregisterEventsToListenTo();        
    }

    private void RegisterEventsToListenTo()
    {
        GameEventSystem.Instance.RegisterListener(GameEvent.BUILDING_OBJECT_SELECTED, OnBuildingObjectSelected);
    }

    private void UnregisterEventsToListenTo()
    {
        GameEventSystem.Instance.UnregisterListener(GameEvent.BUILDING_OBJECT_SELECTED, OnBuildingObjectSelected);
    }

    private void OnBuildingObjectSelected(object obj)
    {
        if (obj is Transform == false)
            return;
        SelectedObject = (Transform)obj;
    }
}
