using UnityEngine;

public class LobbyPlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector2 targetPosition;
    private bool isMoving = false;

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        HandleMovementInput();
        MoveTowardsTarget();
    }

    void HandleMovementInput()
    {
        if (isMoving)
            return;

        Vector2 input = Vector2.zero;
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            input = Vector2.up;
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            input = Vector2.down;
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            input = Vector2.left;
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            input = Vector2.right;

        if (input != Vector2.zero)
        {
            Vector2 candidate = targetPosition + input;
            bool allowed = false;
            // Check if candidate position is in allowedPositions (with a tolerance for float errors).
            foreach (Vector2 pos in LobbyManager.allowedPositions)
            {
                if (Vector2.Distance(candidate, pos) < 0.01f)
                {
                    allowed = true;
                    candidate = pos; // snap exactly
                    break;
                }
            }
            if (allowed && candidate != targetPosition)
            {
                targetPosition = candidate;
                isMoving = true;
            }
        }
    }

    void MoveTowardsTarget()
    {
        if (isMoving)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                isMoving = false;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BossDoor"))
        {
            BossDoor door = collision.GetComponent<BossDoor>();
            if (door != null)
            {
                door.TriggerDoor();
            }
        }
    }
}
