using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class OvenKnobMinigameController : MinigameController
{
    // References
    [SerializeField] private RectTransform _tempMarkerPivot;
    [SerializeField] private RectTransform _tempMarker;
    [SerializeField] private RectTransform _knob;
    [SerializeField] private RectTransform _pointer;
    [SerializeField] private Image _ovenFood;

    // Variables
    [SerializeField] private int _maxChecks;
    [SerializeField] private float _knobSpeed;
    [SerializeField] private float _knobSpeedMultiplier;
    private float _currentSpeed;
    private int _score;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(TimeCountdownDelay(_minigameTimerValue));

        EventManager.OnLaddleWasClickedEvent += CheckClick;

        RotateTempMark();
        _currentSpeed = _knobSpeed;
        _score = 0;

        _ovenFood.sprite = _currentRecipeSO.finishedRecipeSprite;
        _ovenFood.SetNativeSize();
    }

    protected override void OnDestroy()
    {
        EventManager.OnLaddleWasClickedEvent -= CheckClick;

        base.OnDestroy();
    }

    private void Update()
    {
        if (_isCountingDown)
            TimerCountdown(_minigameClock, _minigameTimerValue);

        RotatingKnob();
    }

    private void RotateTempMark()
    {
        float randomRotation = Random.Range(-180, 180);
        _tempMarkerPivot.transform.Rotate(new Vector3 (0, 0, randomRotation));
    }

    private void RotatingKnob()
    {
        _knob.transform.Rotate(0, 0, -_currentSpeed * Time.deltaTime);
    }

    private void AugmentSpeed()
    {
        _currentSpeed *= _knobSpeedMultiplier;
    }
    private void CheckClick()
    {
        if (IsOverlapping(_tempMarker, _pointer))
        {
            RotateTempMark();
            AugmentSpeed();
            IncreaseScore();
        }
    }

    private bool IsOverlapping(RectTransform area, RectTransform mark)
    {
        Vector2 areaPos = area.position;
        Vector2 markPos = mark.position;

        float distance = Vector2.Distance(areaPos, markPos);

        return distance <= 30f; // 30 is an example threshold in pixels
    }

    private void IncreaseScore()
    {
        _score += 1;
        if(_score >= _maxChecks)
        {
            EventManager.OnGameWinTrigger();
        }
    }


    private void OnDrawGizmos()
    {
        if (_tempMarker == null || _pointer == null)
            return;

        Vector3 markerPos = _tempMarker.position;
        Vector3 pointerPos = _pointer.position;

        // Draw marker position
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(markerPos, 5f); // Small green sphere

        // Draw pointer position
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(pointerPos, 5f); // Small red sphere

        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(markerPos, 30f); // 30 = threshold
    }
}
