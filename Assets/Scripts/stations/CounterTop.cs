using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Highlightable))]
public class CounterTop : MonoBehaviour, IInteractable
{
    public Transform holdPoint;
    public Item itemOnCounter;
    private Highlightable myHighlight;

    void Awake() {
        myHighlight = GetComponent<Highlightable>();
    }

    void Start() {
        if (itemOnCounter) {
            Item boundObject = itemOnCounter;
            itemOnCounter = null;
            PlaceItem(boundObject);
        }
    }

    public GameObject GetGameObject() {
        return gameObject;
    }

    public void OnQuickAction(Transform playerHoldPoint, Item heldItem, out Item playerNewItem)
    {
        if (heldItem == null)
        {
            playerNewItem = TakeItem(playerHoldPoint);
            return;
        }

        if (itemOnCounter == null)
        {
            if (PlaceItem(heldItem)) playerNewItem = null;
            else playerNewItem = heldItem;
            return;
        }

        playerNewItem = SwapItems(heldItem, playerHoldPoint);
    }

    public virtual bool IsWorkActionPossible() {
        return false;
    }

    public virtual void OnWorkActionStart(PlayerInteraction player, Item heldItem) {}

    public virtual void OnWorkActionCancel(PlayerInteraction player, Item heldItem) { }

    protected virtual bool PlaceItem(Item newItem) {
        itemOnCounter = newItem;
        itemOnCounter.SetState(ItemState.Placed, holdPoint);
        if (myHighlight != null) myHighlight.RefreshRenderers();
        return true;
    }

    protected virtual Item TakeItem(Transform playerHoldPoint) {
        if (itemOnCounter == null) return null;
        Item item = itemOnCounter;
        itemOnCounter = null;
        item.SetState(ItemState.Held, playerHoldPoint);
        if (myHighlight != null) myHighlight.RefreshRenderers();
        return item;
    }

    private Item SwapItems(Item heldItem, Transform playerHoldPoint) {
        if (heldItem.TryGetComponent<Container>(out var container)) {
            if (container.TryAddItem(itemOnCounter.GetItemSO())) {
                Destroy(itemOnCounter.gameObject);
                PlaceItem(container);
                return null;
            }

        }
        if (itemOnCounter.TryGetComponent<Container>(out var plate)) {
            if (plate.TryAddItem(heldItem.GetItemSO())) {
                Destroy(heldItem.gameObject);
                return null;
            }
        }
        Item counterItem = itemOnCounter;
        counterItem.SetState(ItemState.Held, playerHoldPoint);
        itemOnCounter = heldItem;
        heldItem.SetState(ItemState.Placed, holdPoint);
        if (myHighlight != null) myHighlight.RefreshRenderers();
        return counterItem;
    }

    protected void ReplaceItemWithNew(ItemSO newItemSO) {
        Destroy(itemOnCounter.gameObject);
        itemOnCounter = null;
        Item newItem = newItemSO.Spawn();
        PlaceItem(newItem);
    }
}
