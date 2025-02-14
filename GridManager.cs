using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [Header("Grid Parameters")]
    public float nodeSize = 1f; // match tileSpacing in GridGenerator

    private Node[,] grid;
    private int gridSizeX;
    private int gridSizeY;
    private Vector3 gridOrigin;

    private GridGenerator gridGenerator;

    void Start()
    {
        gridGenerator = GetComponent<GridGenerator>();
        if (gridGenerator == null)
        {
            Debug.LogError("GridGenerator not found on the same GameObject.");
            return;
        }

        gridSizeX = gridGenerator.GetGridSizeX(); // includes outer ring
        gridSizeY = gridGenerator.GetGridSizeY();
        gridOrigin = gridGenerator.GetGridOrigin();

        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 nodePosition = gridOrigin + new Vector3(x * nodeSize, y * nodeSize, 0);

                bool walkable = !IsObstacle(nodePosition);

                // Force the outer ring to be unwalkable
                if (x == 0 || x == gridSizeX - 1 || y == 0 || y == gridSizeY - 1)
                {
                    walkable = false; // invisible boundary
                }

                grid[x, y] = new Node(nodePosition, walkable);
            }
        }
    }

    bool IsObstacle(Vector3 position)
    {
        Collider2D hit = Physics2D.OverlapBox(position, Vector2.one * nodeSize * 0.8f, 0f);
        if (hit != null)
        {
            if (hit.gameObject.CompareTag("Pillar"))
                return true;
        }
        return false;
    }

    // Convert world pos to grid indices, clamp
    public int GetIndexX(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt((worldPos.x - gridOrigin.x) / nodeSize);
        x = Mathf.Clamp(x, 0, gridSizeX - 1);
        return x;
    }

    public int GetIndexY(Vector3 worldPos)
    {
        int y = Mathf.RoundToInt((worldPos.y - gridOrigin.y) / nodeSize);
        y = Mathf.Clamp(y, 0, gridSizeY - 1);
        return y;
    }

    public Node GetNode(int x, int y)
    {
        return grid[x, y];
    }

    public Node GetNodeFromPosition(Vector3 worldPos)
    {
        int x = GetIndexX(worldPos);
        int y = GetIndexY(worldPos);
        return grid[x, y];
    }

    public int GetGridSizeX() { return gridSizeX; }
    public int GetGridSizeY() { return gridSizeY; }
    public Vector3 GetGridOrigin() { return gridOrigin; }

    // BFS expansions in Pathfinding can use the below to get neighbor nodes if needed
    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        int x = GetIndexX(node.Position);
        int y = GetIndexY(node.Position);

        // 4-direction or 8-direction, up to you
        // 4-direction example:
        if (y + 1 < gridSizeY)
            neighbors.Add(grid[x, y + 1]);
        if (y - 1 >= 0)
            neighbors.Add(grid[x, y - 1]);
        if (x - 1 >= 0)
            neighbors.Add(grid[x - 1, y]);
        if (x + 1 < gridSizeX)
            neighbors.Add(grid[x + 1, y]);

        return neighbors;
    }



    void OnDrawGizmos()
    {
        if (grid != null)
        {
            foreach (Node node in grid)
            {
                Gizmos.color = node.Walkable ? Color.white : Color.red;
                Gizmos.DrawCube(node.Position, Vector3.one * (nodeSize * 0.9f));
            }
        }
    }
}
