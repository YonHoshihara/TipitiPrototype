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
    #endregion
}
