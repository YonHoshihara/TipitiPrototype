using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingstoCredits : MonoBehaviour
{
    [SerializeField] private Animator _anim;
    public void CreditsButton()
    {
        
        _anim.SetTrigger("OpenCredits");
    }

    public void CreditsBackButton()
    {
       
        _anim.SetTrigger("CloseCredits");
    }
}
