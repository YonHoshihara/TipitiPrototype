using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixerMinigameController : MinigameController
{
    [Header("Mixer Minigame References")]
    [SerializeField] private GameObject _batedeira;
    [SerializeField] private Animator _laddleAnim;

    [Header("Mixer Minigame Variables")]
    private bool _isDraggingIngredient;

    protected override void Start()
    {
        base.Start();
        EventManager.OnChangeCookingGameMecanicEvent += ChangeGameplayMode;
        EventManager.OnLaddleWasClickedEvent += CheckClick;

        StartCoroutine(TimeCountdownDelay(_minigameTimerValue));
        GenerateIngredientsInPlates();

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
    }

    private void CheckClick()
    {
        if(_isDraggingIngredient) return;

        // trigger mixer animation
        EventManager.OnGameWinTrigger();
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
            _batedeira.SetActive(false);
        }
        else
        {
            _isDraggingIngredient = true;
            _batedeira.SetActive(true);
        }
    }
}
