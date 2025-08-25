using UnityEngine;

public class PeelPainter : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private Camera cam;
    [SerializeField] private PeelableCassava cassava;
    [SerializeField] private LayerMask cassavaMask;

    [Header("Brush")]
    [SerializeField] private Material brushMat;  // uses "Hidden/PeelBrushStamp"
    [Range(0.005f, 0.2f)][SerializeField] private float brushRadiusUV = 0.04f;
    [Range(0f, 1f)][SerializeField] private float brushHardness = 0.7f;
    [Range(0f, 1f)][SerializeField] private float brushStrength = 1.0f;
    [SerializeField] private bool eraseMode = false; // hold a key to toggle erase if wanted

    [Header("Penalty / Feel")]
    [SerializeField] private float maxAngleDegrees = 55f; // too steep angle = bad
    [SerializeField] private float minDragSpeed = 0.02f;  // prevents dotting too slow (inches-ish per frame via Screen.dpi)
    [SerializeField] private float penaltyStrength = 0.5f; // reduce paint when wrong

    [Header("FX (optional)")]
    [SerializeField] private ParticleSystem peelParticles;

    [Header("Audio (simple grains)")]
    [Tooltip("Single AudioSource used for PlayOneShot grains (not looping).")]
    [SerializeField] private AudioSource scrapeSource;
    [Tooltip("Short scrape variations (100–300 ms).")]
    [SerializeField] private AudioClip[] scrapeClips;
    [Range(0.0f, 0.25f)][SerializeField] private float pitchVar = 0.08f;     // ± pitch randomization
    [SerializeField] private float minInterval = 0.06f;                       // cooldown between grains
    [SerializeField] private float speedThreshold = 0.15f;                    // minimum normalized speed to trigger a grain
    [SerializeField] private float speedForMax = 0.12f;                       // dragSpeed at which normSpeed ~= 1

    private Vector2 _lastScreen;
    private bool _hadLast;
    private float _scrapeCooldown = 0f;
    private int _lastIndex = -1;

    private void Update()
    {
        if (cam == null) cam = Camera.main;

        // Cooldown decrement for audio grains
        if (_scrapeCooldown > 0f) _scrapeCooldown -= Time.deltaTime;

        bool inputDown = Input.GetMouseButton(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase != TouchPhase.Ended);
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

        if (Physics.Raycast(ray, out RaycastHit hit, 2000f, cassavaMask))
        {
            // Accept component on parent to avoid strict same-object requirement
            var targetCassava = hit.collider.GetComponentInParent<PeelableCassava>();
            if (targetCassava != cassava) return;

            Vector2 uv = hit.textureCoord;

            // Drag speed in screen space (inches-ish per frame via DPI); normalize to 0..1
            float dragSpeed = _hadLast ? (screenPos - _lastScreen).magnitude / Mathf.Max(1f, Screen.dpi) : 0f;
            float normSpeed = Mathf.InverseLerp(minDragSpeed, speedForMax, dragSpeed); // 0..1
            _lastScreen = screenPos;

            // Surface angle penalty (simulate "too steep" blade angle)
            float angle = Vector3.Angle(hit.normal, -ray.direction);
            bool badAngle = angle > maxAngleDegrees;

            float strength = brushStrength;

            // If drag too slow, reduce strength to avoid dotting/cheese
            if (_hadLast && dragSpeed < minDragSpeed) strength *= 0.25f;
            if (badAngle) strength *= penaltyStrength;

            Stamp(uv, strength, eraseMode ? 1f : 0f);

            // Extra feedback (particles)
            if (peelParticles) peelParticles.Play();

            // Simple audio grain with variation
            PlayScrapeGrain(normSpeed);

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

    // --- Simple audio grain player with anti-repetition, pitch jitter and cooldown ---
    private void PlayScrapeGrain(float normSpeed)
    {
        // Require minimal setup and motion
        if (!scrapeSource || scrapeClips == null || scrapeClips.Length == 0) return;
        if (normSpeed < speedThreshold) return;
        if (_scrapeCooldown > 0f) return;

        // Pick a clip different from the last one
        int idx;
        if (scrapeClips.Length == 1)
        {
            idx = 0;
        }
        else
        {
            do { idx = Random.Range(0, scrapeClips.Length); }
            while (idx == _lastIndex);
        }
        _lastIndex = idx;

        // Light pitch jitter + subtle dependence on speed
        float jitter = (Random.value * 2f - 1f) * pitchVar; // [-pitchVar, +pitchVar]
        scrapeSource.pitch = 1f + jitter + (normSpeed * 0.05f);

        // Volume scales with speed
        float vol = Mathf.Lerp(0.35f, 0.9f, Mathf.Clamp01(normSpeed));
        scrapeSource.PlayOneShot(scrapeClips[idx], vol);

        // Reset cooldown
        _scrapeCooldown = Mathf.Max(0.01f, minInterval);
    }
}
