using UnityEngine;

public class Highlightable : MonoBehaviour
{
    private Color highlightColor = new Color(0.4f, 0.4f, 0.1f);
    private Color spriteHighlightColor = new Color(2.0f, 2.0f, 0.5f);
    private Renderer[] renderers;
    private MaterialPropertyBlock propBlock;
    private static readonly int EmissionColorProp = Shader.PropertyToID("_EmissionColor");
    private static readonly int MainColorProp = Shader.PropertyToID("_Color");

    void Awake() {
        propBlock = new MaterialPropertyBlock();
        RefreshRenderers();
    }

    public void RefreshRenderers() {
        renderers = GetComponentsInChildren<Renderer>();
    }

    public void Highlight(bool active) {
        foreach (Renderer rend in renderers) {
            if (rend == null) continue;
            if (rend is SpriteRenderer sr) {
                Color sColor = active ? spriteHighlightColor : Color.white;
                sr.color = sColor;
            } else {
                rend.GetPropertyBlock(propBlock);
                Color eColor = active ? highlightColor : Color.black;
                propBlock.SetColor(EmissionColorProp, eColor);
                rend.SetPropertyBlock(propBlock);
            }            
        }
    }
}