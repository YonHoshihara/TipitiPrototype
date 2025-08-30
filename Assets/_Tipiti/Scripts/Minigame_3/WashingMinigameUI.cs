using UnityEngine;
using TMPro;

public class WashingMinigameUI : MonoBehaviour
{
    [SerializeField] private CassavaWasher2D washer;
    [SerializeField] private float target = 0.9f;
    [SerializeField] private TextMeshProUGUI percentLabel;

    void Update()
    {
        if (!washer) return;
        float p = washer.DirtProgress;
        if (percentLabel) percentLabel.text = $"{Mathf.RoundToInt(p * 100f)}%";

        if (p >= target)
        {
            // TODO: success flow (desabilitar input, trocar de fase, etc.)
            enabled = false;
        }
    }
}
