using System;
using UnityEngine;

public class RootPuller : MonoBehaviour
{
    [Header("Tension Model")]
    public float tension = 0f;           // current [0..tensionToRelease]
    public float tensionToRelease = 100f;
    public float decayPerSecond = 10f;   // tension decay if cadence is off
    public float gainPerGoodAlt = 12f;   // tension gained per correct alternation
    public float minInterval = 0.08f;    // too fast if below
    public float maxInterval = 0.45f;    // too slow if above

    [Header("Root Transform Feedback")]
    public Transform rootVisual;
    public Vector3 startPos;
    public Vector3 pulledPos; // where it ends when released
    public AnimationCurve pullCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Audio/FX")]
    public AudioSource goodAltSfx;
    public AudioSource strainSfx;
    public GameObject dustVfxPrefab;

    public float NormalizedTension => Mathf.Clamp01(tension / tensionToRelease);

    private bool _isPulling = false;
    private bool _expectLeft = true; // expects 'A' first; alternates to 'D'
    private float _lastHitTime = -999f;

    private Action _onComplete;
    private Action<string> _onFail;

    void Awake()
    {
        if (rootVisual != null) startPos = rootVisual.localPosition;
    }

    public void BeginPulling(Action onComplete, Action<string> onFail)
    {
        _isPulling = true;
        _onComplete = onComplete;
        _onFail = onFail;
        _expectLeft = true;
        _lastHitTime = -999f;
        tension = 0f;
    }

    public void EndPulling()
    {
        _isPulling = false;
    }

    void Update()
    {
        if (!_isPulling) return;

        // PC input
        bool left = Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
        bool right = Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);

        // Mobile (very simple swipe detection sketch)
        // You can replace with a proper gesture system later.
        if (Input.touchCount > 0)
        {
            var t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Ended)
            {
                Vector2 delta = t.position - (t.position - t.deltaPosition);
                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                {
                    if (delta.x < 0) left = true;
                    else right = true;
                }
            }
        }

        // Handle alternation & cadence
        bool pressed = left || right;
        if (pressed)
        {
            bool correctSide = (_expectLeft && left) || (!_expectLeft && right);
            float now = Time.time;
            float interval = now - _lastHitTime;

            if (correctSide)
            {
                // cadence window check (ignore first hit)
                if (_lastHitTime < 0f || (interval >= minInterval && interval <= maxInterval))
                {
                    tension += gainPerGoodAlt;
                    if (goodAltSfx) goodAltSfx.Play();
                    if (dustVfxPrefab) SpawnDust();

                    _expectLeft = !_expectLeft;
                    _lastHitTime = now;
                }
                else
                {
                    // off-cadence penalty: light decay tick
                    tension = Mathf.Max(0f, tension - decayPerSecond * 0.5f);
                    _lastHitTime = now;
                }
            }
            else
            {
                // wrong key: small penalty
                tension = Mathf.Max(0f, tension - decayPerSecond);
                if (strainSfx) strainSfx.Play();
                // Do not flip expectation on wrong key
                _lastHitTime = now;
            }
        }

        // Passive decay
        if (!pressed)
            tension = Mathf.Max(0f, tension - decayPerSecond * Time.deltaTime);

        // Visual feedback (root moving up based on normalized tension)
        if (rootVisual)
        {
            float t = NormalizedTension;
            rootVisual.localPosition = Vector3.Lerp(startPos, pulledPos, pullCurve.Evaluate(t));
        }

        // Success condition
        if (tension >= tensionToRelease)
        {
            _isPulling = false;
            _onComplete?.Invoke();
        }
    }

    void SpawnDust()
    {
        if (!dustVfxPrefab || !rootVisual) return;
        var v = Instantiate(dustVfxPrefab, rootVisual.position, Quaternion.identity);
        Destroy(v, 2f);
    }
}
