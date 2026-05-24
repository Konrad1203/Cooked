using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public Item heldItem;
    public Transform holdPoint;
    public Transform interactiveZone;

    Highlightable currentHighlight;
    public LayerMask interactableLayer;
    private float detectionRadius;
    private float throwForce;
    private float throwTorque;
    public InputActionReference grabPutAction;
    public InputActionReference workThrowAction;
    private GameObject hitObject;
    private IInteractable currentWorkStation;

    void Start() {
        var data = DataManager.Instance;
        detectionRadius = data.playerDetectionRadius;
        throwForce = data.playerThrowForce;
        throwTorque = data.playerThrowTorque;
        PlaceItemInHands(heldItem);
    }

    void Update()
    {
        hitObject = HandleRaycast();

        if (currentWorkStation != null && hitObject != currentWorkStation.GetGameObject())
            CancelWorkAction();
    }

    void OnEnable()
    {
        grabPutAction.action.started += HandleQuickAction;
        workThrowAction.action.started += HandleWorkThrowAction;
        workThrowAction.action.canceled += HandleWorkCancelAction;
    }

    void OnDisable()
    {
        grabPutAction.action.started -= HandleQuickAction;
        workThrowAction.action.started -= HandleWorkThrowAction;
        workThrowAction.action.canceled -= HandleWorkCancelAction;
    }

    private void HandleQuickAction(InputAction.CallbackContext context) {
        if (hitObject == null) {
            DropItemFromHands();
            return;
        } else if (hitObject.TryGetComponent<IInteractable>(out var interactableObj)) {
            interactableObj.OnQuickAction(holdPoint, heldItem, out Item playerNewItem);
            heldItem = playerNewItem;
        } else {
            if (heldItem == null && hitObject.TryGetComponent<Item>(out var item)) PlaceItemInHands(item);
            else DropItemFromHands();
        }
    }

    private void HandleWorkThrowAction(InputAction.CallbackContext context) {
        if (heldItem != null) { // Having item in hands
            ThrowObjectAway(heldItem);
            return;
        }
        if (
            hitObject != null &&
            hitObject.TryGetComponent<IInteractable>(out var interactableObj) && 
            interactableObj.IsWorkActionPossible() &&
            interactableObj != currentWorkStation)
        { // Work possible
            currentWorkStation = interactableObj;
            interactableObj.OnWorkActionStart(this, null);
        }
    }

    private void HandleWorkCancelAction(InputAction.CallbackContext context) {
        CancelWorkAction();
    }

    private void CancelWorkAction() {
        currentWorkStation?.OnWorkActionCancel(this, null);
        currentWorkStation = null;
    }

    private void ThrowObjectAway(Item itemToThrow) {
        DropItemFromHands();
        itemToThrow.Throw(transform, holdPoint, throwForce, throwTorque);
    }
        

    private GameObject HandleRaycast() {
        Highlightable h = FindBestInteractable();
        if (h != null) {
            if (currentHighlight != h) {
                ClearHighlight();
                currentHighlight = h;
            }
            currentHighlight.Highlight(true);
            return h.gameObject;
        } else {
            ClearHighlight();
        }
        return null;
    }

    private Highlightable FindBestInteractable() {
        Collider[] hits = Physics.OverlapSphere(interactiveZone.position, detectionRadius, interactableLayer);
        Highlightable bestTarget = null;
        float minDistance = float.MaxValue;
        foreach (Collider hit in hits) {
            if (hit.gameObject == heldItem) continue;
            Highlightable h = hit.transform.GetComponent<Highlightable>();
            if (h != null) {
                float dist = Vector3.Distance(interactiveZone.position, hit.transform.position);
                if (dist < minDistance) {
                    minDistance = dist;
                    bestTarget = h;
                }
            }
        }
        return bestTarget;
    }

    void ClearHighlight() {
        if (currentHighlight != null) {
            currentHighlight.Highlight(false);
            currentHighlight = null;
        }
    }

    public void PlaceItemInHands(Item newItem) {
        if (newItem == null) return;
        heldItem = newItem;
        newItem.SetState(ItemState.Held, holdPoint);
    }

    public void DropItemFromHands() {
        if (heldItem == null) return;
        if (!(bool)(heldItem.GetItemSO()?.dropable)) return;
        heldItem.SetState(ItemState.Free);
        heldItem = null;
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(interactiveZone.position, detectionRadius);
    }
}
