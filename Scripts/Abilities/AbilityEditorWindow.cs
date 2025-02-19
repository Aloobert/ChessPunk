using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class AbilityEditorWindow : EditorWindow
{
    // Toolbar tabs: Active vs Passive ability editor.
    private int tabIndex = 0;
    private string[] tabNames = new string[] { "Active Ability", "Passive Ability" };

    // Common field for ability name.
    private string abilityName = "New Ability";

    // --- Active Ability Fields ---
    private int damage = 1;
    private int cooldown = 3;
    private int projectileCount = 0;
    private float projectileDelay = 0f;
    private float knockbackDistance = 0f;
    private int knockbackDamage = 0;

    // Enum for projectile directions.
    public enum ProjectileDirection
    {
        Forward,
        Backwards,
        Left,
        Right,
        DiagonalLeftForward,
        DiagonalRightForward,
        DiagonalLeftBackwards,
        DiagonalRightBackwards
    }
    // A dynamic list to hold each projectile’s direction (stored as an int index into ProjectileDirection).
    private List<int> projectileDirections = new List<int>();

    // --- Passive Ability Fields ---
    public enum PassiveType { HealthAura, DamageAura, DefenseAura, SpeedAura }
    private int passiveTypeIndex = 0;
    private float passiveMagnitude = 1f;

    // Opens the window from the Tools menu.
    [MenuItem("Tools/Ability Editor")]
    public static void ShowWindow()
    {
        GetWindow<AbilityEditorWindow>("Ability Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Ability Editor", EditorStyles.boldLabel);
        tabIndex = GUILayout.Toolbar(tabIndex, tabNames);

        // Common ability name field.
        abilityName = EditorGUILayout.TextField("Ability Name", abilityName);

        // Draw either the Active or Passive fields based on the selected tab.
        if (tabIndex == 0)
        {
            DrawActiveAbilityFields();
        }
        else if (tabIndex == 1)
        {
            DrawPassiveAbilityFields();
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Save Ability"))
        {
            SaveAbility();
        }
    }

    /// <summary>
    /// Draws the fields used for configuring an active ability.
    /// </summary>
    private void DrawActiveAbilityFields()
    {
        GUILayout.Label("Active Ability Settings", EditorStyles.boldLabel);
        damage = EditorGUILayout.IntField("Damage", damage);
        cooldown = EditorGUILayout.IntField("Cooldown (Rounds)", cooldown);
        projectileCount = EditorGUILayout.IntField("Projectile Count", projectileCount);
        projectileDelay = EditorGUILayout.FloatField("Projectile Delay", projectileDelay);
        knockbackDistance = EditorGUILayout.FloatField("Knockback Distance", knockbackDistance);
        knockbackDamage = EditorGUILayout.IntField("Knockback Damage", knockbackDamage);

        // Ensure the projectileDirections list matches the projectile count.
        while (projectileDirections.Count < projectileCount)
            projectileDirections.Add(0);
        while (projectileDirections.Count > projectileCount)
            projectileDirections.RemoveAt(projectileDirections.Count - 1);

        // For each projectile, display a dropdown to select its direction.
        if (projectileCount > 0)
        {
            GUILayout.Label("Projectile Directions", EditorStyles.boldLabel);
            for (int i = 0; i < projectileCount; i++)
            {
                projectileDirections[i] = EditorGUILayout.Popup(
                    "Projectile " + (i + 1) + " Direction",
                    projectileDirections[i],
                    System.Enum.GetNames(typeof(ProjectileDirection))
                );
            }
        }
    }

    /// <summary>
    /// Draws the fields used for configuring a passive ability.
    /// </summary>
    private void DrawPassiveAbilityFields()
    {
        GUILayout.Label("Passive Ability Settings", EditorStyles.boldLabel);
        passiveTypeIndex = EditorGUILayout.Popup("Passive Type", passiveTypeIndex, System.Enum.GetNames(typeof(PassiveType)));
        passiveMagnitude = EditorGUILayout.FloatField("Effect Magnitude", passiveMagnitude);
    }

    /// <summary>
    /// Saves the ability data as a new ScriptableObject asset.
    /// </summary>
    private void SaveAbility()
    {
        // Create an instance of the AbilityData ScriptableObject.
        AbilityData ability = ScriptableObject.CreateInstance<AbilityData>();
        ability.abilityName = abilityName;

        if (tabIndex == 0) // Active ability settings.
        {
            ability.isPassive = false;
            ability.damage = damage;
            ability.cooldown = cooldown;
            ability.projectileCount = projectileCount;
            ability.projectileDelay = projectileDelay;
            ability.knockbackDistance = knockbackDistance;
            ability.knockbackDamage = knockbackDamage;
            ability.projectileDirections = projectileDirections.ToArray();
        }
        else if (tabIndex == 1) // Passive ability settings.
        {
            ability.isPassive = true;
            ability.passiveType = (AbilityData.PassiveType)passiveTypeIndex;
            ability.passiveMagnitude = passiveMagnitude;
        }

        // Create (or ensure the existence of) the Abilities folder and save the asset.
        string assetPath = "Assets/Abilities";
        if (!AssetDatabase.IsValidFolder(assetPath))
        {
            AssetDatabase.CreateFolder("Assets", "Abilities");
        }
        string assetName = abilityName.Replace(" ", "") + ".asset";
        string fullPath = System.IO.Path.Combine(assetPath, assetName);
        AssetDatabase.CreateAsset(ability, fullPath);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = ability;
        Debug.Log("Saved Ability: " + abilityName + " at " + fullPath);
    }
}
