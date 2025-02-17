using UnityEngine;

public class HealthAura : PassiveAbility
{
    public int bonusHP = 1;

    void OnEnable()
    {
        PlayerHealth ph = GetComponent<PlayerHealth>();
        if (ph != null)
        {
            ph.maxHP += bonusHP;
            ph.currentHP += bonusHP;
        }
    }

    // Passive abilities may not use UseAbility; you can leave it empty.
    public override IEnumerator UseAbility(Vector3 direction)
    {
        yield break;
    }
}
