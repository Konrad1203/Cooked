using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelEndMenu : MonoBehaviour
{
    public static LevelEndMenu Instance { get; private set; }

    public GameObject content;
    public GameObject firstSelectedButton;
    public Sprite filledStarSprite;
    public Image star1;
    public Image star2;
    public Image star3;
    public TMP_Text pointsFor1Star;
    public TMP_Text pointsFor2Stars;
    public TMP_Text pointsFor3Stars;
    public TMP_Text pointsCollected;
    public TMP_Text pointsBestScore;

    void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start() {
        content.SetActive(false);
    }

    public void SetupAndActivate(LevelInfoSO level, int score) {
        int bestScore = CheckAndSaveHighScore(level.name, score);
        pointsFor1Star.text = level.pointsRequiredFor1Star.ToString();
        if (score >= level.pointsRequiredFor1Star) star1.sprite = filledStarSprite;
        pointsFor2Stars.text = level.pointsRequiredFor2Stars.ToString();
        if (score >= level.pointsRequiredFor2Stars) star2.sprite = filledStarSprite;
        pointsFor3Stars.text = level.pointsRequiredFor3Stars.ToString();
        if (score >= level.pointsRequiredFor3Stars) star3.sprite = filledStarSprite;
        pointsCollected.text = score.ToString();
        pointsBestScore.text = bestScore.ToString();
        Time.timeScale = 0f;
        content.SetActive(true);
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }

    public void RestartGame() {
        Time.timeScale = 1f;
        content.SetActive(false);
        SceneManager.Instance.RestartLevel();
    }

    public void ExitToMainMenu() {
        Time.timeScale = 1f;
        content.SetActive(false);
        SceneManager.Instance.ExitToMainMenu();
    }

    public int CheckAndSaveHighScore(string levelName, int currentScore) {
        string saveKey = GetSaveKey(levelName);
        int oldHighScore = PlayerPrefs.GetInt(saveKey, 0);
        if (currentScore > oldHighScore) {
            PlayerPrefs.SetInt(saveKey, currentScore);
            PlayerPrefs.Save();
            return currentScore;
        }
        return oldHighScore;
    }

    public int GetHighScore(string levelName) {
        return PlayerPrefs.GetInt(GetSaveKey(levelName), 0);
    }

    public string GetSaveKey(string levelName) {
        return "HighScore_" + levelName;
    }
}
