using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BossDoorTrigger : MonoBehaviour
{
    public string bossSceneName = "BossScene";
    // Set the delay (in seconds) before transitioning to the boss scene.
    public float transitionDelay = 0.5f;

    private bool transitioning = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object colliding is the player and ensure we don't start multiple transitions.
        if (other.CompareTag("Player") && !transitioning)
        {
            transitioning = true;
            Debug.Log("Boss door triggered by: " + other.name + ". Transition will occur in " + transitionDelay + " seconds.");
            // Optionally, you might disable the player's input here to prevent further movement.
            StartCoroutine(DelayedSceneTransition());
        }
    }

    IEnumerator DelayedSceneTransition()
    {
        // Wait for the specified delay.
        yield return new WaitForSeconds(transitionDelay);
        // Now load the boss scene.
        SceneManager.LoadScene(bossSceneName);
    }
}
