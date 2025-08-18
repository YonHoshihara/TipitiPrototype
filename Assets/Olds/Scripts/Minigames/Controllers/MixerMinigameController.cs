using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixerMinigameController : MinigameController
{
    [Header("Mixer Minigame References")]
    [SerializeField] private GameObject _batedeira;
    [SerializeField] private GameObject _massa;
    [SerializeField] private Animator _mixerAnim;

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

        _mixerAnim.SetTrigger("Mix");
        _massa.SetActive(true);

        foreach(Transform child in _batedeira.transform)
        {
            child.gameObject.SetActive(false);
        }

        EventManager.OnGameWinTrigger();
    }

    private void ChangeGameplayMode()
    {
        if(_isDraggingIngredient)
        {
            _isDraggingIngredient = false;
        }
        else
        {
            _isDraggingIngredient = true;
            _batedeira.SetActive(true);
        }
    }
}
