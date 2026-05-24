using UnityEngine;

public class ServiceCounter : CounterTop
{
    protected override Item TakeItem(Transform playerHoldPoint) {
        return null;
    }

    protected override bool PlaceItem(Item newItem)
    {
        if (newItem.TryGetComponent<Container>(out var plate)) {
            base.PlaceItem(newItem);
            itemOnCounter = null;
            bool served = GameManager.Instance.RegisterService(plate);
            if (!served) {
                Destroy(newItem.gameObject);
            }
            return true;
        }
        return false;
    }
}
