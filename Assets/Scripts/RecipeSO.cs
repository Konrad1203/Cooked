using UnityEngine;

[CreateAssetMenu(menuName = "Processing/Recipe")]
public class RecipeSO : ScriptableObject {
    public ItemSO input;
    public ItemSO output;
}
