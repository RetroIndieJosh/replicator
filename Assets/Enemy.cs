using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Exploder3d))]
public class Enemy : MonoBehaviour
{
    private void Start() {
        var exploder = GetComponent<Exploder3d>();
        exploder.PieceCount = Mathf.Max(PlayerPrefs.GetInt("Piece Count"), 20);
        exploder.OnExplode.AddListener(() => {
            ScoreManager.instance.EnemyKilled();
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
            Destroy(this, 1f);
        });

        ScoreManager.instance.IncrementEnemyCount();
    }

    private void Update() {
        if (transform.position.z < Camera.main.transform.position.z) {
            ScoreManager.instance.EnemyGotThrough();
            Destroy(gameObject);
            return;
        }

        var distanceToPlayer = Vector3.Distance(transform.position, Camera.main.transform.position);
        var vel = ScoreManager.instance.EnemySpeed + distanceToPlayer / ScoreManager.instance.EnemySpeedDivider;
        GetComponent<AutoMover>().Velocity = Vector3.back * vel;
    }
}
