using Unity.AI.Navigation;
using UnityEngine;

public class GridTest : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public float cellSize = 1.0f;
    public BoxCollider gridCollider;
    public LayerMask buildableSurfaceMask;

    public Grid<BuildingNode> grid;
    private PathfindingCostGrid pfindCostGrid;
    private int previousWidth;
    private int previousHeight;
    private float previousCellSize;
    private GameplayController gameplayController;

    private void Start()
    {
        pfindCostGrid = GetComponentInChildren<PathfindingCostGrid>();
        gameplayController = GameObject.FindObjectOfType<GameplayController>();
        CreateGrid();
    }

    private void Update()
    {
        // Automatically recreate grid if any dimension or cell size changes
        if (gridCollider != null)
        {
            if (cellSize != previousCellSize)
            {
                CreateGrid();
                previousCellSize = cellSize;
            }
        }
        else
        {
            if (width != previousWidth || height != previousHeight || cellSize != previousCellSize)
            {
                CreateGrid();
                previousWidth = width;
                previousHeight = height;
                previousCellSize = cellSize;
            }
        }
    }

    public void CreateBuilding(Transform buildingToCreate, Quaternion rotation)
    {
        Vector3 mousePosition = GetMouseWorldPosition();
        var bNode = grid.GetValue(mousePosition);
        if (bNode.buildingObject == null)
        {
            Vector3 worldPosition = GetBuildingPositionFromGrid();

            bNode.buildingObject = Instantiate(buildingToCreate, worldPosition, rotation);

            BuildingObject buildingObject = bNode.buildingObject.GetComponent<BuildingObject>();
            if (buildingObject != null)
            {
                buildingObject.ParentNode = bNode;
                buildingObject.DirectionToFire = rotation;
            }

           // GameObject.FindObjectOfType<NavMeshSurface>().BuildNavMesh();
        }
    }

    public Vector3 GetBuildingPositionFromGrid()
    {
        Vector3 mousePosition = GetMouseWorldPosition();
        grid.GetXY(mousePosition, out int x, out int y);
        Vector3 pos = grid.GetCenterOfCell(x, y);

        // Perform a raycast downwards from the target position to find the ground level
        if (Physics.Raycast(pos + Vector3.up * 10, Vector3.down, out RaycastHit hit, Mathf.Infinity, buildableSurfaceMask))
        {
            pos.y = hit.point.y;
        }
        return pos;
    }

    private void CreateGrid()
    {
        if (gridCollider != null)
        {
            Vector3 size = gridCollider.size;
            Vector3 scale = gridCollider.transform.lossyScale;
            Vector3 scaledSize = Vector3.Scale(size, scale);

            width = Mathf.CeilToInt(scaledSize.x / cellSize);
            height = Mathf.CeilToInt(scaledSize.z / cellSize);
            Vector3 originPosition = gridCollider.transform.position - new Vector3(scaledSize.x, 0, scaledSize.z) / 2f;

            grid = new Grid<BuildingNode>(width, height, cellSize, originPosition);
        }
        else
        {
            grid = new Grid<BuildingNode>(width, height, cellSize, Vector3.zero);
        }

        if (pfindCostGrid)
        {
            pfindCostGrid.costGrid = grid; 
        }
    }

    public Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point;
        }
        return Vector3.zero;
    }
}

public class BuildingNode
{
    public Transform buildingObject;
    public int pathingCost;

    public BuildingNode()
    {
        buildingObject = null;
        pathingCost = 0;
    }

    public BuildingNode(Transform buildingObject = null, int pathingCost = 0)
    {
        this.buildingObject = buildingObject;
        this.pathingCost = pathingCost;
    }
}