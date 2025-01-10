using UnityEngine;
using UnityEngine.UI;
using TMPro; // Import TextMeshPro namespace

public class LevelSelectionController : MonoBehaviour {
    public Button[] levelButtons; // Assign level buttons in the Inspector
    public Button resetButton; // Assign the Reset button in the Inspector

    private void Start() {
        // Initialize level buttons based on PlayerPrefs
        InitializeLevelButtons();

        // Add listener to reset progress
        resetButton.onClick.AddListener(ResetProgress);
    }

    private void InitializeLevelButtons() {
        int levelReached = PlayerPrefs.GetInt("LevelReached", 1);

        for (int i = 0; i < levelButtons.Length; i++) {
            TextMeshProUGUI buttonText = levelButtons[i].GetComponentInChildren<TextMeshProUGUI>();

            if (i + 1 <= levelReached) {
                levelButtons[i].interactable = true;

                // Assign functionality to load the correct level
                int levelIndex = i + 1;
                levelButtons[i].onClick.AddListener(() => LoadLevel("Level" + levelIndex));

                if (buttonText != null) {
                    buttonText.text = $"Level {i + 1}";
                }
            } else {
                levelButtons[i].interactable = false;

                if (buttonText != null) {
                    buttonText.text = "Locked";
                }
            }
        }
    }

    public void ResetProgress() {
        PlayerPrefs.SetInt("LevelReached", 1); // Reset to the first level
        PlayerPrefs.Save(); // Save changes

        // Reinitialize level buttons to reflect reset progress
        foreach (Button button in levelButtons) {
            button.onClick.RemoveAllListeners(); // Clear old listeners
        }
        InitializeLevelButtons();

        Debug.Log("Progress has been reset. Only the first level is unlocked.");
    }

    public void LoadLevel(string levelName) {
        Debug.Log("Loading level: " + levelName);
        GameManager.Instance.LoadLevel(levelName);
    }
}