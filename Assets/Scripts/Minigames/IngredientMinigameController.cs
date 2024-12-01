using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngredientMinigameController : MinigameController
{
    [Header("Separation Minigame References")]
    [SerializeField] private Image _listClock;
    [SerializeField] private GameObject _listContentGameobject;
    [SerializeField] private GameObject _listPanel;

    [Header("Separation Minigame Variables")]
    [SerializeField] private float _listTimerValue;    
    private bool _minigameIsRunning;


    protected override void Start()
    {
        base.Start();

        StartCoroutine(TimeCountdownDelay(_listTimerValue));
        GenerateList();
        GenerateIngredientsInPlates();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    private void Update()
    {
        if(_isCountingDown)
        {
            if(_minigameIsRunning)
            {
                // if minigame is on
                TimerCountdown(_minigameClock, _minigameTimerValue);
            }
            else
            {
                // if is just showing ingredient list
                TimerCountdown(_listClock, _listTimerValue);
            }
        }
    }

    private void GenerateList()
    {
        // Generate ingredient list on LIST PANEL 
        for(int i = 0; i < _selectedRecipeSO.ingredientsList.Count; i++)
        {
            var ingredientScript = _selectedRecipeSO.ingredientsList[i].GetComponent<DragAndDropIngredient>();

            GameObject ingredient = Instantiate(ingredientScript.GetUIVersion());
            ingredient.transform.SetParent(_listContentGameobject.transform);

            ingredient.GetComponent<IngredientListing>().SetIconAndName(ingredientScript.GetIngredientImage(), ingredientScript.GetIngredientName());
        }
    }

    protected override void TimerCountdown(Image currentClock, float currentClockValue)
    {
        _timeRemaining -= Time.deltaTime;

        if (_timeRemaining > 0)
        {
            currentClock.fillAmount = _timeRemaining / currentClockValue;
        }
        else
        {
            currentClock.fillAmount = 0f;

            if(!_minigameIsRunning)
            {
                _minigameIsRunning = true;
                _isCountingDown = false;

                _listPanel.SetActive(false);
                StartCoroutine(TimeCountdownDelay(_minigameTimerValue));
            }
            else
            {
                EventManager.OnGameOverTrigger();
                _isCountingDown = false;
            }
        }
    }

    protected override IEnumerator TimeCountdownDelay(float currentClockValue)
    {
        _timeRemaining = currentClockValue;
        _isCountingDown = false;
        yield return new WaitForSeconds(_timerDelay);
        _isCountingDown = true;
    }
}
