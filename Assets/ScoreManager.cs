﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices.WindowsRuntime;
using JoshuaMcLean.JoshuaMcLean;

public class ScoreManager : Singleton<ScoreManager>
{
    [SerializeField] private int m_startWave = 0;
    [SerializeField] private bool m_pauseOnStart = false;
    [SerializeField] private int m_scorePerKill = 1;
    [SerializeField] private int m_chargeForWave = 10;
    [SerializeField] private Spawner m_enemySpawner = null;
    [SerializeField] private UnityEvent m_onWaveEnd = new UnityEvent();
    [SerializeField] private float m_gameOverStopTimeSec = 5f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI m_gameOverDisplay = null;
    [SerializeField] private TextMeshProUGUI m_waveDisplay = null;
    [SerializeField] private TextMeshProUGUI m_scoreDisplay = null;

    [Header("Music")]
    [SerializeField] private bool m_enableMusic = true;
    [SerializeField] private List<AudioClip> m_musicLayerList = new List<AudioClip>();

    [Header("Sounds")]
    [SerializeField] private AudioClip m_launchSound = null;
    [SerializeField] private AudioClip m_energyWeaponSound = null;

    public AudioClip LaunchSound => m_launchSound;
    public AudioClip EnergyWeaponSound => m_energyWeaponSound;

    [Header("Waves")]
    [SerializeField] private float m_waveLengthSec = 20f;
    [SerializeField] private int m_allowedPastCount = 5;
    [SerializeField] private float m_enemySpeedBase = 3.5f;
    [SerializeField] private float m_enemySpeedDivider = 5f;
    [SerializeField] private float m_enemySpeedInc = 0.1f;
    [SerializeField] private int m_spawnCountBase = 10;
    [SerializeField] private float m_spawnCountInc = 1.5f;

    private readonly List<Piece> m_pieceList = new List<Piece>();

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
    private bool m_nextWaveStarting = false;

    private List<AudioSource> m_audioSourceList = new List<AudioSource>();

    public bool IsGameOver { get; private set; } = false;

    private float m_timeSinceWaveEndSec = 0f;
    private int m_highScore = 0;
    private int m_maxPieces = 100;

    public void AddPiece( Piece a_piece) {
        m_pieceList.Add(a_piece);
        Debug.Log($"{m_pieceList.Count} pieces");
        if(m_pieceList.Count > m_maxPieces) {
            Destroy(m_pieceList[0].gameObject);
            m_pieceList.RemoveAt(0);
        }
    }

    public void AllSpawned() {
        m_allSpawned = true;
    }

    public void ReadSettings() {
        m_maxPieces = PlayerPrefs.GetInt("Max Pieces");
    }

    public void IncrementCharge() {
        ++m_charge;
    }

    public void EnemyKilled() {
        m_score += m_scorePerKill;
        --m_enemyCount;
    }

    public void IncrementEnemyCount() {
        ++m_enemyCount;

        if (m_enableMusic == false)
            return;

        var enemyTotal = m_enemiesThrough + m_enemyCount;
        var percent = (float)enemyTotal / m_enemySpawner.MaxTotal;
        m_audioSourceList[0].volume = 1f;
        m_audioSourceList[1].volume = Mathf.Min(1f, percent * 2f);
        m_audioSourceList[2].volume = Mathf.Min(1f, (percent - 0.5f) * 2f);
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
        ReadSettings();
        m_gameOverDisplay.enabled = false; 
        m_maxPieces = PlayerPrefs.GetInt("Max Pieces");

        if(m_scoreDisplay == null ) {
            Debug.LogError("Score Manager must have score display");
            Destroy(this);
            return;
        }
        m_startPos = Camera.main.transform.position;
        StartWave(true);

        if (m_enableMusic) {
            foreach (var clip in m_musicLayerList) {
                Debug.Log($"Create music layer {clip}");
                var child = new GameObject();
                var source = child.AddComponent<AudioSource>();
                source.clip = clip;
                source.loop = true;
                source.Play();
                source.volume = 0f;
                child.transform.SetParent(transform);
                child.name = $"Music Layer ({clip})";
                m_audioSourceList.Add(source);
            }
            m_audioSourceList[0].volume = 1f;
        }

        if (m_pauseOnStart)
            Pause();
    }

    AsyncOperation m_pauseMenuOp = null;

    public bool IsPaused { get; private set; } = false;

    private bool m_pauseWasPressed = false;
    private bool m_pausePressed = false;

