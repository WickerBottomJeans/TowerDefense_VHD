using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

    public void PlayGame() {
        SceneManager.LoadScene("LevelSelection");
    }
    public void Challenge ()
    {
        SceneManager.LoadScene("Challenge");
    }    

    public void ExitGame() {
        Application.Quit();
        Debug.Log("Game Closed!");
    }
}