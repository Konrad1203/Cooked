using UnityEngine;

public class SnapTrigger : MonoBehaviour
{
    public CounterTop myParentCounter;

    private void OnTriggerEnter(Collider other) {
        if (myParentCounter.itemOnCounter == null) {
            Item item = other.GetComponentInParent<Item>();
            if (item != null && item.StopFlightIfPossible()) {
                myParentCounter.OnQuickAction(null, item, out _);
            }
        } else if (myParentCounter.itemOnCounter.TryGetComponent<Container>(out var container)) {
            Item item = other.GetComponentInParent<Item>();
            if (item != null && container.CanAddItem(item.GetItemSO()) && item.StopFlightIfPossible()) {
                container.TryAddItem(item.GetItemSO());
                Destroy(item.gameObject);
            }
        }
    }
}
