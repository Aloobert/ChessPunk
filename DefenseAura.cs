using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class DefenseAura : PassiveAbility
{
    public GameObject shieldPrefab;

    void OnEnable()
    {
        // Automatically grant a bubble shield at the start of the fight.
        GameObject shield = Instantiate(shieldPrefab, transform.position, Quaternion.identity, transform);
        shield.transform.localPosition = Vector3.zero;
    }

    public override IEnumerator UseAbility(Vector3 direction)
    {
        yield break;
    }
}
