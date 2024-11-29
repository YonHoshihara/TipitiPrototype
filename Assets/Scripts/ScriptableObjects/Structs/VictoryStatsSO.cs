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
        Debug.Log("3 stars - " + _3StarRating);
        Debug.Log("2 stars - " + _2StarRating);
        Debug.Log("1 stars - " + _1StarRating);

        if(finalTime > _2StarRating)
        {
            Debug.Log("3 estrelas");
            return 3;
        }
        else if(finalTime > _1StarRating)
        {
            Debug.Log("2 estrelas");
            return 2;
        }
        else
        {
            Debug.Log("1 estrelas");
            return 1;
        }

    }
}
