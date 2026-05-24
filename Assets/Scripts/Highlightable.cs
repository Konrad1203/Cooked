using UnityEngine;

public class Highlightable : MonoBehaviour
{
    private Color highlightColor;
    private Renderer[] renderers;
    private MaterialPropertyBlock propBlock;
    private static readonly int EmissionColorProp = Shader.PropertyToID("_EmissionColor");


    void Awake() {
        propBlock = new MaterialPropertyBlock();
        RefreshRenderers();
        foreach (var rend in renderers) {
            foreach (Material mat in rend.materials) {
                if (!mat.IsKeywordEnabled("_EMISSION"))
                    mat.EnableKeyword("_EMISSION");
            }
        }
    }

    private void Start() {
        highlightColor = DataManager.Instance.highlightColor;
    }

    public void RefreshRenderers() {
        renderers = GetComponentsInChildren<Renderer>();
    }

    public void Highlight(bool active) {
        foreach (Renderer rend in renderers) {
            if (rend == null) continue;
            rend.GetPropertyBlock(propBlock);
            Color eColor = active ? highlightColor : Color.black;
            propBlock.SetColor(EmissionColorProp, eColor);
            rend.SetPropertyBlock(propBlock);
        }
    }
}