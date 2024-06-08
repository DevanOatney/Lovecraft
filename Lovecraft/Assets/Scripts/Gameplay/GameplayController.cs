using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    public Transform SelectedObject;
    private Transform previewObject;
    private Vector3 originalScale;

    private GridTest buildingGrid;

    private void Start()
    {
        buildingGrid = GameObject.FindObjectOfType<GridTest>();
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
        CreatePreviewObject();
    }

    private void CreatePreviewObject()
    {
        if (previewObject != null)
        {
            Destroy(previewObject.gameObject);
        }

        previewObject = Instantiate(SelectedObject, buildingGrid.GetBuildingPositionFromGrid(), Quaternion.identity);
        originalScale = previewObject.localScale;

        // Set the preview object to be transparent
        SetPreviewObjectTransparency(previewObject, 0.5f);
    }

    private void Update()
    {
        if (previewObject != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                buildingGrid.CreateBuilding(SelectedObject, previewObject.rotation);
                Destroy(previewObject.gameObject);
                previewObject = null;
            }
            MovePreviewObject();
            RotatePreviewObject();
        }
    }

    private void MovePreviewObject()
    {
        previewObject.position = buildingGrid.GetBuildingPositionFromGrid();
    }

    private void RotatePreviewObject()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            previewObject.Rotate(0, 90, 0);
        }
    }

    private void SetPreviewObjectTransparency(Transform obj, float alpha)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material material = renderer.material;
            Color color = material.color;
            color.a = alpha;
            material.color = color;
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 3000;
        }
    }
}