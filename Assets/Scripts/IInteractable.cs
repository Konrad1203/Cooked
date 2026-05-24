using UnityEngine;

public interface IInteractable
{
    GameObject GetGameObject();
    
    // Space: grab / drop action
    void OnQuickAction(Transform playerHoldPoint, Item heldItem, out Item playerNewItem);

    // E: work / cut action
    bool IsWorkActionPossible();
    void OnWorkActionStart(PlayerInteraction player, Item heldItem);
    void OnWorkActionCancel(PlayerInteraction player, Item heldItem);
}
