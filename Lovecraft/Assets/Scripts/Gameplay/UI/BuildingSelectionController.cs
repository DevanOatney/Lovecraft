using UnityEngine;

public class BuildingSelectionController : MonoBehaviour
{
    public void BuildingObjectSelected(Transform building)
    {
        GameEventSystem.Instance.TriggerEvent(GameEvent.BUILDING_OBJECT_SELECTED, building);
    }
}
