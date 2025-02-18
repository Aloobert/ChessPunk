using UnityEngine;
using UnityEngine.EventSystems;

public class AbilityOption : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    // Enum to designate which slot this option belongs to.
    public enum AbilitySlotType { Primary, Secondary, Special, Passive }
    public AbilitySlotType slotType;

    // Reference to the ability prefab this option represents.
    public GameObject abilityPrefab;

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Inform the AdvancedAbilitySelectManager that this option is being hovered.
        if (AdvancedAbilitySelectManager.Instance != null)
        {
            AdvancedAbilitySelectManager.Instance.SetHoverOption(this);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // On click, select this ability option.
        if (AdvancedAbilitySelectManager.Instance != null)
        {
            AdvancedAbilitySelectManager.Instance.SelectOption(this);
        }
    }
}
