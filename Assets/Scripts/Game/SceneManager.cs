using System.Collections;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance { get; private set; }

    public GameObject InGameObjects;
    public GameManager gameManagerPrefab;
    public GameObject[] managerPrefabs;

    public GameObject MainMenuObjects;
    public IntroAnimation introScene;
    public LevelInfoSO[] levels;


    private GameManager currentGameManager;
    private string currentLevelName;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        InGameObjects.SetActive(false);
        MainMenuObjects.SetActive(true);
    }

    public void LoadScene(string name)
    {
        currentLevelName = name;
        var level = levels[0];
        if (level == null) {
            Debug.Log("Nie został znaleziony level o nazwie: " + name);
            return;
        }
        Debug.Log("Chcemy załadować scenę: " + name);

        currentGameManager = Instantiate(gameManagerPrefab, InGameObjects.transform, true);
        currentGameManager.SetCurrentLevelAndInstantiate(level);

        foreach (var managerPrefab in managerPrefabs) {
            Instantiate(managerPrefab, currentGameManager.transform, true);
        }
        MainMenuObjects.SetActive(false);
        InGameObjects.SetActive(true);
        currentGameManager.StartLevel();
    }

    private void LoadMainScreen() {
        InGameObjects.SetActive(false);
        MainMenuObjects.SetActive(true);
        introScene.Start();
    }

    public void RestartLevel() {
        StartCoroutine(RestartLevelRoutine());
    }

    private IEnumerator RestartLevelRoutine() {
        RemoveCurrentGame();
        yield return new WaitForEndOfFrame();
        LoadScene(currentLevelName);
    }

    public void ExitToMainMenu() {
        RemoveCurrentGame();
        LoadMainScreen();
    }

    private void RemoveCurrentGame() {
        Destroy(currentGameManager.gameObject);
        currentGameManager = null;
    }
}
