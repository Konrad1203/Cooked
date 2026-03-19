using UnityEngine;

public interface IInteractable
{
    // Space: grab / drop action
    void OnQuickAction(PlayerInteraction player, GameObject heldItem);

    // E: work / cut action
    // void OnWorkAction(PlayerInteraction player, GameObject heldItem);
}
