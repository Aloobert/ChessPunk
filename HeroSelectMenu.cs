using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class HeroSelectMenu : MonoBehaviour
{
    public static HeroSelectMenu Instance;  // Singleton instance for easy access

    [Header("Selection Border")]
    public RectTransform selectionBorder;  // UI element (Image) that highlights the current option
    public float borderMoveSpeed = 10f;      // Speed for the border to smoothly move

    [Header("Grid Layout Settings")]
    public int columns = 3;  // Number of columns in your hero grid

    private List<HeroOption> heroOptions = new List<HeroOption>();
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
        // Find all HeroOption components that are children of this menu
        HeroOption[] options = GetComponentsInChildren<HeroOption>();
        heroOptions.AddRange(options);

        if (heroOptions.Count > 0)
        {
            SetSelectedOption(heroOptions[0]);
        }
        else
        {
            Debug.LogWarning("No HeroOptions found in the HeroSelectMenu.");
        }
    }

    void Update()
    {
        HandleKeyboardInput();

        // Space bar to confirm selection
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SelectCurrentOption();
        }

        // Smoothly move the selection border toward the selected option's position
        if (heroOptions.Count > 0 && selectionBorder != null)
        {
            RectTransform selectedRect = heroOptions[currentIndex].GetComponent<RectTransform>();
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
            currentIndex = (currentIndex + 1) % heroOptions.Count;
            SetSelectedOption(heroOptions[currentIndex]);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentIndex = (currentIndex - 1 + heroOptions.Count) % heroOptions.Count;
            SetSelectedOption(heroOptions[currentIndex]);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentIndex = Mathf.Max(currentIndex - columns, 0);
            SetSelectedOption(heroOptions[currentIndex]);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentIndex = Mathf.Min(currentIndex + columns, heroOptions.Count - 1);
            SetSelectedOption(heroOptions[currentIndex]);
        }
    }

    // Sets the currently selected hero option (by keyboard or mouse)
    public void SetSelectedOption(HeroOption option)
    {
        currentIndex = heroOptions.IndexOf(option);
        // Immediately move the selection border to the option's position
        RectTransform optionRect = option.GetComponent<RectTransform>();
        if (optionRect != null && selectionBorder != null)
        {
            selectionBorder.position = optionRect.position;
        }
    }

    // Called when the player confirms selection (Space or mouse click)
    public void SelectCurrentOption()
    {
        if (heroOptions.Count > 0)
        {
            HeroOption selectedOption = heroOptions[currentIndex];
            if (selectedOption != null && selectedOption.heroPrefab != null)
            {
                // Store the selected hero prefab in GameSettings
                GameSettings.SelectedHeroPrefab = selectedOption.heroPrefab;
                Debug.Log("Selected hero: " + selectedOption.heroPrefab.name);

                // Transition to the next scene (e.g., Ability Selector or directly to the BossScene)
                // For now, we'll load a scene named "AbilitySelectScene" (adjust as needed)
                SceneManager.LoadScene("AbilitySelectScene");
            }
            else
            {
                Debug.LogWarning("No hero prefab assigned for the selected option.");
            }
        }
    }
}
