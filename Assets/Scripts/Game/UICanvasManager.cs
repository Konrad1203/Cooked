using UnityEngine;

public class UICanvasManager : MonoBehaviour
{
    public static UICanvasManager Instance { get; private set; }

    public RectTransform orderNotes;
    public OrderUINote orderNotePrefab;
    public ItemIcon orderNoteIngredientPrefab;
    public Gradient timerGradient;

    void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}
