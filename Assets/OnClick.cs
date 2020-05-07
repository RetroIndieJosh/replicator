using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventCaller : MonoBehaviour
{
    [SerializeField] UnityEvent m_onEvent = new UnityEvent();

    protected void Activate() {
        m_onEvent.Invoke();
    }
}

public class OnClick : EventCaller
{
    private void OnMouseDown() {
        Activate();
    }
}
