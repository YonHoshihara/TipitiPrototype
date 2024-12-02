using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{
    #region Events
    public  delegate void OnGameWin();
    public static event OnGameWin OnGameWinEvent;

    public delegate void OnGameOver();
    public static event OnGameOver OnGameOverEvent;

    public delegate void OnChangeCookingGameMecanic();
    public static event OnChangeCookingGameMecanic OnChangeCookingGameMecanicEvent;

    public delegate void OnLaddleWasClicked();
    public static event OnLaddleWasClicked OnLaddleWasClickedEvent;

    public delegate void OnPotWasStirred();
    public static event OnPotWasStirred OnPotWasStirredEvent;
    #endregion


    #region Triggers
    public static void OnGameWinTrigger()
    {
        OnGameWinEvent?.Invoke();
    }

    public static void OnGameOverTrigger()
    {
        OnGameOverEvent?.Invoke();
    }

    public static void OnChangeCookingGameMecanicTrigger()
    {
        OnChangeCookingGameMecanicEvent?.Invoke();
    }

    public static void OnLaddleWasClickedTrigger()
    {
        OnLaddleWasClickedEvent?.Invoke();
    }

    public static void OnPotWasStirredTrigger()
    {
        OnPotWasStirredEvent?.Invoke();
    }
    #endregion
}
