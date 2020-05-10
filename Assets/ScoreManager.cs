using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreManager : Singleton<ScoreManager>
{
    [SerializeField] TextMeshProUGUI m_scoreDisplay = null;
    [SerializeField] private int m_scorePerKill = 1;
    [SerializeField] private int m_chargeForWave = 10;

    public bool IsCharged => m_charge >= m_chargeForWave;

    private int m_score = 0;
    private int m_charge = 0;
    private int m_enemiesThrough = 0;
    private int m_enemyCount = 0;

    public void IncrementScore() {
        m_score += m_scorePerKill;
        ++m_charge;
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
    }

    private void Update() {
        m_scoreDisplay.text = $"{m_score} / {m_charge} Charge / {m_enemiesThrough} Through / {m_enemyCount} Remain";
    }
}
