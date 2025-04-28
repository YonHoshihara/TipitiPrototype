using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinigameController : MonoBehaviour
{
    [Header("Base Minigame References")]
    [SerializeField] protected SelectedRecipeSO _selectedRecipe;
    [SerializeField] protected VictoryStatsSO _victoryStatsSO;
    [SerializeField] protected Image _minigameClock;
    [SerializeField] protected GameObject _victoryPanel;
    [SerializeField] protected GameObject _gameoverPanel;
    [SerializeField] protected GameObject[] _minigamePlates;

    [Header("Base Minigme Variables")]

    [SerializeField] protected float _minigameTimerValue;
    [SerializeField] protected float _timerDelay;
    protected float _timeRemaining;
    protected bool _isCountingDown;
    protected RecipeDetailsSO _currentRecipeSO;

    protected virtual void Start()
    {
        _currentRecipeSO = _selectedRecipe.recipe;
        EventManager.OnGameWinEvent += GameWin;
        EventManager.OnGameOverEvent += GameOver;
    }

    protected virtual void OnDestroy()
    {
        EventManager.OnGameWinEvent -= GameWin;
        EventManager.OnGameOverEvent -= GameOver;
    }

    protected virtual void GenerateIngredientsInPlates()
    {
        // Generate dragable Ingredients on MINIGAME PANEL 
        for(int i = 0; i < _currentRecipeSO.ingredientsList.Count; i++)
        {
            GameObject ingredient = Instantiate(_currentRecipeSO.ingredientsList[i]);
            ingredient.transform.SetParent(_minigamePlates[i].transform);

            ingredient.transform.localRotation = Quaternion.identity; 
            ingredient.transform.localPosition = Vector3.zero;

            ingredient.GetComponent<DragAndDropIngredient>().SetIconNativeSize();
        }
    }

    protected virtual void TimerCountdown(Image currentClock, float currentClockValue)
    {
        _timeRemaining -= Time.deltaTime;

        if (_timeRemaining > 0)
        {
            currentClock.fillAmount = _timeRemaining / currentClockValue;
        }
        else
        {
            currentClock.fillAmount = 0f;
            EventManager.OnGameOverTrigger();
            _isCountingDown = false;
        }
    }

    protected virtual IEnumerator TimeCountdownDelay(float currentClockValue)
    {
        Debug.Log("countdown");
        AudioSystem.Instance.StopLoopingSFX("ClockTicking");
        _timeRemaining = currentClockValue;
        _isCountingDown = false;
        yield return new WaitForSeconds(_timerDelay);
        _isCountingDown = true;
        AudioSystem.Instance.PlayLoopingSFX("ClockTicking");
    }

    protected virtual void GameWin()
    {
        AudioSystem.Instance.StopAllLoopingSFX();
        AudioSystem.Instance.PlaySFX("GameWin");
        _isCountingDown = false;
        _victoryStatsSO.totalTime = _minigameTimerValue;
        _victoryStatsSO.finalTime = _timeRemaining;

        _victoryPanel.SetActive(true);
    }

    protected virtual void GameOver()
    {
        AudioSystem.Instance.StopAllLoopingSFX();
        AudioSystem.Instance.PlaySFX("GameOver");
        _isCountingDown = false;
        _gameoverPanel.SetActive(true);
    }
}
