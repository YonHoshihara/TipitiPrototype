using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "All Ingredients List", menuName = "ScriptableObjects/Ingredients List")]
public class AllIngredientsList : ScriptableObject
{
    public List<GameObject> _ingredientPFB;
}
