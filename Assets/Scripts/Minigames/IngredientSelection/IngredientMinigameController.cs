using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngredientMinigameController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RecipeDetailsSO _selectedRecipeSO;
    [SerializeField] private VictoryStatsSO _victoryStatsSO;

    [SerializeField] private Image _listClock;
    [SerializeField] private Image _minigameClock;

    [SerializeField] private GameObject _listContentGameobject;
    [SerializeField] private GameObject _listPanel;
    [SerializeField] private GameObject _victoryPanel;
    [SerializeField] private GameObject[] _minigamePlates;

    [Header("Variables")]
    [SerializeField] private float _listTimerValue;
    [SerializeField] private float _minigameTimerValue;
    [SerializeField] private float _timerDelay;
    private float _timeRemaining;
    private bool _isCountingDown;
    private bool _minigameIsRunning;

    // Functions

    private void Start()
    {
        EventManager.OnGameWinEvent += GameWin;

        StartCoroutine(TimeCountdownDelay(_listTimerValue));
        GenerateList();
        GenerateIngredientsInPlates();
    }

    private void OnDestroy()
    {
        EventManager.OnGameWinEvent -= GameWin;
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
            GameObject ingredient = Instantiate(_selectedRecipeSO.ingredientsList[i].GetComponent<DragAndDropIngredient>().GetUIVersion());
            ingredient.transform.SetParent(_listContentGameobject.transform);
        }
    }

    private void GenerateIngredientsInPlates()
    {
        // Generate dragable Ingredients on MINIGAME PANEL 
        for(int i = 0; i < _selectedRecipeSO.ingredientsList.Count; i++)
        {
            GameObject ingredient = Instantiate(_selectedRecipeSO.ingredientsList[i]);
            ingredient.transform.SetParent(_minigamePlates[i].transform);

            ingredient.transform.localRotation = Quaternion.identity; 
            ingredient.transform.localPosition = Vector3.zero;
        }
    }

    private void TimerCountdown(Image currentClock, float currentClockValue)
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

                
                Debug.Log("List countdownStop");
                _listPanel.SetActive(false);
                StartCoroutine(TimeCountdownDelay(_minigameTimerValue));
            }
            else
            {
                Debug.Log("Game over");
                _isCountingDown = false;
            }
        }
    }

    private IEnumerator TimeCountdownDelay(float currentClockValue)
    {
        _timeRemaining = currentClockValue;
        _isCountingDown = false;
        yield return new WaitForSeconds(_timerDelay);
        _isCountingDown = true;
    }

    private void GameWin()
    {
        _isCountingDown = false;
        _victoryStatsSO.totalTime = _minigameTimerValue;
        Debug.Log("total timer: " + _victoryStatsSO.totalTime);
        _victoryStatsSO.finalTime = _timeRemaining;
        Debug.Log("final timer: " + _victoryStatsSO.finalTime);

        _victoryPanel.SetActive(true);
    }
}
