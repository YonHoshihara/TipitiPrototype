using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlateDropSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] protected RecipeDetailsSO _recipeSO;
    protected List<GameObject> _droppedObjects;

    protected virtual void Start()
    {
        _droppedObjects = new List<GameObject>();
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        /*
        droppedObject = eventData.pointerDrag;

        if (droppedObject != null && !_droppedObjects.Contains(droppedObject))
        {
            _droppedObjects.Add(droppedObject);
        }

        CheckIngredients();
        */
    }

    protected virtual void CheckIngredients()
    {
        if (_droppedObjects.Count == _recipeSO.ingredientsList.Count)
        {
            EventManager.OnGameWinTrigger();
        }
    }

    public virtual void AddIngredientToList(GameObject ingredient)
    {
        if(!_droppedObjects.Contains(ingredient))
        {
            _droppedObjects.Add(ingredient);
        }

        CheckIngredients();
    }
}
