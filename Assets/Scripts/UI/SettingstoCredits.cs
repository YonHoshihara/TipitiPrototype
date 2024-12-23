using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingstoCredits : MonoBehaviour
{
    [SerializeField] private Animator _anim;
    public void CreditsButton()
    {
        AudioSystem.Instance.PlaySFX("ButtonPass");
        _anim.SetTrigger("OpenCredits");
    }

    public void CreditsBackButton()
    {
        AudioSystem.Instance.PlaySFX("ButtonPass");
        _anim.SetTrigger("CloseCredits");
    }
}
