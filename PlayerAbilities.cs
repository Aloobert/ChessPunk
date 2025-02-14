using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    // An array to hold references to the player's ability prefabs.
    // (You can later use these references to instantiate or enable ability behavior.)
    public GameObject[] primaryAbilityPrefabs = new GameObject[4];

    /// <summary>
    /// Sets up the player's abilities from the selected ability prefabs.
    /// This should be called immediately after the player is instantiated.
    /// </summary>
    /// <param name="selectedAbilities">An array of ability prefab references.</param>
    public void SetupAbilities(GameObject[] selectedAbilities)
    {
        primaryAbilityPrefabs = selectedAbilities;
        Debug.Log("PlayerAbilities: Abilities set up.");

        // (Optional) If you want to instantiate ability components onto the player,
        // you can loop through the array and add them as components.
        // For example:
        // for (int i = 0; i < selectedAbilities.Length; i++)
        // {
        //     if (selectedAbilities[i] != null)
        //     {
        //         // Instantiate as a child or add a component.
        //         // GameObject abilityInstance = Instantiate(selectedAbilities[i], transform);
        //     }
        // }
    }

    // You can later add methods to trigger abilities via input, display cooldowns, etc.
}
