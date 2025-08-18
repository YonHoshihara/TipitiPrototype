using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HarvestCassavaMinigame : MonoBehaviour
{
    public enum State { BranchBreaking, PullingRoot, Success, Fail }

    [Header("Flow")]
    public State current = State.BranchBreaking;
    public List<BranchPoint> branchPoints;
    public RootPuller rootPuller;

    [Header("UI")]
    public TextMeshProUGUI promptText;
    public TextMeshProUGUI branchCounterText;
    public Slider tensionSlider;

    [Header("Tuning")]
    public float branchTimeLimit = 20f;   // opcional: limite de tempo p/ fase A (0 = sem limite)
    public bool failIfTimeRunsOut = false;

    private float _branchTimer;
    private int _totalBranches;
    private int _brokenBranches;

    void Start()
    {
        // Validate refs
        if (branchPoints == null || branchPoints.Count == 0)
            branchPoints = new List<BranchPoint>(GetComponentsInChildren<BranchPoint>());
        _totalBranches = branchPoints.Count;

        foreach (var bp in branchPoints)
        {
            bp.OnBroken.AddListener(HandleBranchBroken);
        }

        // Init UI
        promptText.text = "Quebre os galhos soltos (clique/toque)!";
        UpdateBranchUI();

        _branchTimer = branchTimeLimit;
        // Hook root UI
        // rootPuller.gameObject.SetActive(false);
        if (tensionSlider) tensionSlider.minValue = 0f;
        if (tensionSlider) tensionSlider.maxValue = 1f;
    }

    private void OnDestroy()
    {
        // Unhook events
        foreach (var bp in branchPoints)
        {
            bp.OnBroken.RemoveListener(HandleBranchBroken);
        }
    }

    void Update()
    {
        switch (current)
        {
            case State.BranchBreaking:
                UpdateBranchPhase();
                break;

            case State.PullingRoot:
                UpdatePullPhaseUI();
                break;

            case State.Success:
            case State.Fail:
                // Optionally wait or signal game manager
                break;
        }
    }

    void UpdateBranchPhase()
    {
        // Optional timer for pressure
        if (branchTimeLimit > 0f)
        {
            _branchTimer -= Time.deltaTime;
            if (_branchTimer <= 0f && failIfTimeRunsOut)
            {
                SetFail("Você demorou demais para preparar a planta.");
            }
        }

        if (_brokenBranches >= _totalBranches)
        {
            // Transition to pulling
            current = State.PullingRoot;
            promptText.text = "Puxe a raiz alternando A/D (ou swipes)! Mantenha a cadência.";
            rootPuller.gameObject.SetActive(true);
            rootPuller.BeginPulling(OnPullComplete, OnPullFail);
        }
    }

    void UpdatePullPhaseUI()
    {
        // Sync tension bar
        if (tensionSlider) tensionSlider.value = rootPuller.NormalizedTension;
    }

    void HandleBranchBroken(BranchPoint bp)
    {
        _brokenBranches++;
        UpdateBranchUI();
    }

    void UpdateBranchUI()
    {
        if (branchCounterText)
            branchCounterText.text = $"{_brokenBranches}/{_totalBranches}";
    }

    void OnPullComplete()
    {
        current = State.Success;
        promptText.text = "Sucesso! A raiz saiu inteira. 🎉";
        // TODO: Dispatch event to higher-level game flow
    }

    void OnPullFail(string reason)
    {
        SetFail(reason);
    }

    void SetFail(string reason)
    {
        current = State.Fail;
        promptText.text = $"Falhou: {reason}";
        rootPuller.EndPulling();
    }
}
