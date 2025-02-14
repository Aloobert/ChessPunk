using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class AbilitySelectManager : MonoBehaviour
{
    public static AbilitySelectManager Instance; // Singleton for easy access.

    [Header("Selection Border")]
    public RectTransform selectionBorder; // The UI element that highlights the selection.
    public float borderMoveSpeed = 10f;

    [Header("Ability Grid Settings")]
    public int columns = 4; // Number of columns in your grid.

    // List to store all ability options in the scene.
    private List<AbilityOption> abilityOptions = new List<AbilityOption>();
    private int currentIndex = 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Find all AbilityOption components that are children of this manager's container.
        AbilityOption[] options = GetComponentsInChildren<AbilityOption>();
        abilityOptions.AddRange(options);
        if (abilityOptions.Count > 0)
        {
            SetSelectedOption(abilityOptions[0]);
        }
        else
        {
            Debug.LogWarning("No AbilityOption found in the scene.");
        }
    }

    void Update()
    {
        HandleKeyboardInput();

        // Confirm selection with Space.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SelectCurrentOption();
        }

        // Smoothly move the selection border to the selected option.
        if (abilityOptions.Count > 0 && selectionBorder != null)
        {
            RectTransform selectedRect = abilityOptions[currentIndex].GetComponent<RectTransform>();
            if (selectedRect != null)
            {
                selectionBorder.position = Vector3.Lerp(selectionBorder.position, selectedRect.position, Time.deltaTime * borderMoveSpeed);
            }
        }
    }

    void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentIndex = (currentIndex + 1) % abilityOptions.Count;
            SetSelectedOption(abilityOptions[currentIndex]);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentIndex = (currentIndex - 1 + abilityOptions.Count) % abilityOptions.Count;
            SetSelectedOption(abilityOptions[currentIndex]);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentIndex = Mathf.Max(currentIndex - columns, 0);
            SetSelectedOption(abilityOptions[currentIndex]);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentIndex = Mathf.Min(currentIndex + columns, abilityOptions.Count - 1);
            SetSelectedOption(abilityOptions[currentIndex]);
        }
    }

    // Called when the selection changes.
    public void SetSelectedOption(AbilityOption option)
    {
        currentIndex = abilityOptions.IndexOf(option);
        RectTransform optionRect = option.GetComponent<RectTransform>();
        if (optionRect != null && selectionBorder != null)
        {
            selectionBorder.position = optionRect.position;
        }
    }

    // Called when selection is confirmed.
    public void SelectCurrentOption()
    {
        if (abilityOptions.Count > 0)
        {
            AbilityOption selectedOption = abilityOptions[currentIndex];
            if (selectedOption != null && selectedOption.abilityPrefab != null)
            {
                // For testing, we can store the selected ability into a particular slot.
                // For simplicity, here we store it in slot 0.
                GameSettings.SelectedAbilityPrefabs[0] = selectedOption.abilityPrefab;
                Debug.Log("Selected ability: " + selectedOption.abilityPrefab.name);

                // Transition to the next scene (e.g., Lobby)
                SceneManager.LoadScene("LobbyScene");
            }
            else
            {
                Debug.LogWarning("No ability prefab assigned to the selected option.");
            }
        }
    }
}
