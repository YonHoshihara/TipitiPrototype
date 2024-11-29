using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropIngredient : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    private Canvas _canvas;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private CanvasGroup _canvasGroup;
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

    public void SetLockedPosition()
    {
        _isLocked = true;
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
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;

        if(!_isLocked)
            transform.position = _originPos;
    }

    public void OnPointerDown(PointerEventData eventData){}
    #endregion
}
