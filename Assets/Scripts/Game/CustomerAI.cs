using UnityEngine;
using UnityEngine.AI;

public class CustomerAI : MonoBehaviour
{
    public MeshRenderer[] colorRenderers;
    public Transform holdPoint;
    public float speed = 4f;
    public float rotationSpeed = 8f;

    private Transform targetSlot;
    private Transform nextTargetSlot;
    private bool destroyOnTarget = false;

    void Start()
    {
        ApplyRandomColor();
    }

    void Update()
    {
        if (targetSlot != null)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, 
                targetSlot.position, 
                speed * Time.deltaTime
            );
            Transform lookTarget = nextTargetSlot != null ? nextTargetSlot : targetSlot;
            RotateTowards(lookTarget.position);
            
            if (Vector3.Distance(transform.position, targetSlot.position) < 0.01f) {
                transform.position = targetSlot.position;
                targetSlot = null;
                if (destroyOnTarget) Destroy(gameObject);
            }
        }
    }

    private void RotateTowards(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            direction.y = 0; 
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

    public void SetTarget(Transform target, Transform nextTarget)
    {
        targetSlot = target;
        nextTargetSlot = nextTarget;
    }

    public void ServePlateSetTargetAndDestroy(Item plate, Transform target)
    {
        if (plate != null) plate.SetState(ItemState.Held, holdPoint);
        targetSlot = target;
        nextTargetSlot = target;
        destroyOnTarget = true;
    }

    void ApplyRandomColor()
    {
        if (colorRenderers == null) return;
        Color randomColor = Color.HSVToRGB(Random.Range(0f, 1f), 0.9f, 0.7f);
        foreach (MeshRenderer renderer in colorRenderers)
            renderer.material.color = randomColor;
    }
}
