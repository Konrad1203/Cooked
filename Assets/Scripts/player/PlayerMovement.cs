using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private float moveSpeed;
    private float pushForce;
    CharacterController controller;

    public InputActionReference moveAction;

    private Vector3 currentMoveVelocity;

    private float smoothing;

    void Start()
    {
        var data = DataManager.Instance;
        moveSpeed = data.playerMoveSpeed;
        pushForce = data.playerMovePushForce;
        smoothing = data.playerMovementSmoothing;
        controller = GetComponent<CharacterController>();
        transform.forward = UnityEngine.Vector3.back;
    }

    void Update()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        Vector3 targetMove = new(input.x, 0, input.y);
        currentMoveVelocity = UnityEngine.Vector3.Lerp(currentMoveVelocity, targetMove, Time.deltaTime * smoothing);
        controller.Move(moveSpeed * Time.deltaTime * currentMoveVelocity);
        if (currentMoveVelocity.magnitude > 0.1f)
            transform.forward = currentMoveVelocity;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 2f);
    }

    void OnControllerColliderHit(ControllerColliderHit hit) {
        Rigidbody rb = hit.collider.attachedRigidbody;
        if (rb == null || rb.isKinematic) return;
        if (hit.moveDirection.y < -0.3f) return;
        Vector3 pushDir = new(hit.moveDirection.x, 0, hit.moveDirection.z);
        rb.AddForce(pushDir * pushForce, ForceMode.Impulse);
    }

    void LateUpdate() {
        transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
    }
}
