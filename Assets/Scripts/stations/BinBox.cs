using UnityEngine;
using System.Collections;

public class BinBox : CounterTop
{
    private float shrinkDuration;

    void Start() {
        shrinkDuration = DataManager.Instance.stationTrashShrinkingTime;
    }

    protected override Item TakeItem(Transform playerHoldPoint) {
        return null;
    }

    protected override bool PlaceItem(Item newItem)
    {
        if (newItem.TryGetComponent<Container>(out var container)) {
            GameObject objectToThrowAway = container.ClearPlate();
            if (objectToThrowAway != null) {
                objectToThrowAway.transform.SetPositionAndRotation(holdPoint.transform.position, Quaternion.identity);
                StartCoroutine(ShrinkAndDestroy(objectToThrowAway));
            }
            return false;
        }
        base.PlaceItem(newItem);
        itemOnCounter = null;
        StartCoroutine(ShrinkAndDestroy(newItem.gameObject));
        return true;
    }

    private IEnumerator ShrinkAndDestroy(GameObject item)
    {
        Vector3 initialScale = item.transform.localScale;
        Vector3 initialPosition = item.transform.position;
        float timer = 0f;
        while (timer < shrinkDuration)
        {
            timer += Time.deltaTime;
            float normalizedTime = timer / shrinkDuration;
            item.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, normalizedTime);
            item.transform.position = Vector3.Lerp(initialPosition, new Vector3(initialPosition.x, initialPosition.y - 0.3f, initialPosition.z), normalizedTime);
            yield return null;
        }
        item.transform.localScale = Vector3.zero;
        Destroy(item);
    }
}
