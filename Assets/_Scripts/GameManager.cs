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

    public void SetState(State state) {
        CurrentState = state;

        switch (state) {
            case State.Playing:
                Debug.Log("Game is now playing.");
                // Reset any lose/win UI and gameplay variables
                break;

            case State.Lose:
                Debug.Log("Game Over! You lose.");
                ShowLoseUI();
                break;

            case State.Win:
                Debug.Log("Congratulations! You win.");
                ShowWinUI();
                break;
        }
    }

    private void ShowLoseUI() {
        // TODO: Display "Game Over" UI with options like "Retry" or "Exit"
        // For example, enable a LosePanel in your canvas
    }

    private void ShowWinUI() {
        // TODO: Display "You Win" UI with options like "Next Level" or "Main Menu"
        // For example, enable a WinPanel in your canvas
    }

    public void RestartLevel() {
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        SetState(State.Playing);
    }

    public void ExitGame() {
        Debug.Log("Exiting game...");
        Application.Quit();
    }
}