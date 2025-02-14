using UnityEngine;
using UnityEngine.EventSystems;

public class AbilityOption : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    // Reference to the ability prefab this option represents.
    public GameObject abilityPrefab;

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Inform the AbilitySelectManager that this option is now selected.
        if (AbilitySelectManager.Instance != null)
        {
            AbilitySelectManager.Instance.SetSelectedOption(this);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // On click, immediately select this ability option.
        if (AbilitySelectManager.Instance != null)
        {
            AbilitySelectManager.Instance.SelectCurrentOption();
        }
    }
}
