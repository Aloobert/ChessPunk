using UnityEngine;
using System.Collections;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class BubbleShieldAbility : SpecialAbility
{
    public GameObject shieldPrefab; // assign a shield visual prefab
    private GameObject activeShield;

    void Awake()
    {
        cooldownRounds = 5;
    }

    public override IEnumerator UseAbility(Vector3 direction)
    {
        if (!IsReady())
            yield break;
        // If no shield is active, create one.
        if (activeShield == null)
        {
            activeShield = Instantiate(shieldPrefab, transform.position, Quaternion.identity, transform);
            activeShield.transform.localPosition = Vector3.zero;
        }
        yield return null;
    }

    // Call this when the shield is broken by damage.
    public void BreakShield()
    {
        if (activeShield != null)
        {
            Destroy(activeShield);
            SetCooldown();
        }
    }
}
