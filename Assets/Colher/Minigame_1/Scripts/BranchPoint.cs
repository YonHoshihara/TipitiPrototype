using System;
using UnityEngine;
using UnityEngine.Events;

public class BranchPoint : MonoBehaviour
{
    public UnityEvent<BranchPoint> OnBroken;

    [Header("Feedback")]
    public GameObject breakVfxPrefab;
    public AudioSource sfx; // optional
    public Renderer targetRenderer;
    public Color brokenTint = new Color(0.9f, 0.8f, 0.6f, 1f);

    private bool _isBroken = false;

    void OnMouseDown()
    {
        Debug.Log($"BranchPoint clicked: {name}");
        // Simple PC click; for mobile, use Physics raycast from touch in a separate input manager if needed
        TryBreak();
    }

    public void TryBreak()
    {
        if (_isBroken) return;
        _isBroken = true;

        // VFX/SFX
        if (breakVfxPrefab)
        {
            var v = Instantiate(breakVfxPrefab, transform.position, Quaternion.identity);
            Destroy(v, 2f);
        }
        if (sfx) sfx.Play();

        if (targetRenderer) targetRenderer.material.color = brokenTint;

        // Disable collider to avoid re-clicks
        var col = GetComponent<Collider>();
        if (col) col.enabled = false;
        var col2d = GetComponent<Collider2D>();
        if (col2d) col2d.enabled = false;

        OnBroken?.Invoke(this);
    }
}
