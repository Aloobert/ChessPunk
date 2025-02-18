using UnityEngine;
using UnityEngine.EventSystems;


public class HeroOption : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [Header("Hero Option Settings")]
    public GameObject heroPrefab;  // Assign the corresponding hero prefab in the Inspector

    // When the mouse enters this option, inform the HeroSelectMenu
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (HeroSelectMenu.Instance != null)
        {
            HeroSelectMenu.Instance.SetSelectedOption(this);
        }
    }

    // When clicked, select this hero
    public void OnPointerClick(PointerEventData eventData)
    {
        if (HeroSelectMenu.Instance != null)
        {
            HeroSelectMenu.Instance.SelectCurrentOption();
        }
    }
}
