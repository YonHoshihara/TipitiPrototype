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

    public delegate void OnPeelMacaxeiraPoint();
    public static event OnPeelMacaxeiraPoint OnPeelMacaxeiraPointEvent;

    public delegate void OnPotWasStirred();
    public static event OnPotWasStirred OnPotWasStirredEvent;

    public delegate void OnChangeStepIconToPot();
    public static event OnChangeStepIconToPot OnChangeStepIconToPotEvent;

    public delegate void OnChangeStepIconToIngredient();
    public static event OnChangeStepIconToIngredient OnChangeStepIconToIngredientEvent;

    public delegate void OnLoadDialogue(DialogueSO dialogue);
    public static event OnLoadDialogue OnLoadDialogueEvent;

    public delegate void OnEndDialogue();
    public static event OnEndDialogue OnEndDialogueEvent;

    public delegate void OnLoadNextLevel(string lvl);
    public static event OnLoadNextLevel OnLoadNextLevelEvent;
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

    public static void OnPeelMacaxeiraPointTrigger()
    {
        OnPeelMacaxeiraPointEvent?.Invoke();
    }

    public static void OnPotWasStirredTrigger()
    {
        OnPotWasStirredEvent?.Invoke();
    }

    public static void OnChangeStepIconToPotTrigger()
    {
        OnChangeStepIconToPotEvent?.Invoke();
    }

    public static void OnChangeStepIconToIngredientTrigger()
    {
        OnChangeStepIconToPotEvent?.Invoke();
    }

    public static void OnLoadDialogueTrigger(DialogueSO dialogue)
    {
        OnLoadDialogueEvent?.Invoke(dialogue);
    }

    public static void OnEndDialogueTrigger()
    {
        OnEndDialogueEvent?.Invoke();
    }

    public static void OnLoadNextLevelTrigger(string lvl)
    {
        OnLoadNextLevelEvent?.Invoke(lvl);
    }
    #endregion
}
