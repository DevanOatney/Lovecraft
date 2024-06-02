using Unity.AI.Navigation;
using UnityEngine;

public class GridTest : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public float cellSize = 1.0f;
    public BoxCollider gridCollider;

    private Grid<BuildingNode> grid;
    private PathfindingCostGrid pfindCostGrid;
    private int previousWidth;
    private int previousHeight;
    private float previousCellSize;

    //TEMP, remove me
    public Transform BuildingPrefab;

    private void Start()
    {
        pfindCostGrid = GetComponentInChildren<PathfindingCostGrid>();
        CreateGrid();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Vector3 mousePosition = GetMouseWorldPosition();
            grid.GetValue(mousePosition).pathingCost += 10;
        }
         
        if (Input.GetKeyDown(KeyCode.E))
        {
            Vector3 mousePosition = GetMouseWorldPosition();
            int value = grid.GetValue(mousePosition).pathingCost;
            Debug.Log("Grid Value: " + value);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Vector3 mousePosition = GetMouseWorldPosition();
            var bNode = grid.GetValue(mousePosition);
            if(bNode.buildingObject == null)
            {
                grid.GetXY(mousePosition, out int x, out int y);
                bNode.buildingObject = Instantiate(BuildingPrefab, grid.GetWorldPosition(x, y), Quaternion.identity);
                Vector3 pos = bNode.buildingObject.transform.position;
                bNode.buildingObject.transform.position = new Vector3(pos.x, mousePosition.y, pos.z);
                
                GameObject.FindObjectOfType<NavMeshSurface>().BuildNavMesh();
            }
        }


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

    private Vector3 GetMouseWorldPosition()
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