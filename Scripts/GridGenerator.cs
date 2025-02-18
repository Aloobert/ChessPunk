using UnityEngine;
using System.Collections.Generic;

public class GridGenerator : MonoBehaviour
{
    // Tile Prefabs
    public GameObject whiteTilePrefab;
    public GameObject blackTilePrefab;
    public GameObject pillarPrefab;
    public GameObject bossPrefab;
    public GameObject playerPrefab;

    [Header("Grid Size Configuration")]
    public int minGridSize = 5;  // Must be odd
    public int maxGridSize = 15; // Must be odd
    public float tileSpacing = 1.0f;

    // Internal Grid Parameters
    private int gridSizeX; // playable width
    private int gridSizeY; // playable height
    private int finalSizeX; // playable area + 2 (outer boundary)
    private int finalSizeY; // playable area + 2 (outer boundary)
    private Vector3 gridOrigin;

    // 2D array representing the playable area (true = walkable, false = blocked)
    private bool[,] walkable;

    // List of corridor cells (indices in playable grid coordinates)
    private List<Vector2Int> corridorCells;

    void Start()
    {
        // Generate the grid using a carved corridor method.
        GenerateGridInternalWithRandomCorridor();
        GenerateBoss();
        GeneratePlayer();
    }

    // This method carves a corridor from the bottom center to some cell on the top row.
    // Horizontal moves (which could cause a Y shape) have been removed.
    void GenerateGridInternalWithRandomCorridor()
    {
        // Choose playable grid dimensions.
        gridSizeX = GetRandomOddNumber(minGridSize, maxGridSize);
        gridSizeY = GetRandomOddNumber(minGridSize, maxGridSize);

        // The final size includes an outer boundary (1 tile on each side).
        finalSizeX = gridSizeX + 2;
        finalSizeY = gridSizeY + 2;

        // Calculate grid origin so that the playable area is centered relative to this GameObject's position.
        gridOrigin = transform.position - new Vector3((finalSizeX - 1) * tileSpacing / 2f,
                                                        (finalSizeY - 1) * tileSpacing / 2f,
                                                        0);

        // Initialize the walkable array for the playable area.
        walkable = new bool[gridSizeX, gridSizeY];
        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeY; j++)
            {
                walkable[i, j] = true; // start with all cells walkable
            }
        }

        // Generate a corridor using a biased random walk.
        corridorCells = new List<Vector2Int>();
        Vector2Int currentCell = new Vector2Int(gridSizeX / 2, 0); // start at bottom center
        corridorCells.Add(currentCell);

        // Continue walking until we reach the top row.
        while (currentCell.y < gridSizeY - 1)
        {
            // Build a list of possible moves.
            // Allowed moves: up (0,1), up-left (-1,1), and up-right (1,1)
            List<Vector2Int> moves = new List<Vector2Int>();
            moves.Add(new Vector2Int(0, 1)); // always allow up

            if (currentCell.x > 0) moves.Add(new Vector2Int(-1, 1));  // up-left
            if (currentCell.x < gridSizeX - 1) moves.Add(new Vector2Int(1, 1)); // up-right

            // Weight the moves – we want upward moves to be most likely.
            List<Vector2Int> weightedMoves = new List<Vector2Int>();
            foreach (Vector2Int move in moves)
            {
                if (move == new Vector2Int(0, 1))
                {
                    for (int k = 0; k < 3; k++) weightedMoves.Add(move);
                }
                else // diagonals
                {
                    for (int k = 0; k < 2; k++) weightedMoves.Add(move);
                }
            }

            // Choose a move at random from the weighted list.
            int index = Random.Range(0, weightedMoves.Count);
            Vector2Int chosenMove = weightedMoves[index];
            Vector2Int nextCell = currentCell + chosenMove;

            // Clamp next cell within playable bounds.
            nextCell.x = Mathf.Clamp(nextCell.x, 0, gridSizeX - 1);
            nextCell.y = Mathf.Clamp(nextCell.y, 0, gridSizeY - 1);

            // If the cell is already in the corridor, force upward progress.
            if (corridorCells.Contains(nextCell))
            {
                nextCell = currentCell + new Vector2Int(0, 1);
                nextCell.y = Mathf.Clamp(nextCell.y, 0, gridSizeY - 1);
            }

            currentCell = nextCell;
            corridorCells.Add(currentCell);
        }

        // Mark corridor cells as guaranteed walkable.
        foreach (Vector2Int cell in corridorCells)
        {
            walkable[cell.x, cell.y] = true;
        }

        // Instantiate playable area tiles.
        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeY; j++)
            {
                int worldX = i + 1; // offset for outer boundary
                int worldY = j + 1;
                Vector3 position = gridOrigin + new Vector3(worldX * tileSpacing, worldY * tileSpacing, 0);

                // Alternate tile colors for a checkered look.
                GameObject tileToInstantiate = ((worldX + worldY) % 2 == 0) ? whiteTilePrefab : blackTilePrefab;
                Instantiate(tileToInstantiate, position, Quaternion.identity, transform);

                // Place pillars randomly except in corridor cells and the bottom/top rows.
                if (!corridorCells.Contains(new Vector2Int(i, j)) && Random.value < 0.2f && j != 0 && j != gridSizeY - 1)
                {
                    Instantiate(pillarPrefab, position, Quaternion.identity, transform);
                    walkable[i, j] = false;
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
        // Place the Boss at the top center of the playable area.
        int bossPlayableX = gridSizeX / 2;
        int bossPlayableY = gridSizeY - 1;
        int worldX = bossPlayableX + 1;
        int worldY = bossPlayableY + 1;
        Vector3 bossPosition = gridOrigin + new Vector3(worldX * tileSpacing, worldY * tileSpacing, 0);
        GameObject bossInstance = Instantiate(bossPrefab, bossPosition, Quaternion.identity, transform);
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
        // Place the Player at the bottom center of the playable area.
        int playerPlayableX = gridSizeX / 2;
        int playerPlayableY = 0;
        int worldX = playerPlayableX + 1;
        int worldY = playerPlayableY + 1;
        Vector3 playerPosition = gridOrigin + new Vector3(worldX * tileSpacing, worldY * tileSpacing, 0);
        GameObject playerInstance = Instantiate(GameSettings.SelectedHeroPrefab, playerPosition, Quaternion.identity, transform);
        playerInstance.name = "Player";
        playerInstance.tag = "Player";

        // (Optional) Setup player's abilities.
        PlayerAbilities pa = playerInstance.GetComponent<PlayerAbilities>();
        if (pa != null)
        {
            pa.SetupAbilities(GameSettings.SelectedAbilityPrefabs);
        }
    }

    // Helper method: Returns a random odd number between min and max (inclusive).
    int GetRandomOddNumber(int min, int max)
    {
        if (min % 2 == 0) min += 1;
        if (max % 2 == 0) max -= 1;
        if (min > max) min = max;
        return Random.Range(min / 2, (max / 2) + 1) * 2 + 1;
    }

    // Optional public getters.
    public int GetGridSizeX() { return finalSizeX; }
    public int GetGridSizeY() { return finalSizeY; }
    public Vector3 GetGridOrigin() { return gridOrigin; }
}
