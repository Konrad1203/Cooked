using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public GameObject heldItem;
    public Transform holdPoint;
    public Transform interactiveZone;

    Highlightable currentHighlight;
    public LayerMask interactableLayer;
    public float detectionRadius = 0.8f;
    public float pushForce = 5.0f;
    public float throwTorque = 1.5f;

    void Start() {
        PlaceItemInHands(heldItem);
    }

    void Update() {
        GameObject hitObject = HandleRaycast();
        if (Input.GetKeyDown(KeyCode.Space)) HandleQuickAction(hitObject);
        if (Input.GetKeyDown(KeyCode.LeftAlt)) HandleThrowAction();
        KeepItemInHands();
    }

    private void HandleQuickAction(GameObject hitObject) {
        if (hitObject == null) {
            if (heldItem != null) TakeItemFromHands();
            return;
        }
        if (hitObject.TryGetComponent<IInteractable>(out var interactableObj)){
            interactableObj.OnQuickAction(this, heldItem);
        }
        else { // Fruit on the ground
            if (heldItem == null) PlaceItemInHands(hitObject);
            else TakeItemFromHands();
        }
    }

    private void HandleThrowAction() {
        if (heldItem == null) return;

        GameObject itemToThrow = heldItem;
        TakeItemFromHands();

        // Move thrown item slightly ahead of the player to avoid clipping.
        itemToThrow.transform.position = holdPoint.position + transform.forward * 0.35f + Vector3.up * 0.15f;

        if (itemToThrow.TryGetComponent<Rigidbody>(out var rb)) {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            Vector3 throwDirection = (transform.forward + Vector3.up * 0.25f).normalized;
            rb.AddForce(throwDirection * pushForce, ForceMode.Impulse);

            Vector3 spin = (transform.right * Random.Range(-0.5f, 0.5f) + transform.up * Random.Range(0.2f, 0.6f)).normalized;
            rb.AddTorque(spin * throwTorque, ForceMode.Impulse);
        }
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

    public void PlaceItemInHands(GameObject newItem) {
        if (newItem != null) {
            heldItem = newItem;
            newItem.transform.position = holdPoint.position;
            newItem.transform.rotation = holdPoint.rotation;

            Highlightable newItemHighlight = newItem.transform.GetComponent<Highlightable>();
            if (newItemHighlight != null) {
                newItemHighlight.Highlight(false);
            }
            Collider itemCollider = newItem.GetComponent<Collider>();
            if (itemCollider) itemCollider.isTrigger = true;
        }
    }

    public void TakeItemFromHands() {
        if (heldItem == null) return;
        Collider itemCollider = heldItem.GetComponent<Collider>();
        if (itemCollider != null) itemCollider.isTrigger = false;
        heldItem = null;
    }

    private void KeepItemInHands() {
        if (heldItem != null) {
            heldItem.transform.position = holdPoint.position;
            heldItem.transform.rotation = holdPoint.rotation;
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(interactiveZone.position, detectionRadius);
    }
}
