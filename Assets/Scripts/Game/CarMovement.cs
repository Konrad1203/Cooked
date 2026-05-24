using UnityEngine;

public class CarMovement : MonoBehaviour
{
    private Vector3 startPosition;
    public Transform target;
    public float speed = 10f;

    void Start() {
        startPosition = transform.position;
    }
    void Update() {
        if (target != null) {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, target.position) < 0.01f) transform.position = startPosition;
        }
    }
}
