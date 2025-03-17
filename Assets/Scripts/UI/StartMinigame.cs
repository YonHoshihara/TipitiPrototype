using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StartMinigame : MonoBehaviour
{
    //[SerializeField] private
    [SerializeField] private SelectedRecipeSO _currentRecipe;
    [SerializeField] private TMP_Text _nomeMinigameTxt;
    [SerializeField] private TMP_Text _passoMinigameTxt;
    [SerializeField] private Animator _animator;

    private void Start()
    {
        _passoMinigameTxt.text = "Passo " + _currentRecipe.recipe.GetCurrentRecipeStep();
        Time.timeScale = 0f;
    }

    public void TimeIsActive(float timeScaleValue)
    {
        Time.timeScale = timeScaleValue;
    }

    public void DisableTHisPanel()
    {
        gameObject.SetActive(false);
    }
}
