using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Macaxeira : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Image _descascada;
    private Image _img;

    private void Start()
    {
        _img = GetComponent<Image>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {   
        EventManager.OnPeelMacaxeiraPointTrigger();
        _img.raycastTarget = false;
        _img.color = Color.white;
        _descascada.enabled = true;
    }
}
