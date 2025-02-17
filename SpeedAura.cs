using UnityEngine;

public class SpeedAura : PassiveAbility
{
    public int cooldownReduction = 1;

    void OnEnable()
    {
        PlayerAbilities pa = GetComponent<PlayerAbilities>();
        if (pa != null)
        {
            pa.cooldownReduction += cooldownReduction;
        }
    }

    public override IEnumerator UseAbility(Vector3 direction)
    {
        yield break;
    }
}
