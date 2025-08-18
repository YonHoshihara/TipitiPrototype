using UnityEngine;
using TMPro;

public class PeelingMinigame : MonoBehaviour
{
    public PeelableCassava cassava;
    public PeelPainter painter;

    [Header("UI")]
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI promptText;

    [Header("Game Rules")]
    [Range(0,100)] public float targetPercent = 85f;
    public float maxTime = 45f; // 0 = sem limite
    public bool failIfOvertime = false;

    [Header("Sampling")]
    public int sampleSize = 64; // downsample to estimate peeled %
    public float sampleInterval = 0.5f;

    private float _timer;
    private float _sampleClock;
    private Texture2D _readback;
    private bool _ended = false;

    void Start()
    {
        _timer = maxTime;
        _readback = new Texture2D(sampleSize, sampleSize, TextureFormat.R8, false, true);
        if (promptText) promptText.text = "Descasque a mandioca: arraste o descascador pela superfície!";
    }

    void Update()
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
            float p = EstimatePercent();
            if (progressText) progressText.text = $"{p:0}%";

            if (p >= targetPercent)
            {
                End(true, "Descascado o suficiente!");
            }
        }
    }

    float EstimatePercent()
    {
        if (!cassava || !cassava.peelMaskRT) return 0f;

        // Read a downsampled copy to CPU
        RenderTexture active = RenderTexture.active;
        RenderTexture tmp = RenderTexture.GetTemporary(sampleSize, sampleSize, 0, RenderTextureFormat.R8);
        Graphics.Blit(cassava.peelMaskRT, tmp);
        RenderTexture.active = tmp;
        _readback.ReadPixels(new Rect(0,0,sampleSize,sampleSize), 0, 0);
        _readback.Apply(false, false);
        RenderTexture.active = active;
        RenderTexture.ReleaseTemporary(tmp);

        // Count white pixels (peeled)
        var data = _readback.GetRawTextureData<byte>();
        int white = 0;
        for (int i = 0; i < data.Length; i++)
            if (data[i] > 127) white++;

        float percent = (white * 100f) / data.Length;
        Debug.Log($"Percent: {percent}");

        return percent;
    }

    void End(bool success, string msg)
    {
        _ended = true;
        if (promptText) promptText.text = success ? $"Sucesso! {msg}" : $"Falhou: {msg}";
        // Aqui você pode disparar evento para o flow do jogo
        // e/ou bloquear o input do painter
        painter.enabled = success ? false : false;
    }
}
