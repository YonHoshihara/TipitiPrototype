using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlateDropSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] protected SelectedRecipeSO _selectedRecipeSO;
    [SerializeField] protected AllIngredientsList _allIngredientListSO;
    protected List<string> _droppedIngredients;
    protected List<string> _recipeIngredientsNames;
    protected RecipeDetailsSO _recipeSO;

    protected virtual void Start()
    {
        _recipeSO = _selectedRecipeSO.recipe;
        _droppedIngredients = new List<string>();
        _recipeIngredientsNames = new List<string>();

        foreach(GameObject ingredient in _recipeSO.ingredientsList)
        {
            _recipeIngredientsNames.Add(ingredient.GetComponent<DragAndDropIngredient>().GetIngredientName());
        }
    }

    public virtual void OnDrop(PointerEventData eventData){}

    public virtual void AddIngredientToList(GameObject ingredient)
    {
        DragAndDropIngredient ingredientScript = ingredient.GetComponent<DragAndDropIngredient>();
        
        if(!CheckIfIngredientIsOnRecipe(ingredientScript.GetIngredientName())) return;

        if(!_droppedIngredients.Contains(ingredientScript.GetIngredientName()))
        {
            AudioSystem.Instance.PlaySFX("PlatedIngredient");
            _droppedIngredients.Add(ingredientScript.GetIngredientName());
            ingredientScript.SetLockedPosition();
        }

        CheckIngredientsAmmount();
    }

    protected virtual void CheckIngredientsAmmount()
    {
        if (_droppedIngredients.Count == _recipeSO.ingredientsList.Count)
        {
            EventManager.OnGameWinTrigger();
        }
    }

    protected virtual bool CheckIfIngredientIsOnRecipe(string ingredient)
    {
        if(_recipeIngredientsNames.Contains(ingredient))
            return true;
        else
            return false;
    }
}
