using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.Events;

public class ScoreManager : Singleton<ScoreManager>
{
    [SerializeField] private TextMeshProUGUI m_scoreDisplay = null;
    [SerializeField] private int m_scorePerKill = 1;
    [SerializeField] private int m_chargeForWave = 10;
    [SerializeField] private Spawner m_enemySpawner = null;
    [SerializeField] private UnityEvent m_onWaveEnd = new UnityEvent();

    public bool IsCharged => m_charge >= m_chargeForWave;

    private Vector3 m_startPos = Vector3.zero;
    private int m_score = 0;
    private int m_charge = 0;
    private int m_enemiesThrough = 0;
    private int m_enemyCount = 0;
    private bool m_allSpawned = false;
    private int m_wave = 1;

    public void AllSpawned() {
        m_allSpawned = true;
    }

    public void EnemyKilled() {
        m_score += m_scorePerKill;
        ++m_charge;
        --m_enemyCount;
    }

    public void IncrementEnemyCount() {
        ++m_enemyCount;
    }

    public void EnemyGotThrough() {
        ResetCharge();
        ++m_enemiesThrough;
        --m_enemyCount;
    }

    public void ResetCharge() {
        m_charge = 0;
    }

    private void Start() {
        if(m_scoreDisplay == null ) {
            Debug.LogError("Score Manager must have score display");
            Destroy(this);
            return;
        }
        m_startPos = Camera.main.transform.position;
    }

    private void Update() {
        m_scoreDisplay.text = $"{m_score} / Wave {m_wave} / {m_charge} Charge / {m_enemiesThrough} Through / {m_enemyCount} Remain";
        if (m_allSpawned && m_enemyCount == 0)
            EndWave();
    }

    private void EndWave() {
        Debug.Log("Next wave");
        Camera.main.transform.position = m_startPos;

        m_enemySpawner.ResetSpawner();
        m_onWaveEnd.Invoke();

        m_enemiesThrough = 0;
        m_enemyCount = 0;
        m_allSpawned = false;

        ++m_wave;
    }
}
