using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject whiteTilePrefab;    // White floor tile prefab
    public GameObject blackTilePrefab;    // Black floor tile prefab
    public GameObject bossDoorPrefab;     // Boss door prefab
    public GameObject defaultPlayerPrefab;  // Fallback player prefab if GameSettings.SelectedHeroPrefab is null

    [Header("Grid Settings")]
    [Tooltip("Overall grid size must be at least 5 for this layout.")]
    public int gridRows = 5;
    [Tooltip("Overall grid size must be at least 5 for this layout.")]
    public int gridColumns = 5;
    public float tileSpacing = 1.0f;

    // 5x5 grid positions array.
    private Vector2[,] gridPositions;

    // Static list of allowed positions (inner 3x3 plus designated boss door positions).
    public static List<Vector2> allowedPositions = new List<Vector2>();

    void Start()
    {
        // Ensure grid dimensions are valid.
        if (gridRows < 5 || gridColumns < 5)
        {
            Debug.LogError("Grid dimensions must be at least 5x5 for the lobby layout to work.");
            return;
        }

        GenerateGrid();
        SpawnBossDoors();
        SpawnPlayer();
    }

    void GenerateGrid()
    {
        gridPositions = new Vector2[gridColumns, gridRows];
        int startX = -(gridColumns / 2); // For 5, startX = -2
        int startY = -(gridRows / 2);    // For 5, startY = -2

        allowedPositions.Clear();

        // Build the 5x5 grid.
        for (int x = 0; x < gridColumns; x++)
        {
            for (int y = 0; y < gridRows; y++)
            {
                Vector2 pos = new Vector2((startX + x) * tileSpacing, (startY + y) * tileSpacing);
                gridPositions[x, y] = pos;

                // Only display floor tiles for inner 3x3 area (indices 1,2,3).
                if (x >= 1 && x <= 3 && y >= 1 && y <= 3)
                {
                    GameObject tilePrefab = ((x + y) % 2 == 0) ? whiteTilePrefab : blackTilePrefab;
                    if (tilePrefab != null)
                    {
                        Instantiate(tilePrefab, pos, Quaternion.identity, transform);
                    }
                    allowedPositions.Add(pos);
                }
                // Outer cells are not rendered as floor tiles.
            }
        }
        // Add boss door positions explicitly.
        // Left door at (0,2), right door at (4,2), and top door at (2,4)
        Vector2 leftDoorPos = gridPositions[0, 2];
        Vector2 rightDoorPos = gridPositions[gridColumns - 1, 2]; // index 4
        Vector2 topDoorPos = gridPositions[2, gridRows - 1];        // index 4

        if (!allowedPositions.Contains(leftDoorPos))
            allowedPositions.Add(leftDoorPos);
        if (!allowedPositions.Contains(rightDoorPos))
            allowedPositions.Add(rightDoorPos);
        if (!allowedPositions.Contains(topDoorPos))
            allowedPositions.Add(topDoorPos);
    }

    void SpawnBossDoors()
    {
        if (bossDoorPrefab == null)
        {
            Debug.LogError("BossDoorPrefab is not assigned in LobbyManager.");
            return;
        }
        // Spawn doors at the designated positions in the outer ring:
        Vector2 leftDoorPos = gridPositions[0, 2];         // left cell in middle row
        GameObject leftDoor = Instantiate(bossDoorPrefab, leftDoorPos, Quaternion.identity, transform);
        leftDoor.name = "BossDoor_Left";
        BossDoor leftDoorScript = leftDoor.GetComponent<BossDoor>();
        if (leftDoorScript != null)
            leftDoorScript.doorIdentifier = "Left";

        Vector2 rightDoorPos = gridPositions[gridColumns - 1, 2]; // (4,2)
        GameObject rightDoor = Instantiate(bossDoorPrefab, rightDoorPos, Quaternion.identity, transform);
        rightDoor.name = "BossDoor_Right";
        BossDoor rightDoorScript = rightDoor.GetComponent<BossDoor>();
        if (rightDoorScript != null)
            rightDoorScript.doorIdentifier = "Right";

        Vector2 topDoorPos = gridPositions[2, gridRows - 1];    // (2,4)
        GameObject topDoor = Instantiate(bossDoorPrefab, topDoorPos, Quaternion.identity, transform);
        topDoor.name = "BossDoor_Top";
        BossDoor topDoorScript = topDoor.GetComponent<BossDoor>();
        if (topDoorScript != null)
            topDoorScript.doorIdentifier = "Top";
    }

    void SpawnPlayer()
    {
        // Spawn the player at the center of the overall 5x5 grid: index (2,2)
        Vector2 spawnPos = gridPositions[2, 2];
        GameObject playerPrefab = GameSettings.SelectedHeroPrefab != null ? GameSettings.SelectedHeroPrefab : defaultPlayerPrefab;
        if (playerPrefab == null)
        {
            Debug.LogError("No player prefab assigned (neither in GameSettings nor default).");
            return;
        }
        GameObject player = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        player.name = "Player";

        // Pass along ability selections if the player has a PlayerAbilities component.
        PlayerAbilities pa = player.GetComponent<PlayerAbilities>();
        if (pa != null)
        {
            pa.SetupAbilities(GameSettings.SelectedAbilityPrefabs);
        }

        // Ensure the player has the LobbyPlayerController.
        if (player.GetComponent<LobbyPlayerController>() == null)
        {
            player.AddComponent<LobbyPlayerController>();
        }
    }
}
