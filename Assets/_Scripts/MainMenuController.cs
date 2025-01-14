using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {
    private const string LEVEL_SELECTION_SCENE = "LevelSelection";
    private const string CHALLENGE_SCENE = "Challenge";

    public void PlayGame() {
        SceneManager.LoadScene(LEVEL_SELECTION_SCENE);
    }

    public void Challenge() {
        SceneManager.LoadScene(CHALLENGE_SCENE);
    }

    public void ExitGame() {
        Application.Quit();
        Debug.Log("Game Closed!");
    }
}