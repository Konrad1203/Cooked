using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIElementFollower : MonoBehaviour {

    [SerializeField] private Transform target3D;
    private Vector2 screenOffset;
    private RectTransform rectTransform;
    private Camera mainCam;
    private bool isVisible = true;

    void Start() {
        screenOffset = IconsCanvasManager.Instance.screenOffset;
        rectTransform = GetComponent<RectTransform>();
        mainCam = IconsCanvasManager.Instance.mainCamera;
    }

    public void SetTarget(Transform target) {
        target3D = target;
    }

    void LateUpdate() {
        if (target3D == null) {
            Destroy(gameObject);
            return;
        }
        Vector3 screenPoint = mainCam.WorldToScreenPoint(target3D.position);
        if (screenPoint.z <= 0) {
            if (isVisible) {
                rectTransform.localScale = Vector3.zero;
                isVisible = false;
            }
            return;
        }
        if (!isVisible) {
            rectTransform.localScale = Vector3.one;
            isVisible = true;
        }

        rectTransform.position = (Vector2) screenPoint + screenOffset;
    }
}
