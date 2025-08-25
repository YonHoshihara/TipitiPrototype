using System.Collections;
using UnityEngine;
using TMPro;

public class PeelingMinigame : MonoBehaviour
{
    [SerializeField] private PeelableCassava cassava;
    [SerializeField] private PeelPainter painter;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private TextMeshProUGUI promptText;

    [Header("Game Rules")]
    [Range(0, 100)][SerializeField] private float targetPercent = 85f;
    [SerializeField] private float maxTime = 0f;
    [SerializeField] private bool failIfOvertime = false;

    [Header("Sampling")]
    [SerializeField] private int sampleSize = 64;
    [SerializeField] private float sampleInterval = 0.5f;

    [Header("Visibility")]
    [Tooltip("Optional: visibility mask (R8) saying which UVs are visible (1) vs transparent (0). " +
            "If null, the whole area counts.")]
    [SerializeField] private Texture2D visibilityMask;

    private float _timer;
    private float _sampleClock;
    private Texture2D _readMask;
    private Texture2D _readVis; // downsampled visibility
    private bool _ended;
    private int _lastProgressStep = 0;

    private void Start()
    {
        _timer = maxTime;
        _readMask = new Texture2D(sampleSize, sampleSize, TextureFormat.R8, false, true);
        _readVis = new Texture2D(sampleSize, sampleSize, TextureFormat.R8, false, true);

        if (promptText) promptText.text = "Descasque a mandioca: arraste o descascador pela superfície!";
    }

    private void Update()
    {
        if (_ended) return;

        if (maxTime > 0f)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f && failIfOvertime)
            {
                End(false, "Tempo esgotado.");
                return;
            }
        }

        _sampleClock -= Time.deltaTime;
        if (_sampleClock <= 0f)
        {
            _sampleClock = sampleInterval;
            float p = EstimatePercentWeighted();

            int step = Mathf.FloorToInt(p / 1f); // de 0 a 10
            if (progressText) progressText.text = $"{p:0}%";

            if (step > _lastProgressStep)
            {
                _lastProgressStep = step;
                // feedback
                if (progressText) StartCoroutine(PulseText(progressText));
                // pode tocar um "ting"
            }

            if (p >= targetPercent)
                End(true, "Descascado o suficiente!");
        }
    }

    private float EstimatePercentWeighted()
    {
        if (!cassava || !cassava.peelMaskRT) return 0f;

        // 1) Downsample PeelMask to CPU
        RenderTexture active = RenderTexture.active;
        RenderTexture tmp = RenderTexture.GetTemporary(sampleSize, sampleSize, 0, RenderTextureFormat.R8);
        Graphics.Blit(cassava.peelMaskRT, tmp);
        RenderTexture.active = tmp;
        _readMask.ReadPixels(new Rect(0, 0, sampleSize, sampleSize), 0, 0);
        _readMask.Apply(false, false);
        RenderTexture.active = active;
        RenderTexture.ReleaseTemporary(tmp);

        // 2) Prepare downsampled visibility (if provided), else assume full-ones
        byte[] visData;
        if (visibilityMask != null)
        {
            // Downsample visibilityMask into _readVis using Graphics.Blit for speed (needs it as source Texture)
            // If visibilityMask is NPOT it's fine; GPU will sample.
            RenderTexture rtVis = RenderTexture.GetTemporary(sampleSize, sampleSize, 0, RenderTextureFormat.R8);
            Graphics.Blit(visibilityMask, rtVis);
            active = RenderTexture.active;
            RenderTexture.active = rtVis;
            _readVis.ReadPixels(new Rect(0, 0, sampleSize, sampleSize), 0, 0);
            _readVis.Apply(false, false);
            RenderTexture.active = active;
            RenderTexture.ReleaseTemporary(rtVis);

            visData = _readVis.GetRawTextureData<byte>().ToArray();
        }
        else
        {
            visData = null; // we'll treat as all-ones
        }

        // 3) Weighted percentage
        var maskData = _readMask.GetRawTextureData<byte>();
        long visSum = 0;
        long peeledSum = 0;

        if (visData != null)
        {
            for (int i = 0; i < maskData.Length; i++)
            {
                int v = visData[i];          // 0..255
                if (v <= 0) continue;        // ignore invisible
                visSum += v;
                peeledSum += (maskData[i] * v); // both 0..255 -> 0..~65025
            }
        }
        else
        {
            // No visibility mask: everything counts equally
            for (int i = 0; i < maskData.Length; i++)
            {
                visSum += 255;
                peeledSum += maskData[i] * 255;
            }
        }

        if (visSum <= 0) return 0f;

        // Normalize: peeled in [0..255*visSum], convert to 0..1
        double norm = (double)peeledSum / (double)(255L * visSum);
        return (float)(norm * 100.0);
    }

    private void End(bool success, string msg)
    {
        _ended = true;
        if (promptText) promptText.text = success ? $"Sucesso! {msg}" : $"Falhou: {msg}";
        if (painter) painter.enabled = false;
    }

    private IEnumerator PulseText(TextMeshProUGUI txt)
    {
        Vector3 baseScale = txt.rectTransform.localScale;
        Vector3 target = baseScale * 1.2f;
        float t = 0f;
        while (t < 0.2f)
        {
            t += Time.deltaTime;
            txt.rectTransform.localScale = Vector3.Lerp(baseScale, target, t / 0.2f);
            yield return null;
        }
        txt.rectTransform.localScale = baseScale;
    }

    // Helper to build visibility from a Sprite at runtime (chame após setar o sprite)
    // public void BuildVisibilityFromSprite(Sprite sprite, int targetSize = 512, float alphaThreshold = 0.01f)
    // {
    //     visibilityMask = SpriteVisibilityMask.Build(sprite, targetSize, targetSize, alphaThreshold);
    // }
}
