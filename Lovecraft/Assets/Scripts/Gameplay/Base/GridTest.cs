using UnityEngine;

public class GridTest : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public float cellSize = 1.0f;
    public BoxCollider gridCollider;

    private Grid<int> grid;
    private PathfindingCostGrid pfindCostGrid;
    private int previousWidth;
    private int previousHeight;
    private float previousCellSize;

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
            grid.SetValue(mousePosition, grid.GetValue(mousePosition) + 10);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Vector3 mousePosition = GetMouseWorldPosition();
            int value = grid.GetValue(mousePosition);
            Debug.Log("Grid Value: " + value);
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

            grid = new Grid<int>(width, height, cellSize, originPosition);
        }
        else
        {
            grid = new Grid<int>(width, height, cellSize, Vector3.zero);
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