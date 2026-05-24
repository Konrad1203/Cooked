using UnityEngine;
using System.Collections.Generic;

public class Container : Item
{
    public readonly List<ItemSO> itemsOnPlate = new();

    private IconsCanvasManager iconsCanvas;
    private UIElementFollower myIconsContainer = null;
    private GameObject visualsParent = null;

    void Start() {
        iconsCanvas = IconsCanvasManager.Instance;
    }

    protected override void CreateIcon() {}

    public bool TryAddItem(ItemSO item) {
        if (!CanAddItem(item)) return false;
        itemsOnPlate.Add(item);
        UpdateCanvasVisibility();
        CreateVisualOnPlate(item);
        CreateIconUI(item);
        return true;
    }

    public bool CanAddItem(ItemSO item) {
        if (item.plateVisualPrefab == null) return false;
        if (itemsOnPlate.Contains(item)) return false;
        return true;
    }

    private void UpdateCanvasVisibility() {
        if (myIconsContainer == null) {
            myIconsContainer = Instantiate(iconsCanvas.itemIconsGridContainerPrefab, iconsCanvas.transform);
            myIconsContainer.SetTarget(transform);
        }
        bool shouldBeActive = itemsOnPlate.Count > 0;
        if (myIconsContainer.gameObject.activeSelf != shouldBeActive) {
            myIconsContainer.gameObject.SetActive(shouldBeActive);
        }
    }

    private void ClearCanvas() {
        foreach(Transform child in myIconsContainer.transform) Destroy(child.gameObject);
        myIconsContainer.gameObject.SetActive(false);
    }

    private void CreateVisualOnPlate(ItemSO item) {
        if (visualsParent == null) {
            visualsParent = new GameObject("VisualsParent");
            visualsParent.transform.SetParent(transform);
            visualsParent.transform.localPosition = Vector3.zero;
        }
        Instantiate(item.plateVisualPrefab, visualsParent.transform.position, visualsParent.transform.rotation, visualsParent.transform);
    }

    private void CreateIconUI(ItemSO item) {
        ItemIcon icon = Instantiate(iconsCanvas.itemIconPrefab, myIconsContainer.transform, false);
        icon.SetSprite(item.sprite);
    }

    public GameObject ClearPlate() {
        if (itemsOnPlate.Count == 0) return null;

        GameObject visuals = visualsParent;
        UIElementFollower iconsDuplicate = Instantiate(myIconsContainer, iconsCanvas.transform, false);
        iconsDuplicate.SetTarget(visuals.transform);
        visualsParent = null;

        itemsOnPlate.Clear();
        ClearCanvas();
        return visuals;
    }
}
