using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Highlightable))]
public class CounterTop : MonoBehaviour, IInteractable
{
    public GameObject itemOnCounter;
    private Highlightable myHighlight;

    void Awake() {
        myHighlight = GetComponent<Highlightable>();
    }

    void Start() {
        if (itemOnCounter) {
            GameObject boundObject = itemOnCounter;
            itemOnCounter = null;
            PlaceItem(boundObject);
        }
    }

    public void OnQuickAction(PlayerInteraction player, GameObject heldItem)
    {
        if (heldItem != null) // Player is holding item
        {
            if (itemOnCounter != null) return;
            PlaceItem(heldItem);
            player.TakeItemFromHands();
        } 
        else // Player has empty hands
        {
            GameObject item = TakeItem();
            if (item == null) return;
            player.PlaceItemInHands(item);
        }
    }

    protected virtual void PlaceItem(GameObject newItem) {
        itemOnCounter = newItem;
        newItem.transform.parent = transform;
        newItem.transform.SetPositionAndRotation(
            new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z),
            transform.rotation
        );
        newItem.layer = LayerMask.NameToLayer("Default");

        Rigidbody rb = newItem.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        if (myHighlight != null)
            myHighlight.RefreshRenderers();
    }

    protected virtual GameObject TakeItem() {
        if (itemOnCounter == null) return null;
        GameObject item = itemOnCounter;
        itemOnCounter = null;

        item.transform.parent = null;
        item.layer = LayerMask.NameToLayer("Interactable");

        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;

        if (myHighlight != null)
            myHighlight.RefreshRenderers();
        
        return item;
    }
}
