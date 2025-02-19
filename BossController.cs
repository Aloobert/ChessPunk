using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossController : MonoBehaviour
{
    [Header("Boss Stats")]
    public int maxHP = 10;
    public int currentHP = 10;

    [Header("Movement Settings")]
    public float moveTime = 0.2f;
    private bool isMoving = false;

    private Pathfinding pathfinding;
    private GridManager grid;
    private List<Node> path = new List<Node>();
    private int pathIndex = 0;

    [Header("References")]
    public Transform playerTransform;
    private TurnManager turnManager;

    void Start()
    {
        currentHP = maxHP;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            Debug.Log("PlayerTransform assigned.");
        }

        GameObject tm = GameObject.FindGameObjectWithTag("TurnManager");
        if (tm != null)
        {
            turnManager = tm.GetComponent<TurnManager>();
            Debug.Log("TurnManager assigned.");
        }

        grid = FindObjectOfType<GridManager>();
        if (grid != null)
        {
            pathfinding = grid.GetComponent<Pathfinding>();
            if (pathfinding != null)
            {
                Debug.Log("Pathfinding assigned.");
            }
        }
    }

    public void StartTurn()
    {
        if (!isMoving)
        {
            Debug.Log("Boss's Turn");
            TakeTurn();
        }
    }

    void TakeTurn()
    {
        if (playerTransform == null || pathfinding == null)
        {
            Debug.Log("No player or no pathfinding => skip boss turn.");
            if (turnManager != null) turnManager.EndTurn();
            return;
        }

        Vector3 startPos = transform.position;
        Vector3 targetPos = playerTransform.position;

        path = pathfinding.FindPath(startPos, targetPos);
        if (path != null && path.Count > 0)
        {
            pathIndex = 0;
            StartCoroutine(MoveOneTile());
        }
        else
        {
            Debug.Log("No path => skip boss turn.");
            if (turnManager != null) turnManager.EndTurn();
        }
    }

    IEnumerator MoveOneTile()
    {
        isMoving = true;
        if (pathIndex < path.Count)
        {
            Node targetNode = path[pathIndex];
            Vector3 targetPos = targetNode.Position;

            Collider2D occupant = Physics2D.OverlapPoint(targetPos);
            if (occupant != null && occupant.CompareTag("Player"))
            {
                Debug.Log("Boss tries to move onto Player => deal damage, skip move.");
                PlayerController pc = occupant.GetComponent<PlayerController>();
                if (pc != null)
                {
                    pc.TakeDamage(1);
                }
            }
            else
            {
                yield return StartCoroutine(MoveToPosition(targetPos));
            }

            pathIndex++;
        }

        isMoving = false;
        if (turnManager != null)
        {
            turnManager.EndTurn();
        }
    }

    IEnumerator MoveToPosition(Vector3 newPosition)
    {
        Vector3 startPos = transform.position;
        float elapsed = 0f;
        while (elapsed < moveTime)
        {
            transform.position = Vector3.Lerp(startPos, newPosition, elapsed / moveTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = newPosition;

        // occupant check
        Collider2D occupant = Physics2D.OverlapPoint(transform.position);
        if (occupant != null && occupant.CompareTag("Player"))
        {
            Debug.Log("Boss collides with Player => deal damage, revert boss.");
            PlayerController pc = occupant.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.TakeDamage(1);
            }
            // revert
            transform.position = startPos;
        }
        yield break;
    }

    public void TakeDamage(int amt)
    {
        currentHP -= amt;
        Debug.Log($"Boss took {amt} damage => HP {currentHP}/{maxHP}");
        if (currentHP <= 0)
        {
            Debug.Log("Boss is dead!");
            Destroy(gameObject);
        }
    }
}
