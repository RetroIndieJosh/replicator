using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ScoreManager : Singleton<ScoreManager>
{
    [SerializeField] private TextMeshProUGUI m_scoreDisplay = null;
    [SerializeField] private int m_scorePerKill = 1;
    [SerializeField] private int m_chargeForWave = 10;
    [SerializeField] private Spawner m_enemySpawner = null;
    [SerializeField] private UnityEvent m_onWaveEnd = new UnityEvent();

    [Header("Waves")]
    [SerializeField] private int m_allowedPastCount = 5;
    [SerializeField] private float m_enemySpeedBase = 3.5f;
    [SerializeField] private float m_enemySpeedDivider = 5f;
    [SerializeField] private float m_enemySpeedInc = 0.1f;
    [SerializeField] private int m_spawnCountBase = 10;
    [SerializeField] private float m_spawnCountInc = 1.5f;

    public bool IsCharged => m_charge >= m_chargeForWave;
    public float EnemySpeed => m_enemySpeedBase + m_wave * m_enemySpeedInc;
    public float EnemySpeedDivider => m_enemySpeedDivider;
    public int EnemyCountMax => m_spawnCountBase + Mathf.FloorToInt(m_wave * m_spawnCountInc);

    private Vector3 m_startPos = Vector3.zero;
    private int m_score = 0;
    private int m_charge = 0;
    private int m_enemiesThrough = 0;
    private int m_enemyCount = 0;
    private bool m_allSpawned = false;
    private int m_wave = 0;

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
        if(m_enemiesThrough >= m_allowedPastCount)
            GameOver();
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
        StartWave(true);
        m_wave = 0;
    }

    private void Update() {
        var gamepad = Gamepad.current;
        var keyboard = Keyboard.current;
        if ((keyboard != null && keyboard.rKey.isPressed) ||
            (gamepad != null && gamepad.aButton.IsPressed())) { 
            Time.timeScale = 1f;
            StartWave(true);
        }

        m_scoreDisplay.text = $"{m_score} / Wave {m_wave} / {m_charge} Charge / {m_enemiesThrough} Through / {m_enemyCount} Remain";
        if (m_allSpawned && m_enemyCount == 0)
            StartWave();
    }

    private void GameOver() {
        Debug.Log("Game over");
        Time.timeScale = 0f;
    }

    private void StartWave(bool a_isFirst = false) {
        if (a_isFirst)
            m_wave = 0;
        else
            ++m_wave;

        Debug.Log("Next wave");
        Camera.main.transform.position = m_startPos;

        m_enemySpawner.ResetSpawner();
        m_enemySpawner.MaxTotal = EnemyCountMax;
        m_onWaveEnd.Invoke();

        m_enemiesThrough = 0;
        m_enemyCount = 0;
        m_allSpawned = false;
    }
}
