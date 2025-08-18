using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectedRecipeManager : MonoBehaviour
{
    [SerializeField] private SelectedRecipeSO _selectedRecipeSO;
    public static SelectedRecipeManager Instance;
    public RecipeDetailsSO SelectedRecipe;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Subscribe to scene loaded event
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    private void OnDestroy()
    {
        // Always unsubscribe to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene {scene.name} loaded. SelectedRecipeManager reacting early.");

        // Call your function here!
        InitializeRecipeInNewScene();
    }

    private void InitializeRecipeInNewScene()
    {
        if (SelectedRecipe != null)
        {
            _selectedRecipeSO.recipe = SelectedRecipe;
            Debug.Log($"Setting up recipe: {SelectedRecipe.recipeName}");
        }
    }
}
