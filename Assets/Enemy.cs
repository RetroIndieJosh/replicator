using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Exploder3d))]
public class Enemy : MonoBehaviour
{
    private void Start() {
        GetComponent<Exploder3d>().OnExplode.AddListener(() => {
            ScoreManager.instance.IncrementScore();
        });
        ScoreManager.instance.IncrementEnemyCount();
    }

    private void Update() {
        if (transform.position.z > Camera.main.transform.position.z)
            return;

        ScoreManager.instance.EnemyGotThrough();
        Destroy(gameObject);
    }
}
