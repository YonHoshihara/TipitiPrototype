using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CookingMinigameController : MinigameController
{
    [Header("Cooking Minigame References")]
    [SerializeField] private RectTransform _markerBar;
    [SerializeField] private RectTransform _hitBar;
    [SerializeField] private RectTransform _marker;
    [SerializeField] private GameObject _potBottom;
    [SerializeField] private Animator _laddleAnim;

    [Header("Cooking Minigame Variables")]
    [SerializeField] private float _markerSpeed;
    private bool _isMovingRight = true;
    private float _barWidth;
    private bool _isDraggingIngredient;

    protected override void Start()
    {
        base.Start();

        EventManager.OnChangeCookingGameMecanicEvent += ChangeGameplayMode;
        EventManager.OnLaddleWasClickedEvent += CheckClick;

        StartCoroutine(TimeCountdownDelay(_minigameTimerValue));
        GenerateIngredientsInPlates();

        _barWidth = _markerBar.sizeDelta.x;
        // game starts with dragging mecanic
        _isDraggingIngredient = true;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        EventManager.OnChangeCookingGameMecanicEvent -= ChangeGameplayMode;
        EventManager.OnLaddleWasClickedEvent -= CheckClick;
    }

    private void Update()
    {
        if(_isCountingDown)
            TimerCountdown(_minigameClock, _minigameTimerValue);

        if(!_isDraggingIngredient)
        {
            MoveMarker();
        }
    }

    private void MoveMarker()
    {
        // Calculate marker movement
        float delta = _markerSpeed * Time.deltaTime;
        float newX = _marker.anchoredPosition.x + (_isMovingRight ? delta : -delta);

        // Clamp the marker within the bar's boundaries
        if (newX > _barWidth / 2)
        {
            newX = _barWidth / 2;
            _isMovingRight = false;
        }
        else if (newX < -_barWidth / 2)
        {
            newX = -_barWidth / 2;
            _isMovingRight = true;
        }

        // Update the marker's position
        _marker.anchoredPosition = new Vector2(newX, _marker.anchoredPosition.y);
    }

    private void CheckClick()
    {
        if(_isDraggingIngredient) return;

        if (IsOverlapping(_hitBar, _marker))
        {
            _laddleAnim.SetTrigger("SpinLaddle");
            EventManager.OnPotWasStirredTrigger();
            EventManager.OnChangeCookingGameMecanicTrigger();
        }
    }

    private bool IsOverlapping(RectTransform area, RectTransform mark)
    {
        Rect areaRect = GetWorldRect(area);
        Rect markerRect = GetWorldRect(mark);

        return areaRect.Overlaps(markerRect);
    }

    private Rect GetWorldRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        
        Vector3 bottomLeft = corners[0];
        Vector3 topRight = corners[2];

        return new Rect(bottomLeft.x, bottomLeft.y, 
                        topRight.x - bottomLeft.x, 
                        topRight.y - bottomLeft.y);
    }

    private void ChangeGameplayMode()
    {
        if(_isDraggingIngredient)
        {
            _isDraggingIngredient = false;
            _markerBar.gameObject.SetActive(true);
            _potBottom.SetActive(false);
        }
        else
        {
            _isDraggingIngredient = true;
            _markerBar.gameObject.SetActive(false);
            _potBottom.SetActive(true);
        }
    }
}
