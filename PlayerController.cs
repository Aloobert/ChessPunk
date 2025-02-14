using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Player Stats")]
    public int maxHP = 10;
    public int currentHP = 10;

    [Header("Basic Movement Settings (Fallback)")]
    public float moveTime = 0.2f; // time for 1-tile WASD move

    private Vector3 targetPosition;
    private bool isMoving = false;

    // Cache a reference to the hero movement script
    public HeroMovement heroMovement;

    // Reference to your overlay manager
    public MovementOverlayManager overlayManager;

    // Reference to the TurnManager
    public TurnManager turnManager;

    // Store occupantTile -> behindTile
    private Dictionary<Vector3, Vector3> cachedAttackBehindTile = new Dictionary<Vector3, Vector3>();

    void Start()
    {
        targetPosition = transform.position;
        currentHP = maxHP;

        if (turnManager == null)
        {
            GameObject tm = GameObject.FindGameObjectWithTag("TurnManager");
            if (tm != null)
            {
                turnManager = tm.GetComponent<TurnManager>();
            }
        }
        if (overlayManager == null)
        {
            overlayManager = FindObjectOfType<MovementOverlayManager>();
        }
        if (heroMovement == null)
        {
            heroMovement = GetComponent<HeroMovement>();
        }
    }

    void Update()
    {
        if (turnManager != null && turnManager.currentTurn == TurnManager.Turn.Player && !isMoving)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ShowMovementOverlay();
            }
            HandleBasicMovementInput();
        }
    }

    #region Overlay Logic

    void ShowMovementOverlay()
    {
        if (heroMovement == null) return;

        // If hero is on cooldown, skip
        if (!heroMovement.CanUseMovement())
        {
            Debug.Log("Hero movement on cooldown. No overlay shown.");
            return;
        }

        // Get the tile sets from hero
        MovementTileData mt = heroMovement.GetMovementTileData();

        // Display them
        overlayManager.ShowOverlays(mt.valid, mt.invalid, mt.ability);

        // Store occupant->behindTile dictionary
        cachedAttackBehindTile = mt.attackBehindTile;
    }

    // Called by MovementOverlayManager when a tile is clicked
    public void OnTileOverlayClicked(TileOverlayType type, Vector3 tilePos)
    {
        if (type == TileOverlayType.Valid)
        {
            StartCoroutine(MoveToPosition(tilePos));
        }
        else if (type == TileOverlayType.Ability)
        {
            // Check if tilePos is an occupant tile => stored in cachedAttackBehindTile
            if (cachedAttackBehindTile.ContainsKey(tilePos))
            {
                Vector3 behindTile = cachedAttackBehindTile[tilePos];
                // Move behind the enemy and deal damage
                StartCoroutine(MoveToPositionAttack(behindTile, tilePos));
            }
            else
            {
                // Normal ability tile => no occupant => move directly
                StartCoroutine(MoveToPosition(tilePos));
            }
        }
        else
        {
            Debug.Log("Cannot move to that tile (blocked).");
        }
    }

    #endregion

    #region Basic Movement

    void HandleBasicMovementInput()
    {
        Vector3 dir = Vector3.zero;
        if (Input.GetKeyDown(KeyCode.W)) dir = Vector3.up;
        else if (Input.GetKeyDown(KeyCode.S)) dir = Vector3.down;
        else if (Input.GetKeyDown(KeyCode.A)) dir = Vector3.left;
        else if (Input.GetKeyDown(KeyCode.D)) dir = Vector3.right;

        if (dir != Vector3.zero)
        {
            MoveBasic(dir);
        }
    }

    void MoveBasic(Vector3 direction)
    {
        // Clear overlays if open
        if (overlayManager != null)
        {
            overlayManager.ClearOverlays();
        }

        Vector3 newPosition = targetPosition + direction;

        // Check walkable
        GridManager gm = FindObjectOfType<GridManager>();
        if (gm != null)
        {
            Node node = gm.GetNodeFromPosition(newPosition);
            if (!node.Walkable)
            {
                Debug.Log("Out of bounds or pillar => blocked.");
                return;
            }
        }

        // occupant check
        Collider2D occupant = Physics2D.OverlapPoint(newPosition);
        if (occupant != null)
        {
            if (occupant.CompareTag("Boss"))
            {
                Debug.Log("Player tries to move onto Boss => deal 1 damage, revert, end turn.");
                BossController boss = occupant.GetComponent<BossController>();
                if (boss != null)
                {
                    boss.TakeDamage(1);
                }
                if (turnManager != null)
                {
                    turnManager.EndTurn();
                }
                return;
            }
            else if (occupant.CompareTag("Pillar"))
            {
                Debug.Log("Movement blocked by Pillar.");
                return;
            }
        }

        StartCoroutine(MoveToPosition(newPosition));
    }

    IEnumerator MoveToPosition(Vector3 newPosition)
    {
        isMoving = true;
        Vector3 startPos = transform.position;
        float elapsed = 0f;

        while (elapsed < moveTime)
        {
            transform.position = Vector3.Lerp(startPos, newPosition, elapsed / moveTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = newPosition;
        targetPosition = newPosition;
        isMoving = false;

        // occupant check after arrival
        Collider2D occupant = Physics2D.OverlapPoint(transform.position);
        if (occupant != null && occupant.CompareTag("Boss"))
        {
            Debug.Log("Player landed on Boss => deal 1 damage, revert, end turn.");
            BossController boss = occupant.GetComponent<BossController>();
            if (boss != null)
            {
                boss.TakeDamage(1);
            }
            // revert
            transform.position = startPos;
            targetPosition = startPos;

            if (turnManager != null)
            {
                turnManager.EndTurn();
            }
            yield break;
        }

        // End turn
        if (turnManager != null)
        {
            turnManager.EndTurn();
        }
    }

    #endregion

    // Move behind the occupant tile for an attack
    IEnumerator MoveToPositionAttack(Vector3 behindTile, Vector3 occupantTile)
    {
        isMoving = true;
        Vector3 startPos = transform.position;
        float elapsed = 0f;

        while (elapsed < moveTime)
        {
            transform.position = Vector3.Lerp(startPos, behindTile, elapsed / moveTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = behindTile;
        targetPosition = behindTile;
        isMoving = false;

        // occupantTile should be Boss => deal damage
        Collider2D occupant = Physics2D.OverlapPoint(occupantTile);
        if (occupant != null && occupant.CompareTag("Boss"))
        {
            BossController boss = occupant.GetComponent<BossController>();
            if (boss != null)
            {
                boss.TakeDamage(1);
                Debug.Log("Attacked Boss from behind tile!");
            }
        }

        // End turn
        if (turnManager != null)
        {
            turnManager.EndTurn();
        }
    }

    // Called by TurnManager to start player's turn
    public void StartTurn()
    {
        Debug.Log("Player's turn has started.");
    }

    // Player Takes Damage
    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        Debug.Log($"Player takes {amount} damage => HP now {currentHP}/{maxHP}");
        if (currentHP <= 0)
        {
            currentHP = 0;
            Debug.Log("Player died!");
            Destroy(gameObject);
        }
    }
}
