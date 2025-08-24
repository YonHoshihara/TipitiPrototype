using UnityEngine;

public class PeelableCassava : MonoBehaviour
{
    [Header("Materials")]
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private Material cassavaMat; // instance
    [SerializeField] private Texture baseTex;
    [SerializeField] private Texture peeledTex;

    [Header("Mask")]
    [SerializeField] private int maskWidth = 512;
    [SerializeField] private int maskHeight = 512;
    [SerializeField] private bool runtimeCreateMask = false;
    public RenderTexture peelMaskRT; // runtime created

    private void Awake()
    {
        if (!targetRenderer) targetRenderer = GetComponent<Renderer>();
        cassavaMat = Instantiate(targetRenderer.sharedMaterial);
        targetRenderer.material = cassavaMat;

        if (runtimeCreateMask || peelMaskRT == null)
        {
            peelMaskRT = new RenderTexture(maskWidth, maskHeight, 0, RenderTextureFormat.ARGB32);
            peelMaskRT = new RenderTexture(maskWidth, maskHeight, 0, RenderTextureFormat.R8);
            peelMaskRT.filterMode = FilterMode.Bilinear;
            peelMaskRT.Create();
        }

        // Clear to black (no peel)
        var active = RenderTexture.active;
        RenderTexture.active = peelMaskRT;
        GL.Clear(false, true, Color.black);
        RenderTexture.active = active;

        cassavaMat.SetTexture("_PeelMask", peelMaskRT);
        if (baseTex) cassavaMat.SetTexture("_BaseTex", baseTex);
        if (peeledTex) cassavaMat.SetTexture("_PeeledTex", peeledTex);
    }
    
    private void OnDestroy()
    {
        // Clear to black (no peel)
        var active = RenderTexture.active;
        RenderTexture.active = peelMaskRT;
        GL.Clear(false, true, Color.black);
        RenderTexture.active = active;
    }
}
