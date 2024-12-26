using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventCaller : MonoBehaviour
{
    [SerializeField] private UnityEvent OnEventCall;
    public void CallEvent() {  OnEventCall.Invoke(); }
}
