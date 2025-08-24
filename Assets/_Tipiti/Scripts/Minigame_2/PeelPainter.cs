using UnityEngine;

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
    [SerializeField] private bool eraseMode = false; // hold a key to toggle erase if wanted

    [Header("Penalty / Feel")]
    [SerializeField] private float maxAngleDegrees = 55f; // too steep angle = bad
    [SerializeField] private float minDragSpeed = 0.02f;  // prevents dotting too slow
    [SerializeField] private float penaltyStrength = 0.5f; // reduce paint when wrong

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
        {
            TryPaint(screenPos);
        }
        else
        {
            _hadLast = false;
        }
    }

    private void TryPaint(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, 2_000f, cassavaMask))
        {
            // Debug.Log($"Hit: {hit.collider.name}, uv={hit.textureCoord}");
            if (!hit.collider || hit.collider.GetComponent<PeelableCassava>() != cassava)
                return;

            Vector2 uv = hit.textureCoord;
            // Drag speed in screen space (normalized by minDragSpeed)
            float dragSpeed = _hadLast ? (screenPos - _lastScreen).magnitude / Screen.dpi : 0f;
            _lastScreen = screenPos;

            // Surface angle penalty (simulate "too steep" blade angle)
            float angle = Vector3.Angle(hit.normal, -ray.direction);
            bool badAngle = angle > maxAngleDegrees;

            float strength = brushStrength;

            // If drag too slow, reduce strength to avoid dotting/cheese
            if (_hadLast && dragSpeed < minDragSpeed) strength *= 0.25f;
            if (badAngle) strength *= penaltyStrength;

            Stamp(uv, strength, eraseMode ? 1f : 0f);

            _hadLast = true;
        }
    }

    private void Stamp(Vector2 uv, float strength, float erase)
    {
        if (!cassava || !cassava.peelMaskRT || !brushMat) return;

        brushMat.SetVector("_BrushPos", new Vector4(uv.x, uv.y, 0, 0));
        brushMat.SetFloat("_BrushRadius", brushRadiusUV);
        brushMat.SetFloat("_BrushHardness", brushHardness);
        brushMat.SetFloat("_BrushStrength", Mathf.Clamp01(strength));
        brushMat.SetFloat("_Erase", erase);

        var src = cassava.peelMaskRT;
        // Use a temporary RT to avoid reading & writing the same target at once
        var tmp = RenderTexture.GetTemporary(src.width, src.height, 0, src.format);
        Graphics.Blit(src, tmp);                 // copy current mask to tmp
        Graphics.Blit(tmp, src, brushMat);       // draw brush using tmp as _MainTex into src
        RenderTexture.ReleaseTemporary(tmp);
    }
}
