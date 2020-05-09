using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private void Start() {
        GetComponent<Exploder3d>().OnExplode.AddListener(() => {
            FindObjectOfType<ScoreManager>().IncrementScore();
        });
    }
}
