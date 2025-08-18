using UnityEngine;

public class PeelableCassava : MonoBehaviour
{
    [Header("Materials")]
    public Renderer targetRenderer;
    public Material cassavaMat;       // instance
    public Texture baseTex;
    public Texture peeledTex;

    [Header("Mask")]
    public int maskWidth = 512;
    public int maskHeight = 512;
    public RenderTexture peelMaskRT;  // runtime created

    void Awake()
    {
        if (!targetRenderer) targetRenderer = GetComponent<Renderer>();
        cassavaMat = Instantiate(targetRenderer.sharedMaterial);
        targetRenderer.material = cassavaMat;

        // peelMaskRT = new RenderTexture(maskWidth, maskHeight, 0, RenderTextureFormat.ARGB32);
        // peelMaskRT = new RenderTexture(maskWidth, maskHeight, 0, RenderTextureFormat.R8);
        // peelMaskRT.filterMode = FilterMode.Bilinear;
        // peelMaskRT.Create();

        // Clear to black (no peel)
        var active = RenderTexture.active;
        RenderTexture.active = peelMaskRT;
        GL.Clear(false, true, Color.black);
        RenderTexture.active = active;

        cassavaMat.SetTexture("_PeelMask", peelMaskRT);
        if (baseTex)   cassavaMat.SetTexture("_BaseTex", baseTex);
        if (peeledTex) cassavaMat.SetTexture("_PeeledTex", peeledTex);
    }
}
