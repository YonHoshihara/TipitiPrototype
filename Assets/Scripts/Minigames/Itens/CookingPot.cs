using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CookingPot : PlateDropSlot
{
    [SerializeField] private Image _stepIconImg;
    [SerializeField] private Sprite _potSprite;
    private int _potStirredAmount = 0;
    private int _ingredientListIndex = 0;
    private bool _allIngredientsAdded = false;

    protected override void Start()
    {
        base.Start();

        EventManager.OnPotWasStirredEvent += PotStirred;

        SetStepIconImg();
    }

    private void OnDestroy()
    {
        EventManager.OnPotWasStirredEvent -= PotStirred;
    }

    public override void AddIngredientToList(GameObject ingredient)
    {
        DragAndDropIngredient ingredientScript = ingredient.GetComponent<DragAndDropIngredient>();
        
        if(ingredientScript.GetIngredientName() != _recipeIngredientsNames[_ingredientListIndex]) return;

        if(!_droppedIngredients.Contains(ingredientScript.GetIngredientName()))
        {
            _droppedIngredients.Add(ingredientScript.GetIngredientName());
            ingredientScript.SetLockedPosition();
        }

        CheckIngredientsAmmount();
        _ingredientListIndex += 1;
        SetStepIconImg();
        ingredientScript.IsInsideCookingPot();
        EventManager.OnChangeCookingGameMecanicTrigger();
    }

    protected override void CheckIngredientsAmmount()
    {
        if (_droppedIngredients.Count == _recipeSO.ingredientsList.Count)
        {
            _allIngredientsAdded = true;
        }
    }

    private void PotStirred()
    {
        _potStirredAmount += 1;
        SetStepIconImg();

        if(_potStirredAmount >= _recipeSO.ingredientsList.Count && _allIngredientsAdded)
        {
            EventManager.OnGameWinTrigger();
        }
    }

    private void SetStepIconImg()
    {
        if(_potStirredAmount >= _recipeSO.ingredientsList.Count) return;

        if(_ingredientListIndex <= _potStirredAmount || _ingredientListIndex == 0)
        {
            _stepIconImg.sprite = _recipeSO.ingredientsList[_ingredientListIndex].GetComponent<DragAndDropIngredient>().GetIngredientImage();
            _stepIconImg.SetNativeSize();
            _stepIconImg.rectTransform.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            _stepIconImg.sprite = _potSprite;
            _stepIconImg.SetNativeSize();
            _stepIconImg.rectTransform.localScale = new Vector3(0.3f, 0.3f, 1f);
        }
    }
}
