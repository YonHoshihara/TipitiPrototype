using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mixer : PlateDropSlot
{
    [SerializeField] private Image _stepIconImg;
    [SerializeField] private Sprite _mixerSprite;
    private int _ingredientListIndex = 0;
    private bool _allIngredientsAdded = false;

    protected override void Start()
    {
        base.Start();

        SetStepIconImg();
    }

    private void OnDestroy()
    {
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
    }

    protected override void CheckIngredientsAmmount()
    {
        if (_droppedIngredients.Count == _recipeSO.ingredientsList.Count)
        {
            _allIngredientsAdded = true;
        }
    }

    private void SetStepIconImg()
    {
        if(_ingredientListIndex < _recipeSO.ingredientsList.Count || _ingredientListIndex == 0)
        {
            _stepIconImg.sprite = _recipeSO.ingredientsList[_ingredientListIndex].GetComponent<DragAndDropIngredient>().GetIngredientImage();
            _stepIconImg.SetNativeSize();
            _stepIconImg.rectTransform.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            _stepIconImg.sprite = _mixerSprite;
            _stepIconImg.SetNativeSize();
            _stepIconImg.rectTransform.localScale = new Vector3(0.3f, 0.3f, 1f);
            EventManager.OnChangeCookingGameMecanicTrigger();
        }
    }
}
