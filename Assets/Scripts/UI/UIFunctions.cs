using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIFunctions : MonoBehaviour
{
    private void Start()
    {
        EventManager.OnLoadNextLevelEvent += LoadLevel;
    }

    private void OnDestroy()
    {
        EventManager.OnLoadNextLevelEvent -= LoadLevel;
    }

    public void PlayGameButton(string sceneName)
    {
        AudioSystem.Instance.PlaySFX("ButtonClickForward");
        SceneManager.LoadScene(sceneName);
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadLevel(string sceneName)
    {
        AudioSystem.Instance.PlaySFX("ButtonClickForward");
        SceneManager.LoadScene(sceneName);
    }

    public void ShowPopUp(GameObject go)
    {
        AudioSystem.Instance.PlaySFX("ButtonClickForward");
        go.SetActive(true);
    }

    public void HidePopUp(GameObject go)
    {
        AudioSystem.Instance.PlaySFX("ButtonClickBack");
        go.SetActive(false);
    }

    public void ChangeScene(string sceneName)
    {
        AudioSystem.Instance.PlaySFX("ButtonPass");
        SceneManager.LoadScene(sceneName);
    }


}
