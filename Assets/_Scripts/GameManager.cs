using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public enum State {
        Playing,
        Lose,
        Win,
        Paused
    }

    [SerializeField] private GameObject loseCanvasPrefab; // Assign your prefab here
    [SerializeField] private GameObject winCanvasPrefab;
    [SerializeField] private GameObject pauseCanvasPrefab; // New prefab for pause UI
    private GameObject loseCanvasInstance;
    private GameObject winCanvasInstance;
    private GameObject pauseCanvasInstance; // Instance for pause UI
    public static GameManager Instance { get; private set; }
    public State CurrentState { get; private set; }
    private bool isPaused = false;

    public void TogglePause() {
        if (isPaused) {
            ResumeGame();
        } else {
            PauseGame();
        }
    }

    private void PauseGame() {
        isPaused = true;
        Time.timeScale = 0f; // Pause the game
        SetState(State.Paused); // Set state to Paused

        // Show the pause menu (if it's not already shown)
        if (pauseCanvasInstance == null) {
            pauseCanvasInstance = Instantiate(pauseCanvasPrefab);
        }
        // Enable the pause menu
        pauseCanvasInstance.SetActive(true);
    }

    private void ResumeGame() {
        isPaused = false;
        Time.timeScale = 1f; // Resume the game
        SetState(State.Playing); // Set state to Playing

        // Hide the pause menu
        if (pauseCanvasInstance != null) {
            pauseCanvasInstance.SetActive(false);
        }
    }

    private void Awake() {
        // Ensure there's only one instance of GameManager
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start() {
        SetState(State.Playing);
    }

    private void Update() {
        // Key controls for testing
        if (Input.GetKeyDown(KeyCode.B)) {
            SetState(State.Lose);
        } else if (Input.GetKeyDown(KeyCode.N)) {
            SetState(State.Win);
        } else if (Input.GetKeyDown(KeyCode.P)) {
            TogglePause(); // Toggle pause state when 'P' is pressed
        }
    }

    public void SetState(State state) {
        CurrentState = state;

        switch (state) {
            case State.Playing:
                Debug.Log("Game is now playing.");
                break;

            case State.Lose:
                Debug.Log("Game Over! You lose.");
                ShowLoseUI();
                break;

            case State.Win:
                Debug.Log("Congratulations! You win.");
                ShowWinUI();
                UnlockNextLevel();
                break;

            case State.Paused:
                Debug.Log("Game is paused.");

                break;
        }
    }

    private void ShowLoseUI() {
        if (loseCanvasInstance == null) {
            loseCanvasInstance = Instantiate(loseCanvasPrefab); // Spawn the prefab
        } else {
            Debug.LogWarning("Lose UI is already displayed!");
        }
    }

    private void ShowWinUI() {
        if (winCanvasInstance == null) {
            winCanvasInstance = Instantiate(winCanvasPrefab); // Spawn the prefab
        } else {
            Debug.LogWarning("Win UI is already displayed!");
        }
    }

    public void LoadLevel(string levelName) {
        Debug.Log("Loading level: " + levelName);
        SceneManager.LoadScene(levelName);
        SetState(State.Playing);
    }

    public void RestartLevel() {
        LoadLevel(SceneManager.GetActiveScene().name);
    }

    public void OpenMenu() {
        LoadLevel("MainMenu");
    }

    public void LoadNextLevel() {
        // Get the current scene index from the build settings
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextLevelIndex = currentSceneIndex + 1;

        string nextLevelSceneName = "Level" + nextLevelIndex;  // Assuming your scenes are named as "Level1", "Level2", ...

        // Check if the next level exists
        if (IsSceneInBuild(nextLevelSceneName)) {
            // If scene exists, load it
            SceneManager.LoadScene(nextLevelSceneName);
            SetState(State.Playing);
        } else {
            // If no scene exists, notify and disable the "Next" button
            Debug.Log("No more levels to load.");
            Button nextButton = winCanvasPrefab.transform.Find("NextButton")?.GetComponent<Button>();

            if (nextButton != null) {
                nextButton.interactable = false;  // Disable the button
            } else {
                Debug.LogError("NextButton not found!");
            }
        }
    }

    // Check if a scene is available in the build settings by name
    private bool IsSceneInBuild(string sceneName) {
        int totalScenesInBuild = SceneManager.sceneCountInBuildSettings;

        // Loop through the scenes in build settings and check if the scene exists
        for (int i = 0; i < totalScenesInBuild; i++) {
            if (SceneUtility.GetScenePathByBuildIndex(i).Contains(sceneName)) {
                return true;  // Scene exists in the build settings
            }
        }

        return false;  // Scene does not exist
    }

    private void UnlockNextLevel() {
        // Get the current level index (for tracking unlocked levels)
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        int highestLevelUnlocked = PlayerPrefs.GetInt("LevelReached", 1);

        // If the player has completed this level and it's the highest level they've reached, unlock the next one
        if (currentLevelIndex >= highestLevelUnlocked) {
            PlayerPrefs.SetInt("LevelReached", currentLevelIndex + 1);
            PlayerPrefs.Save();
            Debug.Log("Next level unlocked: " + (currentLevelIndex + 1));
        }
    }
}