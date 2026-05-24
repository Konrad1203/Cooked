using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TimeVisual : MonoBehaviour
{
    [Header("Komponenty UI")]
    [SerializeField] private Slider timerSlider;
    [SerializeField] private Image fillImage;
    [SerializeField] private TMP_Text timerText;

    private float maxTime;

    void Start()
    {
        maxTime = GameManager.Instance.GetLevelDuration();
        if (timerSlider != null)
        {
            timerSlider.maxValue = maxTime;
            timerSlider.value = maxTime;
        }
    }

    void LateUpdate()
    {
        if (!GameManager.Instance.IsGameActive) return;
        float timeRemaining = GameManager.Instance.TimeRemaining;
        timerSlider.value = timeRemaining;
        int secondsInt = Mathf.CeilToInt(timeRemaining);
        int minutes = secondsInt / 60;
        int seconds = secondsInt % 60;
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        float timeRatio = timeRemaining / maxTime;
        fillImage.color = UICanvasManager.Instance.timerGradient.Evaluate(timeRatio);
    }
}
