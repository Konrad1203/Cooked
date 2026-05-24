using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public InputActionReference pauseAction;
    public static bool isGamePaused = false;
    public GameObject content;
    public GameObject firstSelectedButton;

    void Start() {
        Resume();
    }

    void OnEnable() {
        pauseAction.action.started += HandlePauseGameAction;
    }

    void OnDisable() {
        pauseAction.action.started -= HandlePauseGameAction;
    }

    private void HandlePauseGameAction(InputAction.CallbackContext context) {
        if (isGamePaused) Resume();
        else Pause();
    }

    public void Resume() {
        content.SetActive(false);
        Time.timeScale = 1f;
        isGamePaused = false;
    }

    private void Pause() {
        content.SetActive(true);
        Time.timeScale = 0f;
        isGamePaused = true;
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }

    public void RestartGame() {
        Resume();
        SceneManager.Instance.RestartLevel();
    }

    public void ExitToMainMenu() {
        Resume();
        SceneManager.Instance.ExitToMainMenu();
    }
}
