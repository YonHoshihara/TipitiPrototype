using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientListing : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _text;

    public void SetIconAndName(Sprite sprite, string name)
    {
        _icon.sprite = sprite;
        _text.text = name;
    }
}
