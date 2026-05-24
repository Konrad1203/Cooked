using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Processing/Recipe book")]
public class RecipeBookSO : ScriptableObject
{
    public RecipeSO[] recipes;
    public Dictionary<ItemSO, ItemSO> recipeLookup;

    private void OnEnable() {
        BuildLookup();
    }

    private void OnValidate() {
        BuildLookup();
    }

    private void BuildLookup() {
        recipeLookup = new Dictionary<ItemSO, ItemSO>();
        if (recipes == null) return;
        foreach (var recipe in recipes) {
            if (recipe == null || recipe.input == null || recipe.output == null) continue;
            recipeLookup[recipe.input] = recipe.output;
        }
    }

    public bool HasRecipeFor(ItemSO input) {
        return input != null && recipeLookup != null && recipeLookup.ContainsKey(input);
    }

    public ItemSO GetRecipeOutput(ItemSO input) {
        if (input == null || recipeLookup == null) return null;
        if (recipeLookup.TryGetValue(input, out ItemSO output)) return output;
        return null;
    }
}
