using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOnCollision : MonoBehaviour
{
    [SerializeField] private bool m_destroyAfterPlay;

    private void OnCollisionEnter(Collision collision) {
        var source = GetComponent<AudioSource>();
        source.Play();
        Destroy(source, source.clip.length);
        Destroy(this);
    }
}
