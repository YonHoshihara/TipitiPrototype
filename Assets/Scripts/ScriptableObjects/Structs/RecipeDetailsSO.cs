using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe Details", menuName = "ScriptableObjects/Recipe Details")]
public class RecipeDetailsSO : ScriptableObject
{
    public string recipeName;
    public List<GameObject> ingredientsList;
}
