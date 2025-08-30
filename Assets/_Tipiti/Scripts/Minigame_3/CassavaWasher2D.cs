// CassavaWasher2D.cs
using UnityEngine;

public class CassavaWasher2D : MonoBehaviour
{
    [Header("Camera & Movement")]
    [SerializeField] private Camera cam;                
    [SerializeField] private bool onlyWhenHeld = true;  
    [Tooltip("Smoothing time (seconds) used by SmoothDamp. Smaller = snappier.")]
    [SerializeField] private float smoothTime = 0.08f;  // e.g. 0.06 ~ 0.14
    [Tooltip("Hard cap on speed (units/sec) to avoid teleport when pointer jumps.")]
    [SerializeField] private float maxSpeed = 25f;

    [Header("Pick (optional)")]
    [Tooltip("Only start dragging if the click/touch hits this object (recommended).")]
    [SerializeField] private bool requireClickOnCassava = true;
    [Tooltip("Layer(s) used to detect click on cassava. Leave empty to use all.")]
    [SerializeField] private LayerMask pickMask = ~0;
    [Tooltip("Maintain pointer offset from the initial grab point.")]
    [SerializeField] private bool keepGrabOffset = true;

    [Header("Water (reference)")]
    [Tooltip("SpriteRenderer of the water that will become dirty.")]
    [SerializeField] private SpriteRenderer waterSprite;  
    [Tooltip("Optional precise area: if set, we check OverlapPoint against this collider.")]
    [SerializeField] private Collider2D waterArea;        
    [Tooltip("Fallback mask if waterArea is not provided.")]
    [SerializeField] private LayerMask waterMask = ~0;

    [Header("Water Material Blend")]
    [SerializeField] private string waterBlendProp = "_Blend"; // 0=clean, 1=dirty
    [SerializeField] private bool stopAtFullyDirty = true;

    [Header("Dirt Logic")]
    [SerializeField] private float minSpeedToDirty = 0.3f;  
    [SerializeField] private float dirtRatePerMeter = 0.5f; 

    // Runtime state
    private Vector3 _target;
    private Vector3 _lastPos;
    private bool _isDragging;
    private Vector3 _smoothVel;          // SmoothDamp velocity
    private Vector3 _grabOffsetWorld;    // pointer->object offset at grab

    private Material _waterMat;   
    private int _blendID = -1;

    /// Current dirt progress [0..1], mirrors water _Blend.
    public float DirtProgress
    {
        get
        {
            if (_waterMat == null || _blendID < 0) return 0f;
            return Mathf.Clamp01(_waterMat.GetFloat(_blendID));
        }
    }

    void Awake()
    {
        if (!cam) cam = Camera.main;

        // Validate and instance the water material
        if (!waterSprite)
        {
            Debug.LogError("[CassavaWasher2D] Missing 'waterSprite' reference.");
            enabled = false; return;
        }
        if (!waterSprite.sharedMaterial)
        {
            Debug.LogError("[CassavaWasher2D] 'waterSprite' has no material assigned.");
            enabled = false; return;
        }

        _waterMat = new Material(waterSprite.sharedMaterial);
        waterSprite.material = _waterMat;
        _blendID = Shader.PropertyToID(waterBlendProp);

        if (!_waterMat.HasProperty(_blendID))
            Debug.LogWarning($"[CassavaWasher2D] Water material has no '{waterBlendProp}' property.");

        _target = transform.position;
        _lastPos = transform.position;
    }

    void Update()
    {
        if (stopAtFullyDirty && DirtProgress >= 1f)
            return;

        // --- Pointer world position (mouse/touch) ---
        Vector3 sp = (Input.touchCount > 0) ? (Vector3)Input.GetTouch(0).position : Input.mousePosition;
        Vector3 wp = cam ? cam.ScreenToWorldPoint(sp) : sp;
        wp.z = transform.position.z;

        bool pressed  = Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
        bool down     = (Input.touchCount > 0) ? (Input.GetTouch(0).phase != TouchPhase.Ended && Input.GetTouch(0).phase != TouchPhase.Canceled)
                                               : Input.GetMouseButton(0);
        bool released = Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && 
                          (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled));

        // --- Begin/End drag (optional hit test) ---
        if (pressed)
        {
            bool canGrab = true;
            if (requireClickOnCassava)
            {
                var hit = Physics2D.OverlapPoint(wp, pickMask);
                canGrab = hit && (hit.transform == transform || hit.transform.IsChildOf(transform));
            }
            if (canGrab)
            {
                _isDragging = true;
                _smoothVel = Vector3.zero; // reset smoothing
                _grabOffsetWorld = keepGrabOffset ? (transform.position - wp) : Vector3.zero;
            }
        }
        if (released) _isDragging = false;

        // --- Choose/hold target ---
        if (onlyWhenHeld)
        {
            if (_isDragging) _target = wp + _grabOffsetWorld;
        }
        else
        {
            _target = wp + (keepGrabOffset ? _grabOffsetWorld : Vector3.zero);
        }

        // --- SmoothDamp movement (suave e estável) ---
        // Clamp max speed (avoid huge jumps when pointer teleports)
        Vector3 next = Vector3.SmoothDamp(transform.position, _target, ref _smoothVel, smoothTime, maxSpeed);

        transform.position = next;

        // --- Speed from visual motion (m/s) ---
        float speed = (transform.position - _lastPos).magnitude / Mathf.Max(Time.deltaTime, 1e-6f);
        _lastPos = transform.position;

        // --- Are we over water? ---
        bool overWater = false;
        Vector2 p2 = transform.position;

        if (waterArea) overWater = waterArea.OverlapPoint(p2);
        else           overWater = Physics2D.OverlapPoint(p2, waterMask) != null;

        // --- Dirty the water if moving fast enough over water ---
        if (_waterMat && _blendID >= 0 && overWater && speed >= minSpeedToDirty)
        {
            float blend = _waterMat.GetFloat(_blendID);
            float deltaDist = speed * Time.deltaTime; // meters moved this frame
            blend = Mathf.Clamp01(blend + deltaDist * dirtRatePerMeter);
            _waterMat.SetFloat(_blendID, blend);
        }
    }

    // Utility
    public void ResetWaterDirt() { if (_waterMat && _blendID >= 0) _waterMat.SetFloat(_blendID, 0f); }
}
