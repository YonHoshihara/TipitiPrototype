using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryScreenSystem : MonoBehaviour
{
    [SerializeField] private SelectedRecipeSO _selectedRecipeSO;
    [SerializeField] private VictoryStatsSO _victoryStatsSO;
    [SerializeField] private Image[] _defaultPanelStars;
    [SerializeField] private Image[] _finishedRecipePanelStars;
    [SerializeField] private Image _finalTimeBar;
    [SerializeField] private Image _finishedRecipeImg;
    [SerializeField] private GameObject _defaultPanel;
    [SerializeField] private GameObject _endRecipePanel;

    private void Start()
    {
        int index = _selectedRecipeSO.recipe.recipeStepIndex;

        if(index + 1 >= _selectedRecipeSO.recipe.steps.Count)
        {
            _defaultPanel.SetActive(false);
            _endRecipePanel.SetActive(true);
            SetStarRate(_finishedRecipePanelStars);
            _finishedRecipeImg.sprite = _selectedRecipeSO.recipe.finishedRecipeSprite;
        }
        else
        {
            _defaultPanel.SetActive(true);
            _endRecipePanel.SetActive(false);
            SetStarRate(_defaultPanelStars);
        }
    }
            

    private void SetStarRate(Image[] stars)
    {
        int aux = _victoryStatsSO.GetStarRating();
        
        for(int i = 0; i < aux; i++)
        {
            Color temp = stars[i].color;
            temp.a = 1f;
            stars[i].color = temp;
        }

        if(_finalTimeBar != null)
        {
            _finalTimeBar.fillAmount = _victoryStatsSO.finalTime / _victoryStatsSO.totalTime;
        }
    }

    public void LoadNextLevel()
    {
        RecipeDetailsSO currentRecipe = _selectedRecipeSO.recipe;
        currentRecipe.recipeStepIndex += 1;
        EventManager.OnLoadNextLevelTrigger(currentRecipe.steps[currentRecipe.recipeStepIndex]);
    }
}
