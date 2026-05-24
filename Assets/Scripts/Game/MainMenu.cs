using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void PlayGame(string sceneName)
    {
        SceneManager.Instance.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Player closed game");
        Application.Quit();
    }
}
