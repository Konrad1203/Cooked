using UnityEngine;
using UnityEngine.UI;

public class ItemIcon : MonoBehaviour
{
    [SerializeField] private Image image;

    public void SetSprite(Sprite sprite) {
        image.sprite = sprite;
    }
}
