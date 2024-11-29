using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Selected Recipe", menuName = "ScriptableObjects/Selected Recipe")]
public class SelectedRecipeSO : ScriptableObject
{
    public RecipeDetailsSO recipe;
}
