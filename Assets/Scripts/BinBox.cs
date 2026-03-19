using UnityEngine;
using System.Collections;

public class BinBox : CounterTop
{
    public float shrinkDuration = 0.5f;

    protected override GameObject TakeItem()
    {
        return null;
    }

    protected override void PlaceItem(GameObject newItem)
    {
        base.PlaceItem(newItem);
        itemOnCounter = null;
        StartCoroutine(ShrinkAndDestroy(newItem));
    }

    private IEnumerator ShrinkAndDestroy(GameObject item)
    {
        Vector3 initialScale = item.transform.localScale;
        float timer = 0f;
        while (timer < shrinkDuration)
        {
            timer += Time.deltaTime;
            float normalizedTime = timer / shrinkDuration;
            item.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, normalizedTime);
            yield return null;
        }
        item.transform.localScale = Vector3.zero;
        Destroy(item);
    }
}
