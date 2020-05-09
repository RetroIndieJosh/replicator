using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(DirectionalGravity))]
public class GravityToNearest : MonoBehaviour
{
    [SerializeField] private string m_targetTag = "";
    [SerializeField] private float m_checkRadius = 500f;

    private DirectionalGravity m_directionalGravity = null;
    private bool m_gravitySet = false;
    // TODO private bool m_continuous: check for nearest on update and change if needed

    private void Awake() {
        m_directionalGravity = GetComponent<DirectionalGravity>();
    }

    private void Start() {
        var pos = transform.position;
        if(Mathf.Abs(pos.x) > Mathf.Abs(pos.y))
            m_directionalGravity.GravityDirection = Vector3.left * Mathf.Sign(pos.x);
        else
            m_directionalGravity.GravityDirection = Vector3.down * Mathf.Sign(pos.y);
    }

    private void Update() {
    }
}
