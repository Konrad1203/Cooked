using UnityEngine;

[CreateAssetMenu(menuName = "Item")]
public class ItemSO : ScriptableObject
{
    public Sprite sprite;
    public Item prefab;
    public bool dropable = true;
    public bool showIcon = false;
    public GameObject plateVisualPrefab;

    public Item Spawn() {
        return Instantiate(prefab);
    }
}
