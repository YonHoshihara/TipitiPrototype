using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlateDropSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private RecipeDetailsSO _recipeSO;
    private List<GameObject> _droppedObjects;

    private void Start()
    {
        _droppedObjects = new List<GameObject>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;

        if (droppedObject != null && !_droppedObjects.Contains(droppedObject))
        {
            _droppedObjects.Add(droppedObject);
            droppedObject.GetComponent<DragAndDropIngredient>().SetLockedPosition();
        }

        CheckIngredients();
    }

    private void CheckIngredients()
    {
        if (_droppedObjects.Count == _recipeSO.ingredientsList.Count)
        {
            EventManager.OnGameWinTrigger();
        }
    }
}
