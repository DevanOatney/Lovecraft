using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    public Transform SelectedObject;
    public GameOverController GameOverScene;
    private Transform previewObject;
    private Vector3 originalScale;

    private LineRenderer lineRenderer;
    private GridTest buildingGrid;

    private float creationSpeed = 0.15f;
    private float creationTimer = 0f;

    private bool waitingForGameOver = false;

    private void Start()
    {
        buildingGrid = GameObject.FindObjectOfType<GridTest>();
        GameOverScene = GameObject.FindObjectOfType<GameOverController>();
        RegisterEventsToListenTo();
        BakeNavMeshes();
    }

    private void BakeNavMeshes()
    {
        var nMeshes = GameObject.FindObjectsOfType<NavMeshSurface>();
        foreach (var nMesh in nMeshes)
        {
            nMesh.BuildNavMesh();
        }

        RoundController.Instance.SetSceneInitialized(true);
    }

    private void OnDestroy()
    {
        UnregisterEventsToListenTo();
    }

    private void RegisterEventsToListenTo()
    {
        GameEventSystem.Instance.RegisterListener(GameEvent.BUILDING_OBJECT_SELECTED, OnBuildingObjectSelected);
        GameEventSystem.Instance.RegisterListener(GameEvent.PLAYER_KILLED, OnPlayerKilled);
        GameEventSystem.Instance.RegisterListener(GameEvent.DIALOGUE_COMPLETE, OnDialogueComplete);
    }

    private void UnregisterEventsToListenTo()
    {
        GameEventSystem.Instance.UnregisterListener(GameEvent.BUILDING_OBJECT_SELECTED, OnBuildingObjectSelected);
        GameEventSystem.Instance.UnregisterListener(GameEvent.PLAYER_KILLED, OnPlayerKilled);
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
        var buildingObj = previewObject.GetComponent<BuildingObject>();
        if (buildingObj != null)
        {
            buildingObj.IsInPreviewMode = true;
        }

        originalScale = previewObject.localScale;

        // Set the preview object to be transparent
        SetPreviewObjectTransparency(previewObject, 0.5f);

        if (previewObject.GetComponent<UnityEngine.AI.NavMeshObstacle>() != null) {
            previewObject.GetComponent<UnityEngine.AI.NavMeshObstacle>().enabled = false;
        }
        previewObject.GetComponentInChildren<BoxCollider>().enabled = false;

        // Add LineRenderer to the preview object for the arrow
        lineRenderer = previewObject.gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 3;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
    }

    private void Update()
    {
        if (previewObject != null)
        {
            MovePreviewObject();
            RotatePreviewObject();
            DrawArrow();

            if (creationTimer >= creationSpeed)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    creationTimer = 0f;
                    buildingGrid.CreateBuilding(SelectedObject, previewObject.rotation);
                    Destroy(previewObject.gameObject);
                    previewObject = null;

                    GameEventSystem.Instance.TriggerEvent(GameEvent.BUILDING_OBJECT_PLACE, null);
                }
            }
            else
                creationTimer += Time.deltaTime;

            if (Input.GetMouseButtonDown(1))
            {
                Destroy(previewObject.gameObject);
                previewObject = null;
            }
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

    private void DrawArrow()
    {
        Vector3 arrowStart = previewObject.position;
        Vector3 arrowEnd = previewObject.position + previewObject.forward * 2.0f; // Adjust length as needed
        Vector3 arrowHeadLeft = arrowEnd + Quaternion.Euler(0, 150, 0) * (previewObject.forward * 0.5f); // Adjust size as needed
        Vector3 arrowHeadRight = arrowEnd + Quaternion.Euler(0, -150, 0) * (previewObject.forward * 0.5f);

        BuildingObject bObj = previewObject.GetComponent<BuildingObject>();
        if(bObj != null)
        {
            if(bObj.ProjectilePrefab != null)
            {
                lineRenderer.SetPosition(0, arrowStart);
                lineRenderer.SetPosition(1, arrowEnd);
                lineRenderer.SetPosition(2, arrowHeadLeft);
            }
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

    private void OnPlayerKilled(object data)
    {
        Time.timeScale = 0f;
        GameOverScene.gameObject.SetActive(true);
        GameOverScene.OnGameOver();
        waitingForGameOver = true;
    }

    private void OnDialogueComplete(object data)
    {
        if (waitingForGameOver)
        {
            SceneManager.Instance.ReloadCurrentScene();
        }
    }
}