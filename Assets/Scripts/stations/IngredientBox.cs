using UnityEngine;

public class IngredientBox : CounterTop
{
    [SerializeField] private ItemSO ingredientSO;
    [SerializeField] private SpriteRenderer imageBox;

    public void Awake() {
        imageBox.sprite = ingredientSO.sprite;
    }

    protected override Item TakeItem(Transform playerHoldPoint) {
        if (itemOnCounter == null) itemOnCounter = ingredientSO.Spawn();
        return base.TakeItem(playerHoldPoint);
    }
}
