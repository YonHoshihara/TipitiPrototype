using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientMinigameController : MinigameController
{
    [Header("Separation Minigame References")]
    [SerializeField] protected AllIngredientsList _allIngredientsSO;
    [SerializeField] private Image _listClock;
    [SerializeField] private GameObject _listContentGameobject;
    [SerializeField] private GameObject _listPanel;
    [SerializeField] private TMP_Text _recipeNameTxt;

    [Header("Separation Minigame Variables")]
    [SerializeField] private float _listTimerValue;    
    private bool _minigameIsRunning;


    protected override void Start()
    {
        base.Start();
        StartCoroutine(TimeCountdownDelay(_listTimerValue));
        GenerateList();
        GenerateIngredientsInPlates();

        _recipeNameTxt.text = _selectedRecipe.recipe.recipeName;
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

    protected override void GenerateIngredientsInPlates()
    {
        List<GameObject> tempList = _allIngredientsSO._ingredientPFB.Except(_currentRecipeSO.ingredientsList).ToList();
        List<GameObject> plateIngredients = new List<GameObject>();
        plateIngredients.AddRange(_currentRecipeSO.ingredientsList);

        int aux = 0;
        while(plateIngredients.Count < _minigamePlates.Length)
        {
            plateIngredients.Add(tempList[aux]);
            aux++;
        }

        plateIngredients = ShuffleIngredients(plateIngredients);

        // Generate dragable Ingredients on MINIGAME PANEL 
        for(int i = 0; i < plateIngredients.Count; i++)
        {
            GameObject ingredient = Instantiate(plateIngredients[i]);
            ingredient.transform.SetParent(_minigamePlates[i].transform);

            ingredient.transform.localRotation = Quaternion.identity; 
            ingredient.transform.localPosition = Vector3.zero;


            DragAndDropIngredient ingredientScript = ingredient.GetComponent<DragAndDropIngredient>();
            ingredientScript.SetIconNativeSize();

            if (ingredientScript.GetIngredientName() == "Macaxeira")
                ingredientScript.SetDifferentForm();
        }
    }

    private List<GameObject> ShuffleIngredients(List<GameObject> ingredients)
    {
        int n = ingredients.Count;

        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1); // UnityEngine.Random.Range
            (ingredients[n], ingredients[k]) = (ingredients[k], ingredients[n]); // Swap elements
        }
        
        return ingredients;
    }

    private void GenerateList()
    {
        // Generate ingredient list on LIST PANEL 
        for(int i = 0; i < _currentRecipeSO.ingredientsList.Count; i++)
        {
            var ingredientScript = _currentRecipeSO.ingredientsList[i].GetComponent<DragAndDropIngredient>();

            GameObject ingredient = Instantiate(ingredientScript.GetUIVersion());
            ingredient.transform.SetParent(_listContentGameobject.transform);

            if (ingredientScript.GetIngredientName() == "Macaxeira")
                ingredient.GetComponent<IngredientListing>().SetIconAndName(ingredientScript.GetIngredientAlternateImage(), ingredientScript.GetIngredientName());
            else
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
        AudioSystem.Instance.StopLoopingSFX("ClockTicking");
        _timeRemaining = currentClockValue;
        _isCountingDown = false;
        yield return new WaitForSeconds(_timerDelay);
        _isCountingDown = true;
        AudioSystem.Instance.PlayLoopingSFX("ClockTicking");
    }
}
