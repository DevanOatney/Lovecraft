using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System;

public class PathfindingCostGrid : MonoBehaviour
{
    public Grid<int> costGrid;

    public List<Vector3> FindPath(NavMeshAgent agent, Vector3 startPosition, Vector3 endPosition)
    {
        List<Vector3> path = new List<Vector3>();

        NavMeshPath navMeshPath = new NavMeshPath();
        agent.CalculatePath(endPosition, navMeshPath);

        if (navMeshPath.status != NavMeshPathStatus.PathComplete)
        {
            return path; // Return empty path if NavMesh path is incomplete
        }

        path.AddRange(navMeshPath.corners);

        // Adjust path based on grid costs using A* algorithm
        List<Vector3> adjustedPath = AStarPathfinding(agent, startPosition, endPosition);

        // Return the adjusted path if found, otherwise return the original path
        return adjustedPath.Count > 0 ? adjustedPath : path;
    }

    private List<Vector3> AStarPathfinding(NavMeshAgent agent, Vector3 start, Vector3 goal)
    {
        PriorityQueue<Node> openList = new PriorityQueue<Node>();
        HashSet<Node> closedList = new HashSet<Node>();

        Node startNode = new Node(start, null, 0, GetHeuristic(start, goal));
        openList.Enqueue(startNode);

        while (openList.Count > 0)
        {
            Node currentNode = openList.Dequeue();

            float distanceToGoal = Vector3.Distance(currentNode.Position, goal);
            if (distanceToGoal <= costGrid.CellSize)
            {
                return RetracePath(currentNode);
            }

            closedList.Add(currentNode);

            foreach (var neighbor in GetNeighbors(currentNode.Position))
            {
                int neighborCost = costGrid.GetValue(neighbor);
                float newMovementCostToNeighbor = currentNode.GCost + Vector3.Distance(currentNode.Position, neighbor) + neighborCost;

                Node neighborNode = new Node(neighbor, currentNode, newMovementCostToNeighbor, GetHeuristic(neighbor, goal));

                if (closedList.Contains(neighborNode) || openList.Contains(neighborNode))
                {
                    continue;
                }

                openList.Enqueue(neighborNode);
            }
        }

        return new List<Vector3>(); // Return empty list if no better path found
    }

    private List<Vector3> RetracePath(Node endNode)
    {
        List<Vector3> path = new List<Vector3>();
        Node currentNode = endNode;

        while (currentNode != null)
        {
            path.Add(currentNode.Position);
            currentNode = currentNode.Parent;
        }

        path.Reverse();
        return path;
    }

    private IEnumerable<Vector3> GetNeighbors(Vector3 position)
    {
        int x, y;
        costGrid.GetXY(position, out x, out y);
        List<Vector3> neighbors = new List<Vector3>();

        if (x + 1 < costGrid.Width) neighbors.Add(costGrid.GetCenterOfCell(x + 1, y));
        if (x - 1 >= 0) neighbors.Add(costGrid.GetCenterOfCell(x - 1, y));
        if (y + 1 < costGrid.Height) neighbors.Add(costGrid.GetCenterOfCell(x, y + 1));
        if (y - 1 >= 0) neighbors.Add(costGrid.GetCenterOfCell(x, y - 1));

        return neighbors;
    }

    private float GetHeuristic(Vector3 a, Vector3 b)
    {
        return Vector3.Distance(a, b);
    }

    private class Node : IComparable<Node>
    {
        public Vector3 Position { get; }
        public Node Parent { get; }
        public float GCost { get; }
        public float HCost { get; }
        public float FCost => GCost + HCost;

        public Node(Vector3 position, Node parent, float gCost, float hCost)
        {
            Position = position;
            Parent = parent;
            GCost = gCost;
            HCost = hCost;
        }

        public int CompareTo(Node other)
        {
            return FCost.CompareTo(other.FCost);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Node))
            {
                return false;
            }

            return Position.Equals(((Node)obj).Position);
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }
    }

    private class PriorityQueue<T> where T : IComparable<T>
    {
        private List<T> elements = new List<T>();

        public int Count => elements.Count;

        public void Enqueue(T item)
        {
            elements.Add(item);
            elements.Sort();
        }

        public T Dequeue()
        {
            T item = elements[0];
            elements.RemoveAt(0);
            return item;
        }

        public bool Contains(T item)
        {
            return elements.Contains(item);
        }
    }
}