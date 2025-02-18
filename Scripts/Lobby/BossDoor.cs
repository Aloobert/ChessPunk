using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BossDoor : MonoBehaviour
{
    [Header("Door Settings")]
    public string BossScene = "BossScene";  // Set to your boss arena scene name.
    public string doorIdentifier;  // e.g. "Left", "Right", "Top"
    public float triggerDelay = 0.3f;  // Delay (in seconds) before loading the scene

    void Start()
    {
        // Ensure this object is tagged for triggers
        gameObject.tag = "BossDoor";
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
        else
            Debug.LogWarning("BossDoor does not have a Collider2D component.");
    }

    public void TriggerDoor()
    {
        Debug.Log("Boss Door triggered: " + doorIdentifier + ". Transition in " + triggerDelay + " seconds.");
        StartCoroutine(WaitAndLoad());
    }

    IEnumerator WaitAndLoad()
    {
        yield return new WaitForSeconds(triggerDelay);
        SceneManager.LoadScene(BossScene);
    }

    // Allow clicking on the door to also trigger it.
    void OnMouseDown()
    {
        TriggerDoor();
    }
}
