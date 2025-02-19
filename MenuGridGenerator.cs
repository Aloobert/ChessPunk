using UnityEngine;

public class MenuGridGenerator : MonoBehaviour
{
    public GameObject whiteTilePrefab;
    public GameObject blackTilePrefab;
    public GameObject pillarPrefab;

    public int minGridSize = 5;
    public int maxGridSize = 5; // if you want a fixed 5x5, or allow range
    public float tileSpacing = 1f;

    private int gridSizeX;
    private int gridSizeY;
    private Vector3 gridOrigin;

    void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        // We can fix it to the same logic or a simpler approach
        gridSizeX = Random.Range(minGridSize, maxGridSize + 1);
        gridSizeY = gridSizeX; // if you want a square

        // Center the grid at (0,0,0)
        gridOrigin = transform.position - new Vector3((gridSizeX - 1) * tileSpacing / 2f,
                                                      (gridSizeY - 1) * tileSpacing / 2f,
                                                      0);

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 pos = new Vector3(gridOrigin.x + x * tileSpacing,
                                          gridOrigin.y + y * tileSpacing, 0);

                GameObject tilePrefab = ((x + y) % 2 == 0) ? whiteTilePrefab : blackTilePrefab;
                Instantiate(tilePrefab, pos, Quaternion.identity, transform);

                // If you want pillars for decoration:
                if (Random.value < 0.1f && pillarPrefab != null)
                {
                    Instantiate(pillarPrefab, pos, Quaternion.identity, transform);
                }
            }
        }
    }
}
