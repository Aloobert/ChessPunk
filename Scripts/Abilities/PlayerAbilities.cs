using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    [Header("Assigned Abilities")]
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
        AbilityInstance primaryInstance = primaryObj.GetComponent<AbilityInstance>();
        if (primaryInstance != null && primaryInstance.abilityData != null)
        {
            primaryAbility = primaryInstance.abilityData;
        }
        else
        {
            Debug.LogWarning("Primary ability prefab missing AbilityInstance or abilityData.");
        }

        // Secondary Ability (index 1)
        GameObject secondaryObj = Instantiate(abilityPrefabs[1]);
        AbilityInstance secondaryInstance = secondaryObj.GetComponent<AbilityInstance>();
        if (secondaryInstance != null && secondaryInstance.abilityData != null)
        {
            secondaryAbility = secondaryInstance.abilityData;
        }
        else
        {
            Debug.LogWarning("Secondary ability prefab missing AbilityInstance or abilityData.");
        }

        // Special Ability (index 2)
        GameObject specialObj = Instantiate(abilityPrefabs[2]);
        AbilityInstance specialInstance = specialObj.GetComponent<AbilityInstance>();
        if (specialInstance != null && specialInstance.abilityData != null)
        {
            specialAbility = specialInstance.abilityData;
        }
        else
        {
            Debug.LogWarning("Special ability prefab missing AbilityInstance or abilityData.");
        }

        // Passive Ability (index 3)
        GameObject passiveObj = Instantiate(abilityPrefabs[3]);
        AbilityInstance passiveInstance = passiveObj.GetComponent<AbilityInstance>();
        if (passiveInstance != null && passiveInstance.abilityData != null)
        {
            passiveAbility = passiveInstance.abilityData;
        }
        else
        {
            Debug.LogWarning("Passive ability prefab missing AbilityInstance or abilityData.");
        }
    }

}
