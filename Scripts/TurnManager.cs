using UnityEngine;
using System.Collections;

public class TurnManager : MonoBehaviour
{
    public enum Turn { Player, Boss }
    public Turn currentTurn = Turn.Player;

    [Header("Turn Settings")]
    public float turnDelay = 1.0f; // Delay before next turn

    public PlayerController playerController;
    public BossController bossController;

    void Start()
    {
        // Find PlayerController if not assigned
        if (playerController == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerController = player.GetComponent<PlayerController>();
            }
        }

        // Find BossController if not assigned
        if (bossController == null)
        {
            GameObject boss = GameObject.FindGameObjectWithTag("Boss");
            if (boss != null)
            {
                bossController = boss.GetComponent<BossController>();
            }
        }

        StartTurn();
    }

    public void StartTurn()
    {
        // Check if Player or Boss is destroyed
        if (playerController == null)
        {
            Debug.Log("No Player => game over scenario or player died.");
            GameOver();
            return;
        }
        if (bossController == null)
        {
            Debug.Log("No Boss => boss destroyed => player victory.");
            Victory();
            return;
        }

        switch (currentTurn)
        {
            case Turn.Player:
                Debug.Log("Player's Turn");
                if (playerController != null)
                {
                    playerController.StartTurn();
                }
                break;

            case Turn.Boss:
                Debug.Log("Boss's Turn");
                if (bossController != null)
                {
                    bossController.StartTurn();
                }
                break;
        }
    }

    public void EndTurn()
    {
        // Switch turn
        switch (currentTurn)
        {
            case Turn.Player:
                currentTurn = Turn.Boss;
                break;
            case Turn.Boss:
                currentTurn = Turn.Player;
                break;
        }

        // Wait turnDelay before starting next turn
        StartCoroutine(DelayedStartTurn());
    }

    private IEnumerator DelayedStartTurn()
    {
        yield return new WaitForSeconds(turnDelay);
        StartTurn();
    }

    private void GameOver()
    {
        Debug.Log("Game Over! The player has died.");
        // Possibly load game over scene or UI
    }

    private void Victory()
    {
        Debug.Log("Victory! The boss is defeated.");
        // Possibly load victory scene or UI
    }
}
