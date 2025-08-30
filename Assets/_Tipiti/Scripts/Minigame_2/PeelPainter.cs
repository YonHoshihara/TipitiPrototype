using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class StampEvent : UnityEvent<Vector3, Vector3, float> {} // pos, normal, strength

public class PeelPainter : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private Camera cam;
    [SerializeField] private PeelableCassava cassava;
    [SerializeField] private LayerMask cassavaMask;

    [Header("Brush")]
    [SerializeField] private Material brushMat;  // uses "Hidden/PeelBrushStamp"
    [Range(0.005f, 0.2f)] [SerializeField] private float brushRadiusUV = 0.04f;
    [Range(0f, 1f)] [SerializeField] private float brushHardness = 0.7f;
    [Range(0f, 1f)] [SerializeField] private float brushStrength = 1.0f;
    [SerializeField] private bool eraseMode = false;

    [Header("Penalty / Feel")]
    [SerializeField] private float maxAngleDegrees = 55f;
    [SerializeField] private float minDragSpeed = 0.02f;
    [SerializeField] private float penaltyStrength = 0.5f;

    [Header("Events")]
    public StampEvent OnStampWorld; // <-- particles listen here

    private Vector2 _lastScreen;
    private bool _hadLast;

    private void Update()
    {
        if (cam == null) cam = Camera.main;

        bool inputDown = Input.GetMouseButton(0);
        Vector2 screenPos = (Input.touchCount > 0)
            ? (Vector2)Input.GetTouch(0).position
            : (Vector2)Input.mousePosition;

        if (inputDown)
            TryPaint(screenPos);
        else
            _hadLast = false;
    }

    private void TryPaint(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, 2000f, cassavaMask))
        {
            if (!hit.collider || hit.collider.GetComponent<PeelableCassava>() != cassava)
                return;

            Vector2 uv = hit.textureCoord;

            // Drag speed in screen space
            float dragSpeed = _hadLast ? (screenPos - _lastScreen).magnitude / Screen.dpi : 0f;
            _lastScreen = screenPos;

            // Angle penalty
            float angle = Vector3.Angle(hit.normal, -ray.direction);
            bool badAngle = angle > maxAngleDegrees;

            float strength = brushStrength;
            if (_hadLast && dragSpeed < minDragSpeed) strength *= 0.25f;
            if (badAngle) strength *= penaltyStrength;

            // Stamp and only emit particles if peel actually increased
            bool changed = StampAndCheckDelta(uv, strength, eraseMode ? 1f : 0f);

            if (changed && OnStampWorld != null)
            {
                // Emit particles at world pos, along surface normal
                OnStampWorld.Invoke(hit.point, hit.normal, Mathf.Clamp01(strength));
            }

            _hadLast = true;
        }
    }

    /// Stamps the brush into the peel mask and returns true if "peeled" value increased nearby.
    private bool StampAndCheckDelta(Vector2 uv, float strength, float erase)
    {
        if (!cassava || !cassava.peelMaskRT || !brushMat) return false;

        var src = cassava.peelMaskRT;

        // Copy BEFORE state
        var before = RenderTexture.GetTemporary(src.width, src.height, 0, src.format);
        Graphics.Blit(src, before);

        // Do the stamp
        brushMat.SetVector("_BrushPos", new Vector4(uv.x, uv.y, 0, 0));
        brushMat.SetFloat("_BrushRadius", brushRadiusUV);
        brushMat.SetFloat("_BrushHardness", brushHardness);
        brushMat.SetFloat("_BrushStrength", Mathf.Clamp01(strength));
        brushMat.SetFloat("_Erase", erase);
        Graphics.Blit(before, src, brushMat);

        // Check local delta around the brush (downsample tiny area)
        bool increased = CheckLocalIncrease(before, src, uv);

        RenderTexture.ReleaseTemporary(before);
        return increased;
    }

    /// Reads a very small window around UV and returns true if AFTER > BEFORE somewhere.
    private bool CheckLocalIncrease(RenderTexture before, RenderTexture after, Vector2 uv)
    {
        // Window size in texels around the brush center (clamped)
        int texW = after.width;
        int texH = after.height;
        int rad = Mathf.Max(2, Mathf.RoundToInt(brushRadiusUV * Mathf.Min(texW, texH) * 0.6f));
        int cx = Mathf.Clamp(Mathf.RoundToInt(uv.x * texW), 0, texW - 1);
        int cy = Mathf.Clamp(Mathf.RoundToInt(uv.y * texH), 0, texH - 1);

        int x0 = Mathf.Clamp(cx - rad, 0, texW - 1);
        int y0 = Mathf.Clamp(cy - rad, 0, texH - 1);
        int w = Mathf.Clamp(cx + rad, 0, texW - 1) - x0 + 1;
        int h = Mathf.Clamp(cy + rad, 0, texH - 1) - y0 + 1;

        // Read tiny rectangles (convert to very small 8x8 to minimize CPU read cost)
        const int kCheck = 8;
        var smallBefore = new Texture2D(kCheck, kCheck, TextureFormat.R8, false, true);
        var smallAfter  = new Texture2D(kCheck, kCheck, TextureFormat.R8, false, true);

        // Blit subrect -> temp RT -> ReadPixels -> 8x8
        bool increased = false;

        RenderTexture subBefore = RenderTexture.GetTemporary(w, h, 0, before.format);
        RenderTexture subAfter  = RenderTexture.GetTemporary(w, h, 0, after.format);

        // Copy the sub-rects
        Graphics.CopyTexture(before, 0, 0, x0, y0, w, h, subBefore, 0, 0, 0, 0);
        Graphics.CopyTexture(after,  0, 0, x0, y0, w, h, subAfter,  0, 0, 0, 0);

        // Downscale to 8x8 and read
        RenderTexture downB = RenderTexture.GetTemporary(kCheck, kCheck, 0, before.format);
        RenderTexture downA = RenderTexture.GetTemporary(kCheck, kCheck, 0, after.format);
        Graphics.Blit(subBefore, downB);
        Graphics.Blit(subAfter,  downA);

        var active = RenderTexture.active;

        RenderTexture.active = downB;
        smallBefore.ReadPixels(new Rect(0, 0, kCheck, kCheck), 0, 0);
        smallBefore.Apply(false, false);

        RenderTexture.active = downA;
        smallAfter.ReadPixels(new Rect(0, 0, kCheck, kCheck), 0, 0);
        smallAfter.Apply(false, false);

        RenderTexture.active = active;

        var b = smallBefore.GetRawTextureData<byte>();
        var a = smallAfter.GetRawTextureData<byte>();
        for (int i = 0; i < b.Length; i++)
        {
            if (a[i] > b[i]) { increased = true; break; }
        }

        RenderTexture.ReleaseTemporary(subBefore);
        RenderTexture.ReleaseTemporary(subAfter);
        RenderTexture.ReleaseTemporary(downB);
        RenderTexture.ReleaseTemporary(downA);

        return increased;
    }
}
