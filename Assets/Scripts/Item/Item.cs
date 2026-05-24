using UnityEngine;

[RequireComponent(typeof(Highlightable))]
[RequireComponent(typeof(Collider))]
public class Item : MonoBehaviour
{
    [SerializeField] protected ItemSO itemSO;
    private bool isFlying;
    private float flyTimer;
    private float minFlyTime;
    private Rigidbody rb;
    private Collider collider_;
    private Highlightable highlight;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        collider_ = GetComponent<Collider>();
        highlight = GetComponent<Highlightable>();
    }

    void Start() {
        minFlyTime = DataManager.Instance.itemMinFlightTime;
        CreateIcon();
    }

    void Update() {
        if (isFlying) flyTimer += Time.deltaTime;
    }

    protected virtual void CreateIcon()
    {       
        if (itemSO && itemSO.showIcon) {
            IconsCanvasManager manager = IconsCanvasManager.Instance;
            UIElementFollower icon = Instantiate(manager.itemIconStandalonePrefab, manager.transform, false);
            icon.SetTarget(transform);
            icon.GetComponent<ItemIcon>().SetSprite(itemSO.sprite);
        }
    }

    public void SetState(ItemState state, Transform target = null)
    {
        switch (state) {
            case ItemState.Held:
                transform.SetParent(target);
                transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                SetPhysics(false);
                highlight.Highlight(false);
                break;

            case ItemState.Free:
                transform.SetParent(GameManager.Instance.transform);
                SetPhysics(true);
                break;

            case ItemState.Placed:
                transform.SetParent(target);
                transform.SetPositionAndRotation(target.position, target.rotation);
                SetPhysics(false);
                break;
        }
    }

    private void SetPhysics(bool enabled) {
        collider_.enabled = enabled;
        if (rb == null) return;
        isFlying = enabled;
        flyTimer = 0f;
        rb.isKinematic = !enabled;
        rb.detectCollisions = enabled;
    }

    public void Throw(Transform playerTransform, Transform holdPoint, float pushForce, float throwTorque)
    {
        if (rb == null) return;
        transform.position = holdPoint.position + playerTransform.forward * 0.35f + Vector3.up * 0.15f;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 throwDirection = (playerTransform.forward + Vector3.up * 0.25f).normalized;
        rb.AddForce(throwDirection * pushForce, ForceMode.Impulse);

        Vector3 spin = (playerTransform.right * Random.Range(-0.5f, 0.5f) + playerTransform.up * Random.Range(0.2f, 0.6f)).normalized;
        rb.AddTorque(spin * throwTorque, ForceMode.Impulse);
    }

    public bool StopFlightIfPossible() {
        bool canBeSnapped = isFlying && flyTimer >= minFlyTime;
        if (canBeSnapped) isFlying = false;
        return canBeSnapped;
    }

    public ItemSO GetItemSO()
    {
        return itemSO;
    }
}
