using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRotation : MonoBehaviour
{
    private Vector3 m_relativePos;
    private Quaternion m_rotation;

    // Start is called before the first frame update
    void Start()
    {
        m_rotation = transform.rotation;
        m_relativePos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = m_rotation;
        transform.localPosition = m_relativePos;
    }
}
