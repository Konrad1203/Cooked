using UnityEngine;
using UnityEngine.UI;

public class IconsCanvasManager : MonoBehaviour
{
    public static IconsCanvasManager Instance;

    public Camera mainCamera;
    public Vector2 screenOffset = new(0, 100);
    public ItemIcon itemIconPrefab;
    public UIElementFollower itemIconsGridContainerPrefab;
    public UIElementFollower itemIconStandalonePrefab;
    public Slider progressSliderPrefab; // UIElementFollower

    void Awake() {
        mainCamera = Camera.main;
        Instance = this;
    }

    // public Camera MainCamera =>
    //     _mainCamera != null ? _mainCamera : _mainCamera = Camera.main;
}
