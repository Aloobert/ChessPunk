#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class AbilityEditorWindow : EditorWindow
{
    private string abilityName = "New Ability";
    private AbilityClass abilityClass = AbilityClass.Primary;
    private DamageType damageType = DamageType.Physical;
    private int damage = 10;
    private float cooldown = 1.0f;
    private float range = 5f;
    private bool isMelee = false;
    private GameObject projectilePrefab;
    private int projectileCount = 1;
    private float spreadAngle = 0f;
    private AnimationClip abilityAnimation;
    private Sprite icon;

    [MenuItem("Tools/Ability Editor")]
    public static void ShowWindow()
    {
        GetWindow<AbilityEditorWindow>("Ability Editor");
    }

    void OnGUI()
    {
        GUILayout.Label("Create a New Ability", EditorStyles.boldLabel);

        abilityName = EditorGUILayout.TextField("Ability Name", abilityName);
        abilityClass = (AbilityClass)EditorGUILayout.EnumPopup("Ability Class", abilityClass);
        damageType = (DamageType)EditorGUILayout.EnumPopup("Damage Type", damageType);
        icon = (Sprite)EditorGUILayout.ObjectField("Icon", icon, typeof(Sprite), false);

        GUILayout.Space(10);
        GUILayout.Label("Combat Settings", EditorStyles.boldLabel);
        damage = EditorGUILayout.IntField("Damage", damage);
        cooldown = EditorGUILayout.FloatField("Cooldown", cooldown);
        range = EditorGUILayout.FloatField("Range", range);

        GUILayout.Space(10);
        GUILayout.Label("Projectile / Melee Settings", EditorStyles.boldLabel);
        isMelee = EditorGUILayout.Toggle("Is Melee", isMelee);
        if (!isMelee)
        {
            projectilePrefab = (GameObject)EditorGUILayout.ObjectField("Projectile Prefab", projectilePrefab, typeof(GameObject), false);
            projectileCount = EditorGUILayout.IntField("Projectile Count", projectileCount);
            spreadAngle = EditorGUILayout.FloatField("Spread Angle", spreadAngle);
        }
        abilityAnimation = (AnimationClip)EditorGUILayout.ObjectField("Ability Animation", abilityAnimation, typeof(AnimationClip), false);

        GUILayout.Space(10);
        if (GUILayout.Button("Create Ability Asset"))
        {
            CreateAbilityAsset();
        }
    }

    void CreateAbilityAsset()
    {
        // Decide which base type to create based on Ability Class.
        // For simplicity, we assume active abilities use the ShotgunAbility type (you can later expand to a selection of types).
        AbilityBase newAbility = ScriptableObject.CreateInstance<ShotgunAbility>();

        newAbility.abilityName = abilityName;
        newAbility.abilityClass = abilityClass;
        newAbility.damageType = damageType;
        newAbility.icon = icon;
        newAbility.damage = damage;
        newAbility.cooldown = cooldown;
        newAbility.range = range;
        newAbility.isMelee = isMelee;
        newAbility.projectilePrefab = projectilePrefab;
        newAbility.projectileCount = projectileCount;
        newAbility.spreadAngle = spreadAngle;
        newAbility.abilityAnimation = abilityAnimation;

        // Save the asset to a folder "Assets/Abilities"
        string assetPath = "Assets/Abilities";
        if (!AssetDatabase.IsValidFolder(assetPath))
        {
            AssetDatabase.CreateFolder("Assets", "Abilities");
        }
        string assetName = assetPath + "/" + abilityName + ".asset";
        AssetDatabase.CreateAsset(newAbility, assetName);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newAbility;

        Debug.Log($"Created new ability asset: {abilityName} at {assetName}");
    }
}
#endif
