using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionController : MonoBehaviour {
    public Button[] levelButtons; // Assign level buttons in the Inspector

    private void Start() {
        int levelReached = PlayerPrefs.GetInt("LevelReached", 1);

        for (int i = 0; i < levelButtons.Length; i++) {
            if (i + 1 <= levelReached) {
                levelButtons[i].interactable = true;

                // Add listener dynamically to load the correct level
                int levelIndex = i + 1; // Capture the level index for closure
                levelButtons[i].onClick.AddListener(() => LoadLevel("Level " + levelIndex));
            } else {
                levelButtons[i].interactable = false;
                levelButtons[i].GetComponentInChildren<Text>().text = "Locked";
            }
        }
    }

    public void LoadLevel(string levelName) {
        Debug.Log("Loading level: " + levelName);
        GameManager.Instance.LoadLevel(levelName);
    }
}