using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CookingPot : PlateDropSlot
{
    private int _potStirredAmount = 0;
    private bool _allIngredientsAdded = false;

    protected override void Start()
    {
        base.Start();

        EventManager.OnPotWasStirredEvent += PotStirred;
    }

    private void OnDestroy()
    {
        EventManager.OnPotWasStirredEvent -= PotStirred;
    }

    public override void AddIngredientToList(GameObject ingredient)
    {
        base.AddIngredientToList(ingredient);

        ingredient.GetComponent<DragAndDropIngredient>().IsInsideCookingPot();
        EventManager.OnChangeCookingGameMecanicTrigger();
    }

    protected override void CheckIngredients()
    {
        if (_droppedObjects.Count == _recipeSO.ingredientsList.Count)
        {
            _allIngredientsAdded = true;
        }
    }

    private void PotStirred()
    {
        _potStirredAmount += 1;

        if(_potStirredAmount >= _recipeSO.ingredientsList.Count && _allIngredientsAdded)
        {
            EventManager.OnGameWinTrigger();
        }
    }
}
