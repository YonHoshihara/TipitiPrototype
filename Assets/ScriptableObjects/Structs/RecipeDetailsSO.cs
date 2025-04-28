using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe Details", menuName = "ScriptableObjects/Recipe Details")]
public class RecipeDetailsSO : ScriptableObject
{
    public string recipeName;
    public Sprite finishedRecipeSprite;
    public List<GameObject> ingredientsList;
    public List<String> steps;
    public int recipeStepIndex = 0;
    public RecipeDetailsSO nextRecipe;

    public int GetCurrentRecipeStep()
    {
        return recipeStepIndex + 1;
    }

    private void ResetStep()
    {
        recipeStepIndex = 0;
    }
}
