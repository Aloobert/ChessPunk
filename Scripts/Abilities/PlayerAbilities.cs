using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    [Header("Assigned Abilities")]
    // These fields store the active ability instances for each slot.
    public AbilityBase primaryAbility;
    public AbilityBase secondaryAbility;
    public AbilityBase specialAbility;
    public AbilityBase passiveAbility;

    void Update()
    {
        // For testing purposes: press "F" to use the primary ability.
        if (Input.GetKeyDown(KeyCode.F))
        {
            UsePrimaryAbility();
        }
    }

    /// <summary>
    /// Activates the primary ability.
    /// </summary>
    public void UsePrimaryAbility()
    {
        if (primaryAbility != null)
        {
            primaryAbility.ActivateAbility(gameObject);
        }
        else
        {
            Debug.LogWarning("Primary ability not set!");
        }
    }

    /// <summary>
    /// Call this method at the end of the turn to reduce ability cooldowns.
    /// </summary>
    public void EndTurn()
    {
        if (primaryAbility != null) primaryAbility.EndTurn();
        if (secondaryAbility != null) secondaryAbility.EndTurn();
        if (specialAbility != null) specialAbility.EndTurn();
        if (passiveAbility != null) passiveAbility.EndTurn();
    }

    /// <summary>
    /// Sets up the player's abilities using the provided ability prefab array.
    /// Expects 4 prefabs: [0]=Primary, [1]=Secondary, [2]=Special, [3]=Passive.
    /// </summary>
    /// <param name="abilityPrefabs">An array of GameObjects that contain an AbilityBase component.</param>
    public void SetupAbilities(GameObject[] abilityPrefabs)
    {
        if (abilityPrefabs == null || abilityPrefabs.Length < 4)
        {
            Debug.LogWarning("Insufficient ability prefabs provided for SetupAbilities.");
            return;
        }

        // Primary Ability (index 0)
        GameObject primaryObj = Instantiate(abilityPrefabs[0]);
        AbilityBase primary = primaryObj.GetComponent<AbilityBase>();
        if (primary != null)
        {
            primaryAbility = primary;
        }
        else
        {
            Debug.LogWarning("Primary ability prefab missing AbilityBase component.");
        }

        // Secondary Ability (index 1)
        GameObject secondaryObj = Instantiate(abilityPrefabs[1]);
        AbilityBase secondary = secondaryObj.GetComponent<AbilityBase>();
        if (secondary != null)
        {
            secondaryAbility = secondary;
        }
        else
        {
            Debug.LogWarning("Secondary ability prefab missing AbilityBase component.");
        }

        // Special Ability (index 2)
        GameObject specialObj = Instantiate(abilityPrefabs[2]);
        AbilityBase special = specialObj.GetComponent<AbilityBase>();
        if (special != null)
        {
            specialAbility = special;
        }
        else
        {
            Debug.LogWarning("Special ability prefab missing AbilityBase component.");
        }

        // Passive Ability (index 3)
        GameObject passiveObj = Instantiate(abilityPrefabs[3]);
        AbilityBase passive = passiveObj.GetComponent<AbilityBase>();
        if (passive != null)
        {
            passiveAbility = passive;
        }
        else
        {
            Debug.LogWarning("Passive ability prefab missing AbilityBase component.");
        }
    }
}
