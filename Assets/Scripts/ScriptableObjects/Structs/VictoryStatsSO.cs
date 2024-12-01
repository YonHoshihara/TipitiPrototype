using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Victory Stats", menuName = "ScriptableObjects/Victory Stats")]
public class VictoryStatsSO : ScriptableObject
{
    public float finalTime;
    public float totalTime;
    
    public int GetStarRating()
    {
        float percentage = totalTime / 3;
        float _1StarRating = percentage;
        float _2StarRating = percentage * 2; 
        float _3StarRating = percentage * 3;

        if(finalTime > _2StarRating)
        {
            return 3;
        }
        else if(finalTime > _1StarRating)
        {
            return 2;
        }
        else
        {
            return 1;
        }

    }
}
