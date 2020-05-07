using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTrigger : EventCaller
{
    [SerializeField] private bool m_onEnter = false;
    [SerializeField] private bool m_onExit = false;
    [SerializeField] private bool m_onStay = false;

    private void OnTriggerEnter(Collider other) {
        if (m_onEnter) Activate();
    }
    private void OnTriggerExit(Collider other) {
        if (m_onExit) Activate();
    }
    private void OnTriggerStay(Collider other) {
        if (m_onStay) Activate();
    }
}
