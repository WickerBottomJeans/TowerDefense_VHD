using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public enum State {
        Playing,
        Lose,
        Win
    }

    public static GameManager Instance { get; private set; }
    public State CurrentState { get; private set; }

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
        } else if (Input.GetKeyDown(KeyCode.M)) {
            OpenMenu();
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
        }
    }

    private void ShowLoseUI() {
        Debug.Log("Lose UI should be displayed here.");
        RestartLevel();
    }

    private void ShowWinUI() {
        Debug.Log("Win UI should be displayed here.");
        RestartLevel();
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

    private void UnlockNextLevel() {
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        int highestLevelUnlocked = PlayerPrefs.GetInt("LevelReached", 1);

        if (currentLevelIndex >= highestLevelUnlocked) {
            PlayerPrefs.SetInt("LevelReached", currentLevelIndex + 1);
            PlayerPrefs.Save();
            Debug.Log("Next level unlocked: " + (currentLevelIndex + 1));
        }
    }
}