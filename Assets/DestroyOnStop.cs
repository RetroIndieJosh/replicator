using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnStop : MonoBehaviour
{
    private Rigidbody m_body = null;

    private void Awake() {
        m_body = GetComponent<Rigidbody>();
    }

    private void Update() {
        if (m_body.velocity.magnitude < Mathf.Epsilon)
            Destroy(gameObject);
    }
}
