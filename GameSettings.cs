using UnityEngine;

public static class GameSettings
{
    // The hero prefab selected in the Hero Select scene.
    public static GameObject SelectedHeroPrefab;

    // The array of ability prefab references selected in the Ability Select scene.
    // (For example, one per slot; here we assume 4 ability slots.)
    public static GameObject[] SelectedAbilityPrefabs = new GameObject[4];
}
