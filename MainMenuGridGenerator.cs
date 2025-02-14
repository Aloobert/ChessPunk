using UnityEngine;

/// <summary>
/// Generates a checkerboard grid for the Main Menu scene,
/// plus an invisible boundary around the outer ring, which is marked unwalkable.
/// Also spawns the player in the center tile.
/// </summary>
public class MainMenuGridGenerator : MonoBehaviour
{
    [Header("Grid Configuration")]
    public int gridWidth = 5;   // playable width (not counting the border)
    public int gridHeight = 5;  // playable height (not counting the border)
    public float tileSize = 1f;

    [Header("Tile Prefabs")]
    public GameObject whiteTilePrefab;
    public GameObject blackTilePrefab;
    public GameObject invisibleWallPrefab;
    public GameObject playerPrefab;

    // Internal grid size (including border)
    private int finalWidth;
    private int finalHeight;
    // The origin point so we can center the grid at (0,0)
    private Vector3 gridOrigin;

    /// <summary>
    /// A simple struct or class to hold each tile's data:
    /// position and whether it's walkable or not.
    /// </summary>
    private TileInfo[,] tileArray;

    void Start()
    {
        GenerateGrid();
        SpawnPlayer();
    }

    /// <summary>
    /// Generates the grid with a +1 border on each side,
    /// checkerboard inside, invisible walls around.
    /// </summary>
    void GenerateGrid()
    {
        // +2 in each dimension to create a one-tile border
        finalWidth = gridWidth + 2;
        finalHeight = gridHeight + 2;

        // Prepare the array
        tileArray = new TileInfo[finalWidth, finalHeight];

        // Calculate the origin so the grid is centered at the object's position
        gridOrigin = transform.position - new Vector3(
            (finalWidth - 1) * tileSize / 2f,
            (finalHeight - 1) * tileSize / 2f,
            0f
        );

        for (int x = 0; x < finalWidth; x++)
        {
            for (int y = 0; y < finalHeight; y++)
            {
                // The world position of this tile
                Vector3 spawnPos = gridOrigin + new Vector3(x * tileSize, y * tileSize, 0f);

                // Create a TileInfo entry
                tileArray[x, y] = new TileInfo
                {
                    position = spawnPos,
                    walkable = true // we'll set false if it's a border
                };

                // If it's on the outer ring => invisible wall, unwalkable
                if (x == 0 || x == finalWidth - 1 || y == 0 || y == finalHeight - 1)
                {
                    tileArray[x, y].walkable = false;

                    // Optionally instantiate an invisible wall prefab
                    if (invisibleWallPrefab != null)
                    {
                        Instantiate(invisibleWallPrefab, spawnPos, Quaternion.identity, transform);
                    }
                }
                else
                {
                    // Checkerboard logic => pick white or black tile
                    bool isWhite = ((x + y) % 2 == 0);
                    GameObject tilePrefab = isWhite ? whiteTilePrefab : blackTilePrefab;
                    if (tilePrefab != null)
                    {
                        Instantiate(tilePrefab, spawnPos, Quaternion.identity, transform);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Spawns the player in the center of the playable area (the tile is definitely walkable).
    /// </summary>
    void SpawnPlayer()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("No playerPrefab assigned in MainMenuGridGenerator!");
            return;
        }

        // The playable area is from x=1..(finalWidth-2), y=1..(finalHeight-2)
        // So the center is finalWidth/2, finalHeight/2
        int centerX = finalWidth / 2;
        int centerY = finalHeight / 2;

        Vector3 playerPos = tileArray[centerX, centerY].position;
        Instantiate(playerPrefab, playerPos, Quaternion.identity);
    }

    /// <summary>
    /// Checks whether the given world position is walkable (true) or not (false).
    /// Use this in your player movement code to disallow moving onto invisible walls.
    /// </summary>
    public bool IsWalkable(Vector3 pos)
    {
        // Convert the world position to grid indices
        int x = Mathf.RoundToInt((pos.x - gridOrigin.x) / tileSize);
        int y = Mathf.RoundToInt((pos.y - gridOrigin.y) / tileSize);

        // Check bounds
        if (x < 0 || x >= finalWidth || y < 0 || y >= finalHeight)
            return false;

        return tileArray[x, y].walkable;
    }
}

/// <summary>
/// Simple data structure holding each tile's position and walkability.
/// </summary>
public class TileInfo
{
    public Vector3 position;
    public bool walkable;
}
