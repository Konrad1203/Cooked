using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ScoreVisual : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text multiplierText;
    [SerializeField] private float indicatorLifetime = 0.5f;
    [SerializeField] private float indicatorRiseDistance = 40f;
    [SerializeField] private float indicatorStackSpacing = 18f;
    [SerializeField] private float indicatorFadeInTime = 0.08f;

    private readonly List<RectTransform> activeIndicators = new();
    private int lastPoints;

    void Awake()
    {
        scoreText.text = "0";
        multiplierText.text = "x1.0";
        lastPoints = 0;
    }
    
    private void OnEnable()
    {
        GameManager.OnPointsChanged += UpdateScoreText;
        GameManager.OnMultiplierChanged += UpdateMultiplierText;
    }

    private void OnDisable()
    {
        GameManager.OnPointsChanged -= UpdateScoreText;
        GameManager.OnMultiplierChanged -= UpdateMultiplierText;
    }

    private void UpdateScoreText(int newPointsCount)
    {
        int delta = newPointsCount - lastPoints;
        scoreText.text = newPointsCount.ToString();
        lastPoints = newPointsCount;
        if (delta != 0) ShowScoreIndicator(delta);
    }

    private void UpdateMultiplierText(float newMultiplier)
    {
        multiplierText.text = newMultiplier.ToString("x0.0");
    }

    private void ShowScoreIndicator(int delta)
    {
        TMP_Text indicator = Instantiate(scoreText, scoreText.transform.parent);
        indicator.text = $"{(delta > 0 ? "+" : "-")}{Mathf.Abs(delta)}";
        indicator.color = delta > 0 ? Color.green : Color.red;
        indicator.raycastTarget = false;

        RectTransform indicatorRect = indicator.rectTransform;
        RectTransform scoreRect = scoreText.rectTransform;
        indicatorRect.SetAsLastSibling();

        Vector2 basePosition = scoreRect.anchoredPosition;
        basePosition.y += 30;
        Vector2 startPosition = basePosition + Vector2.up * (indicatorStackSpacing * activeIndicators.Count);
        indicatorRect.anchoredPosition = startPosition;

        CanvasGroup canvasGroup = indicator.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = indicator.gameObject.AddComponent<CanvasGroup>();
        }

        activeIndicators.Add(indicatorRect);
        StartCoroutine(AnimateIndicator(indicator, indicatorRect, canvasGroup));
    }

    private IEnumerator AnimateIndicator(TMP_Text indicator, RectTransform indicatorRect, CanvasGroup canvasGroup)
    {
        Vector2 startPosition = indicatorRect.anchoredPosition;
        Vector2 endPosition = startPosition + Vector2.up * indicatorRiseDistance;

        float elapsed = 0f;
        while (elapsed < indicatorLifetime)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / indicatorLifetime);
            float fadeProgress = progress < indicatorFadeInTime
                ? Mathf.Clamp01(progress / indicatorFadeInTime)
                : 1f - (progress - indicatorFadeInTime) / Mathf.Max(0.0001f, 1f - indicatorFadeInTime);

            indicatorRect.anchoredPosition = Vector2.Lerp(startPosition, endPosition, progress);
            canvasGroup.alpha = Mathf.Clamp01(fadeProgress);
            yield return null;
        }

        activeIndicators.Remove(indicatorRect);
        Destroy(indicator.gameObject);
    }
}