    public bool PausePressedAndReleased {
        get {
            m_pauseWasPressed = m_pausePressed;
            m_pausePressed = (Gamepad.current != null && Gamepad.current.startButton.isPressed)
               || (Keyboard.current != null && Keyboard.current.escapeKey.isPressed);
            return m_pauseWasPressed && !m_pausePressed;
        }
    }

    private bool FirePressed
        => (Gamepad.current != null && Gamepad.current.aButton.isPressed)
            || (Keyboard.current != null && Keyboard.current.spaceKey.isPressed);

    // TODO move pause/unpause into IsPaused
    private void Unpause() {
        if (IsPaused == false)
            return;
        IsPaused = false;

        ReadSettings();
        Time.timeScale = 1f;
        m_pauseMenuOp = null;
        m_scoreDisplay.enabled = true;
    }

    private void Pause() {
        if (IsPaused)
            return;
        IsPaused = true;

        m_scoreDisplay.enabled = false;
        m_pauseMenuOp = SceneManager.LoadSceneAsync("title", LoadSceneMode.Additive);
        Time.timeScale = 0f;
    }

    private void Update() {
        if (m_pauseMenuOp == null) {
            if (PausePressedAndReleased)
                Pause();
        } else {
            if (m_pauseMenuOp.isDone && Time.timeScale > 0f)
                Unpause();
            return;
        }

        var health = Mathf.Max(0, m_allowedPastCount - m_enemiesThrough);
        var chargePercent = Mathf.FloorToInt(m_charge * 100f / m_chargeForWave);
        m_scoreDisplay.text = $"Wave {m_wave} / Score {m_score} (HI {m_highScore}) / "
            + $"Health {health} / "
            + (chargePercent < 100 ? $"Charge {chargePercent}%" : "CHARGED");

        if (IsGameOver) {
            if (Time.timeScale > 0f) {
                Time.timeScale = Mathf.Max(Time.timeScale - Time.unscaledDeltaTime / m_gameOverStopTimeSec, 0f);
                foreach (var source in m_audioSourceList) {
                    source.volume -= Time.unscaledDeltaTime / (m_gameOverStopTimeSec - 1);
                    var pitchDown = 1 / m_gameOverStopTimeSec * Time.unscaledDeltaTime;
                    source.pitch -= pitchDown;
                }
            }

            if (FirePressed)
                StartWave(true);
            return;
        }

        if(m_nextWaveStarting) {
            m_timeSinceWaveEndSec += Time.unscaledDeltaTime;
            if (m_timeSinceWaveEndSec > 2f) {
                m_timeSinceWaveEndSec = 0f;
                StartWave();
            }
        } else if (m_allSpawned && m_enemyCount == 0 && IsGameOver == false) {
            m_nextWaveStarting = true;
            foreach (var enemy in FindObjectsOfType<Enemy>())
                Destroy(enemy);
            ++m_wave;
            m_waveDisplay.text = $"Wave {m_wave} starting";
            Time.timeScale = 0f;
        }
    }

    private void GameOver() {
        if (IsGameOver) return;

        IsGameOver = true;
        Debug.Log("Game over");
        m_gameOverDisplay.enabled = true;
        m_gameOverDisplay.text = "GAME OVER";
        if (m_score >= m_highScore) {
            PlayerPrefs.SetInt("High Score", m_score);
            m_gameOverDisplay.text += " ~ New high score!";
        }
    }

    private void StartWave(bool a_isFirst = false) {
        if (IsGameOver)
            return;

        m_waveDisplay.text = "";
        m_nextWaveStarting = false;
        Time.timeScale = 1f;

        if (a_isFirst) {
            m_wave = m_startWave;
            m_score = 0;
            m_charge = 0;
            m_enemiesThrough = 0;
            m_highScore = PlayerPrefs.GetInt("High Score");
        } 

        Debug.Log("Next wave");
        Camera.main.transform.position = m_startPos;

        m_enemySpawner.SecBetweenSpawns = m_waveLengthSec / EnemyCountMax;
        m_enemySpawner.MaxTotal = EnemyCountMax;
        m_enemySpawner.ResetSpawner();
        m_onWaveEnd.Invoke();

        foreach (var piece in m_pieceList)
            Destroy(piece.gameObject);
        m_pieceList.Clear();
        foreach (var wave in GameObject.FindGameObjectsWithTag("Wave"))
            Destroy(wave);
        foreach (var bullet in GameObject.FindGameObjectsWithTag("Bullet"))
            Destroy(bullet);

        m_enemiesThrough = 0;
        m_enemyCount = 0;
        m_allSpawned = false;
    }
}
