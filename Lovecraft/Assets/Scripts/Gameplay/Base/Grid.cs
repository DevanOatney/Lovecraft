using System;
using UnityEngine;

public class Grid<T>
{
    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;
    private T[,] gridArray;

    public int Width { get { return width; } }
    public int Height { get { return height; } }
    public float CellSize { get { return cellSize; } }

    public Grid(int width, int height, float cellSize, Vector3 originPosition)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new T[width, height];

        if (typeof(T) == typeof(int) || typeof(T) == typeof(float) || typeof(T) == typeof(double) || typeof(T) == typeof(string))
        {
            // No need to initialize primitive types
            return;
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                try
                {
                    gridArray[x, y] = Activator.CreateInstance<T>();
                }
                catch (MissingMethodException)
                {
                    Debug.LogError($"Type {typeof(T)} must have a parameterless constructor.");
                    throw;
                }
            }
        }

        DrawGrid();
    }


    private void DrawGrid()
    {
        Vector3 debugDrawOffset = new Vector3(0f, 0.1f, 0f);
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                Color lineColor = GetColorBasedOnValue(gridArray[x, y]);
                Debug.DrawLine(GetWorldPosition(x, y) + debugDrawOffset, GetWorldPosition(x, y + 1) + debugDrawOffset, lineColor, 100f);
                Debug.DrawLine(GetWorldPosition(x, y) + debugDrawOffset, GetWorldPosition(x + 1, y) + debugDrawOffset, lineColor, 100f);
            }
        }
        Debug.DrawLine(GetWorldPosition(0, height) + debugDrawOffset, GetWorldPosition(width, height) + debugDrawOffset, Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(width, 0) + debugDrawOffset, GetWorldPosition(width, height) + debugDrawOffset, Color.white, 100f);
    }

    private Color GetColorBasedOnValue(T value)
    {
        if (value is int intValue)
        {
            // Example: Higher values are closer to red, lower values closer to green
            float normalizedValue = Mathf.InverseLerp(0, 100, intValue); // Adjust the range as needed
            return Color.Lerp(Color.green, Color.red, normalizedValue);
        }
        else if (value is BuildingNode buildingNode)
        {
            // Example: Higher pathingCost values are closer to red, lower values closer to green
            float normalizedValue = Mathf.InverseLerp(0, 100, buildingNode.pathingCost); // Adjust the range as needed
            return Color.Lerp(Color.green, Color.red, normalizedValue);
        }
        return Color.white;
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, 0, y) * cellSize + originPosition;
    }

    public Vector3 GetCenterOfCell(int x, int y)
    {
        return new Vector3(x + 0.5f, 0, y + 0.5f) * cellSize + originPosition;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
    }

    public void SetValue(int x, int y, T value)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = value;
            DrawGrid();
        }
    }

    public void SetValue(Vector3 worldPosition, T value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetValue(x, y, value);
    }

    public T GetValue(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x, y];
        }
        else
        {
            return default(T);
        }
    }

    public T GetValue(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetValue(x, y);
    }
}