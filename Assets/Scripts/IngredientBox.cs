using UnityEngine;

public class IngredientBox : CounterTop
{
    public GameObject ingredientPrefab;

    protected override GameObject TakeItem() {
        if (itemOnCounter != null) return base.TakeItem();
        GameObject newItem = Instantiate(ingredientPrefab);
        return newItem;
    }
}
