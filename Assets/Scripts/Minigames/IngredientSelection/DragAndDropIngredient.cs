using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDropIngredient : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    private Canvas _canvas;
    [SerializeField] private string _ingredientName;
    [SerializeField] private Image _ingredientIcon;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private Collider2D _col;
    [SerializeField] private GameObject _uiVersion;
    private Vector3 _originPos;
    private bool _isLocked;
    

    private void Start()
    {
        _canvas = FindAnyObjectByType<Canvas>();
        _isLocked = false;
        _originPos = transform.position;
    }

    public GameObject GetUIVersion()
    {
        return _uiVersion;
    }

    private void SetLockedPosition()
    {
        _isLocked = true;
    }

    public Sprite GetIngredientImage()
    {
        return _ingredientIcon.sprite;
    }

    public string GetIngredientName()
    {
        return _ingredientName;
    }

    public void SetIconNativeSize()
    {
        _ingredientIcon.SetNativeSize();
    }

    public void IsInsideCookingPot()
    {
        _rb.bodyType = RigidbodyType2D.Dynamic;
        _col.enabled = true;
        SetLockedPosition();
    }


    #region Pointer functions
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_isLocked) return;

        _canvasGroup.alpha = 0.5f;
        _canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_isLocked) return;

        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    public void OnDrop(PointerEventData eventData){}

    public void OnEndDrag(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            if (result.gameObject.CompareTag("DropZone"))
            {
                result.gameObject.GetComponent<PlateDropSlot>().AddIngredientToList(this.gameObject);
                SetLockedPosition();
                break;
            }
        }

        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;

        if(!_isLocked)
            transform.position = _originPos;
    }

    public void OnPointerDown(PointerEventData eventData){}
    #endregion
}
