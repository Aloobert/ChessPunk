using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    // Tile Prefabs
    public GameObject whiteTilePrefab;
    public GameObject blackTilePrefab;
    public GameObject pillarPrefab;
    public GameObject bossPrefab;
    public GameObject playerPrefab;

    // Grid Configuration
    [Header("Grid Size Configuration")]
    public int minGridSize = 5;   // Must be odd
    public int maxGridSize = 15;  // Must be odd
    public float tileSpacing = 1.0f;

    // Internal Grid Parameters
    private int gridSizeX;     // The “playable” dimension
    private int gridSizeY;     // The “playable” dimension
    private int finalSizeX;    // +2 for invisible boundary
    private int finalSizeY;    // +2 for invisible boundary
    private Vector3 gridOrigin;

    void Start()
    {
        GenerateGrid();
        GenerateBoss();
        GeneratePlayer();
    }

    void GenerateGrid()
    {
        // Randomly determine grid size within specified range, ensuring it's odd
        gridSizeX = GetRandomOddNumber(minGridSize, maxGridSize);
        gridSizeY = GetRandomOddNumber(minGridSize, maxGridSize);

        // We add +2 in each dimension to create an outer ring
        finalSizeX = gridSizeX + 2;
        finalSizeY = gridSizeY + 2;

        // Calculate grid origin so that the center tile of the "playable area" aligns with (0,0,0)
        // The playable area is from (1..finalSizeX-2, 1..finalSizeY-2)
        // We'll shift so (1,1) is near -some offset
        gridOrigin = transform.position - new Vector3(
            (finalSizeX - 1) * tileSpacing / 2f,
            (finalSizeY - 1) * tileSpacing / 2f,
            0
        );

        // Create actual visible tiles for the playable area
        // plus we skip outer ring if we want it invisible
        for (int x = 1; x < finalSizeX - 1; x++)
        {
            for (int y = 1; y < finalSizeY - 1; y++)
            {
                Vector3 position = new Vector3(
                    gridOrigin.x + x * tileSpacing,
                    gridOrigin.y + y * tileSpacing,
                    0
                );

                // Alternate between white and black tiles
                // We can do (x + y) or (x-1 + y-1) depending on preference
                GameObject tileToInstantiate = ((x + y) % 2 == 0) ? whiteTilePrefab : blackTilePrefab;
                Instantiate(tileToInstantiate, position, Quaternion.identity, this.transform);

                // Randomly place pillars, avoiding first/last row => interpret that as y != 1, y != finalSizeY-2
                if (Random.value < 0.2f && y != 1 && y != finalSizeY - 2)
                {
                    Instantiate(pillarPrefab, position, Quaternion.identity, this.transform);
                }
            }
        }
    }

    void GenerateBoss()
    {
        if (bossPrefab == null)
        {
            Debug.LogError("Boss Prefab not assigned in the GridGenerator.");
            return;
        }

        // Place Boss at the top center tile in the playable region
        int topY = finalSizeY - 2; // playable top row
        int centerX = (finalSizeX / 2);

        Vector3 bossPosition = gridOrigin + new Vector3(centerX * tileSpacing, topY * tileSpacing, 0);
        GameObject bossInstance = Instantiate(bossPrefab, bossPosition, Quaternion.identity, this.transform);
        bossInstance.name = "Boss";
        bossInstance.tag = "Boss";
    }

    void GeneratePlayer()
    {
        if (GameSettings.SelectedHeroPrefab == null)
        {
            Debug.LogError("No hero selected! Please ensure a hero is selected in the Hero Select scene.");
            return;
        }

        // Calculate the spawn position for the player (e.g., bottom center tile)
        Vector3 playerPosition = gridOrigin + new Vector3(
            Mathf.FloorToInt(gridSizeX / 2) * tileSpacing,
            0,
            0
        );

        GameObject playerInstance = Instantiate(GameSettings.SelectedHeroPrefab, playerPosition, Quaternion.identity, transform);
        playerInstance.name = "Player";
        playerInstance.tag = "Player";

        // (Optional) After instantiation, you can also pass ability selections.
        // For example, if the player prefab has a PlayerAbilities component:
        PlayerAbilities pa = playerInstance.GetComponent<PlayerAbilities>();
        if (pa != null)
        {
            pa.SetupAbilities(GameSettings.SelectedAbilityPrefabs);

        }
    }


    // Helper method to get a random odd number within a range
    int GetRandomOddNumber(int min, int max)
    {
        if (min % 2 == 0) min += 1;
        if (max % 2 == 0) max -= 1;
        if (min > max) min = max;
        return Random.Range(min / 2, (max / 2) + 1) * 2 + 1;
    }

    // Public getters
    public int GetGridSizeX()
    {
        return finalSizeX; // The entire array size including outer ring
    }

    public int GetGridSizeY()
    {
        return finalSizeY;
    }

    public Vector3 GetGridOrigin()
    {
        return gridOrigin;
    }
}
