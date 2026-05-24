using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.InputSystem.LowLevel;

[RequireComponent(typeof(RectTransform))]
public class OrderUINote : MonoBehaviour
{
    [SerializeField] private float spawnAnimationDuration = 0.35f;
    [SerializeField] private float spawnAnimationScale = 0.85f;

    [SerializeField] private Slider timerSlider;
    [SerializeField] private Image timerFillImage;
    [SerializeField] private Image dishIcon;
    [SerializeField] private RectTransform ingredientsGroup;
    public OrderSO Order;

    private float maxTime;
    private float currTime;

    private CanvasGroup canvasGroup;
    private Coroutine spawnAnimationCoroutine;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void SetOrder(OrderSO order)
    {
        Order = order;
        dishIcon.sprite = order.orderIcon;
        maxTime = GameManager.Instance.GetOrderDecayTime();
        timerSlider.maxValue = maxTime;
        timerSlider.value = maxTime;
        currTime = maxTime;

        var prefab = UICanvasManager.Instance.orderNoteIngredientPrefab;
        foreach (ItemSO ingredient in order.requiredItems)
        {
            ItemIcon ingredientIcon = Instantiate(prefab, ingredientsGroup.transform);
            ingredientIcon.SetSprite(ingredient.sprite);
        }
        // Canvas.ForceUpdateCanvases();
        // LayoutRebuilder.ForceRebuildLayoutImmediate(ingredientsGroup);
        // GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ingredientsGroup.rect.width + 10);

        SetSelfWidth(order.requiredItems.Length);
        PlaySpawnAnimation();
    }

    private void SetSelfWidth(int orderSize)
    {
        //                          for item box     spacing between   margin inline + 10 points bigger than ingredients list
        int width = Math.Max(160, orderSize * 60 + (orderSize - 1) * 5 + 20);
        GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }

    void LateUpdate()
    {
        if (!GameManager.Instance.IsGameActive) return;
        currTime -= Time.deltaTime;
        if (currTime < 0)
        {
            DestroyItself();
            return;
        }
        timerSlider.value = currTime;
        timerFillImage.color = UICanvasManager.Instance.timerGradient.Evaluate(currTime / maxTime);
    }

    private void DestroyItself()
    {
        GameManager.Instance.RemoveOrderDueToTimeout(this);
        StartCoroutine(DespawnAndDestroyRoutine());
    }

    private void PlaySpawnAnimation() {
        if (spawnAnimationCoroutine != null) StopCoroutine(spawnAnimationCoroutine);
        spawnAnimationCoroutine = StartCoroutine(SpawnAnimationRoutine());
    }

    private IEnumerator SpawnAnimationRoutine()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector3 targetScale = Vector3.one;
        Vector3 startScale = Vector3.one * spawnAnimationScale;

        rectTransform.localScale = startScale;
        canvasGroup.alpha = 0f;

        float elapsed = 0f;
        while (elapsed < spawnAnimationDuration) {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / spawnAnimationDuration);
            float easedProgress = 1f - Mathf.Pow(1f - progress, 3f);

            rectTransform.localScale = Vector3.Lerp(startScale, targetScale, easedProgress);
            canvasGroup.alpha = easedProgress;
            yield return null;
        }

        rectTransform.localScale = targetScale;
        canvasGroup.alpha = 1f;
        spawnAnimationCoroutine = null;
    }

    public void InitiateDespawn() {
        StartCoroutine(DespawnAndDestroyRoutine());
    }

    private IEnumerator DespawnAndDestroyRoutine()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector3 startScale = rectTransform.localScale;
        Vector3 endScale = Vector3.one * spawnAnimationScale;

        float elapsed = 0f;
        while (elapsed < spawnAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / spawnAnimationDuration);
            float easedProgress = 1f - Mathf.Pow(1f - progress, 3f);

            rectTransform.localScale = Vector3.Lerp(startScale, endScale, easedProgress);
            canvasGroup.alpha = 1f - easedProgress;
            yield return null;
        }

        Destroy(gameObject);
    }
}
