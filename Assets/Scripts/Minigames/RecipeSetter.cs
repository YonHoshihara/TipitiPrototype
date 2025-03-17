using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeSetter : MonoBehaviour
{
    [SerializeField] private RecipeDetailsSO _recipe;
    [SerializeField] private SelectedRecipeSO _selectedRecipeSO;
    [SerializeField] private string _specialFirstScene;
 
    public void SetSelectedRecipe()
    {
        _selectedRecipeSO.recipe = _recipe;
        _selectedRecipeSO.recipe.recipeStepIndex = 0;
        StartCoroutine(LoadNextScene());
    }

    private IEnumerator LoadNextScene()
    {
        _selectedRecipeSO.recipe = _recipe;
        yield return new WaitForSeconds(0.3f);

        if(_specialFirstScene == "" || _specialFirstScene == null)
            EventManager.OnLoadNextLevelTrigger(_recipe.steps[0]);
        else
            EventManager.OnLoadNextLevelTrigger(_specialFirstScene);
    }
}
