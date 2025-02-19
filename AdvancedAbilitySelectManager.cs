using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AdvancedAbilitySelectManager : MonoBehaviour
{
    public static AdvancedAbilitySelectManager Instance;

    [Header("UI References")]
    public Button confirmButton;

    [Header("Selection Border")]
    public RectTransform selectionBorder;
    public float borderMoveSpeed = 10f;

    // Private storage for the selected ability in each slot.
    private GameObject selectedPrimary;
    private GameObject selectedSecondary;
    private GameObject selectedSpecial;
    private GameObject selectedPassive;

    // The option currently being hovered over (for visual feedback).
    private AbilityOption currentHoverOption;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Disable the Confirm Button initially.
        if (confirmButton != null)
            confirmButton.interactable = false;
    }

    void Update()
    {
        // If an option is being hovered, smoothly move the selection border toward it.
        if (currentHoverOption != null && selectionBorder != null)
        {
            RectTransform optionRect = currentHoverOption.GetComponent<RectTransform>();
            if (optionRect != null)
            {
                selectionBorder.position = Vector3.Lerp(selectionBorder.position, optionRect.position, Time.deltaTime * borderMoveSpeed);
            }
        }
    }

    /// <summary>
    /// Called by an AbilityOption when the pointer hovers over it.
    /// </summary>
    public void SetHoverOption(AbilityOption option)
    {
        currentHoverOption = option;
        // Immediately move the border to this option's position.
        RectTransform optionRect = option.GetComponent<RectTransform>();
        if (optionRect != null && selectionBorder != null)
        {
            selectionBorder.position = optionRect.position;
        }
        Debug.Log("Hovering over option for slot: " + option.slotType);
    }

    /// <summary>
    /// Called when an ability option is clicked.
    /// </summary>
    public void SelectOption(AbilityOption option)
    {
        switch (option.slotType)
        {
            case AbilityOption.AbilitySlotType.Primary:
                selectedPrimary = option.abilityPrefab;
                Debug.Log("Primary ability selected: " + (selectedPrimary != null ? selectedPrimary.name : "null"));
                break;
            case AbilityOption.AbilitySlotType.Secondary:
                selectedSecondary = option.abilityPrefab;
                Debug.Log("Secondary ability selected: " + (selectedSecondary != null ? selectedSecondary.name : "null"));
                break;
            case AbilityOption.AbilitySlotType.Special:
                selectedSpecial = option.abilityPrefab;
                Debug.Log("Special ability selected: " + (selectedSpecial != null ? selectedSpecial.name : "null"));
                break;
            case AbilityOption.AbilitySlotType.Passive:
                selectedPassive = option.abilityPrefab;
                Debug.Log("Passive ability selected: " + (selectedPassive != null ? selectedPassive.name : "null"));
                break;
        }
        CheckIfAllSelected();
    }

    // Checks whether all four ability slots have a selection and enables the Confirm button.
    void CheckIfAllSelected()
    {
        if (selectedPrimary != null && selectedSecondary != null &&
            selectedSpecial != null && selectedPassive != null)
        {
            if (confirmButton != null)
                confirmButton.interactable = true;
            Debug.Log("All abilities selected. Confirm button enabled.");
        }
        else
        {
            if (confirmButton != null)
                confirmButton.interactable = false;
        }
    }

    // Called when the Confirm Button is clicked.
    public void ConfirmSelection()
    {
        if (confirmButton != null && confirmButton.interactable)
        {
            // Save the selected abilities in a consistent order:
            GameSettings.SelectedAbilityPrefabs[0] = selectedPrimary;
            GameSettings.SelectedAbilityPrefabs[1] = selectedSecondary;
            GameSettings.SelectedAbilityPrefabs[2] = selectedSpecial;
            GameSettings.SelectedAbilityPrefabs[3] = selectedPassive;

            // Log the selections from GameSettings.
            Debug.Log("Confirming selections:");
            Debug.Log("Primary: " + (GameSettings.SelectedAbilityPrefabs[0] != null ? GameSettings.SelectedAbilityPrefabs[0].name : "null"));
            Debug.Log("Secondary: " + (GameSettings.SelectedAbilityPrefabs[1] != null ? GameSettings.SelectedAbilityPrefabs[1].name : "null"));
            Debug.Log("Special: " + (GameSettings.SelectedAbilityPrefabs[2] != null ? GameSettings.SelectedAbilityPrefabs[2].name : "null"));
            Debug.Log("Passive: " + (GameSettings.SelectedAbilityPrefabs[3] != null ? GameSettings.SelectedAbilityPrefabs[3].name : "null"));

            // Transition to the Lobby scene.
            SceneManager.LoadScene("LobbyScene");
        }
        else
        {
            Debug.LogWarning("ConfirmSelection called but the confirm button is not interactable.");
        }
    }
}
