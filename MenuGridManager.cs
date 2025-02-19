using UnityEngine;

public class MenuGridManager : MonoBehaviour
{
    public float nodeSize = 1f;
    private Node[,] grid;
    private int gridSizeX;
    private int gridSizeY;
    private Vector3 gridOrigin;

    private MenuGridGenerator generator;

    void Start()
    {
        // Find MenuGridGenerator on this GameObject
        generator = GetComponent<MenuGridGenerator>();
        if (generator == null)
        {
            Debug.LogWarning("No MenuGridGenerator found. The grid might not match the visuals.");
            return;
        }

        // Suppose the generator sets final gridSizeX, gridSizeY in its code. 
        // Or we just read the same minGridSize, maxGridSize:
        gridSizeX = generator.maxGridSize;
        gridSizeY = generator.maxGridSize;

        // Center it at transform.position
        gridOrigin = transform.position - new Vector3((gridSizeX - 1) * nodeSize / 2f,
                                                      (gridSizeY - 1) * nodeSize / 2f,
                                                      0);

        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 nodePos = gridOrigin + new Vector3(x * nodeSize, y * nodeSize, 0);

                // For the interior, check if there's a pillar, etc.
                bool walkable = !IsObstacle(nodePos);

                // Force the outer ring to be unwalkable
                if (x == 0 || x == gridSizeX - 1 || y == 0 || y == gridSizeY - 1)
                {
                    walkable = false;
                }

                grid[x, y] = new Node(nodePos, walkable);
            }
        }
    }

    bool IsObstacle(Vector3 position)
    {
        // Check if there's a pillar at 'position'
        Collider2D hit = Physics2D.OverlapBox(position, Vector2.one * nodeSize * 0.8f, 0f);
        if (hit != null && hit.CompareTag("Pillar"))
        {
            return true;
        }
        return false;
    }

    public Node GetNodeFromPosition(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt((worldPos.x - gridOrigin.x) / nodeSize);
        int y = Mathf.RoundToInt((worldPos.y - gridOrigin.y) / nodeSize);
        x = Mathf.Clamp(x, 0, gridSizeX - 1);
        y = Mathf.Clamp(y, 0, gridSizeY - 1);
        return grid[x, y];
    }
}
