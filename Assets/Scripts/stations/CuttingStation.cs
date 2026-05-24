using UnityEngine;
using UnityEngine.UI;

public class CuttingStation : CounterTop
{
    [SerializeField] private RecipeBookSO recipeBook;
    private float timeToCut;
    private float currentProgress = 0f;
    private bool isCutting = false;
    private Slider myProgressSlider;

    public override bool IsWorkActionPossible() {
        return true;
    }

    public override void OnWorkActionStart(PlayerInteraction player, Item heldItem) {
        StartCutting();
    }

    public override void OnWorkActionCancel(PlayerInteraction player, Item heldItem) {
        StopCutting();
    }

    void Start() {
        timeToCut = DataManager.Instance.stationCuttingTime;
    }

    void Update() {
        if (isCutting) {
            if (currentProgress < timeToCut) {
                currentProgress += Time.deltaTime;
                myProgressSlider.value = currentProgress;
            } else {
                FinishCutting();
            }
        }
    }

    private void StartCutting() {
        if (itemOnCounter != null && recipeBook != null && recipeBook.HasRecipeFor(itemOnCounter.GetItemSO())) {
            EnableProgressSlider();
            isCutting = true;
        }
    }

    private void EnableProgressSlider() {
        if (myProgressSlider == null) {
            var canvasManager = IconsCanvasManager.Instance;
            myProgressSlider = Instantiate(canvasManager.progressSliderPrefab, canvasManager.transform, false);
            myProgressSlider.maxValue = timeToCut;
            myProgressSlider.value = 0;
        }
        myProgressSlider.GetComponent<UIElementFollower>().SetTarget(itemOnCounter.transform);
        myProgressSlider.gameObject.SetActive(true);
    }

    private void StopCutting() {
        isCutting = false;
    }

    private void ResetCutProgress() {
        isCutting = false;
        currentProgress = 0;
        if (myProgressSlider != null)
            myProgressSlider.gameObject.SetActive(false);
    }

    protected override Item TakeItem(Transform playerHoldPoint) {
        if (itemOnCounter != null) ResetCutProgress();
        return base.TakeItem(playerHoldPoint);
    }

    private void FinishCutting() {
        ResetCutProgress();
        ItemSO cutItem = recipeBook.GetRecipeOutput(itemOnCounter.GetItemSO());
        if (cutItem) ReplaceItemWithNew(cutItem);
        else Debug.Log("Unable to find recipe for cutting: " + itemOnCounter);
    }
}
