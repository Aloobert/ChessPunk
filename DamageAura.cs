using UnityEngine;

public class DamageAura : PassiveAbility
{
    public int bonusDamage = 1;

    void OnEnable()
    {
        PlayerAbilities pa = GetComponent<PlayerAbilities>();
        if (pa != null)
        {
            pa.damageBonus += bonusDamage;
        }
    }

    public override IEnumerator UseAbility(Vector3 direction)
    {
        yield break;
    }
}
