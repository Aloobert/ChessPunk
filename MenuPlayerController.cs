using UnityEngine;
using System.Collections;

public class MenuPlayerController : MonoBehaviour
{
    public float moveTime = 0.2f;  // Time to move one tile
    private Vector3 targetPosition;
    private bool isMoving = false;

    // Reference to the active MenuButtonTrigger (if any)
    private MenuButtonTrigger currentButtonTrigger;

    // Reference to the MenuManager
    private MenuManager menuManager;

    void Start()
    {
        targetPosition = transform.position;
        menuManager = FindObjectOfType<MenuManager>();
        if (menuManager == null)
        {
            Debug.LogError("MenuManager not found in the scene!");
        }
    }

    void Update()
    {
        if (!isMoving)
        {
            HandleMovementInput();

            // When the player presses Space, check if they're standing on a button trigger
            if (Input.GetKeyDown(KeyCode.Space) && currentButtonTrigger != null)
            {
                SelectButton(currentButtonTrigger.buttonType);
            }
        }
    }

    // WASD movement (basic movement without turn-based logic)
    void HandleMovementInput()
    {
        Vector3 direction = Vector3.zero;
        if (Input.GetKeyDown(KeyCode.W)) direction = Vector3.up;
        else if (Input.GetKeyDown(KeyCode.S)) direction = Vector3.down;
        else if (Input.GetKeyDown(KeyCode.A)) direction = Vector3.left;
        else if (Input.GetKeyDown(KeyCode.D)) direction = Vector3.right;

        if (direction != Vector3.zero)
        {
            Vector3 newPos = targetPosition + direction;

            // Optional: if you have a MenuGridManager that marks boundaries, you can check if the newPos is walkable
            Collider2D hit = Physics2D.OverlapPoint(newPos);
            if (hit != null && hit.CompareTag("Pillar"))
            {
                Debug.Log("Movement blocked by a pillar.");
                return;
            }

            StartCoroutine(MoveToPosition(newPos));
        }
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
    }

    // Called when a button is selected (by stepping on its trigger and pressing Space)
    private void SelectButton(MenuButtonType buttonType)
    {
        Debug.Log("Menu button selected: " + buttonType);
        switch (buttonType)
        {
            case MenuButtonType.NewGame:
                menuManager.StartNewGame();
                break;
            case MenuButtonType.Continue:
                menuManager.ContinueGame();
                break;
            case MenuButtonType.Quit:
                menuManager.QuitGame();
                break;
        }
    }

    // Use OnTriggerEnter2D and OnTriggerExit2D to detect when the player is over a button trigger.
    void OnTriggerEnter2D(Collider2D other)
    {
        MenuButtonTrigger mbt = other.GetComponent<MenuButtonTrigger>();
        if (mbt != null)
        {
            currentButtonTrigger = mbt;
            // (Optional) Visual feedback can be added here, e.g., highlight the UI button.
            Debug.Log("Player has entered " + mbt.buttonType + " trigger.");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        MenuButtonTrigger mbt = other.GetComponent<MenuButtonTrigger>();
        if (mbt != null && currentButtonTrigger == mbt)
        {
            currentButtonTrigger = null;
            // (Optional) Remove visual feedback.
            Debug.Log("Player has left " + mbt.buttonType + " trigger.");
        }
    }
}
