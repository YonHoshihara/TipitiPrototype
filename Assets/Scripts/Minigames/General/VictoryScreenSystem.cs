using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryScreenSystem : MonoBehaviour
{
    [SerializeField] private VictoryStatsSO _victoryStatsSO;
    [SerializeField] private Image[] _stars;
    [SerializeField] private Image _finalTimeBar;

    private void Start()
    {
        SetStarRate();
    }

    private void SetStarRate()
    {
        int aux = _victoryStatsSO.GetStarRating();
        
        for(int i = 0; i < aux; i++)
        {
            Color temp = _stars[i].color;
            temp.a = 1f;
            _stars[i].color = temp;
        }

        if(_finalTimeBar != null)
        {
            _finalTimeBar.fillAmount = _victoryStatsSO.finalTime / _victoryStatsSO.totalTime;
        }
    }
}
