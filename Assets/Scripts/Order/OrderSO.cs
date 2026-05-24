using UnityEngine;

[CreateAssetMenu(fileName = "Order", menuName = "Order")]
public class OrderSO : ScriptableObject
{
    public Sprite orderIcon;
    public ItemSO[] requiredItems;
}
