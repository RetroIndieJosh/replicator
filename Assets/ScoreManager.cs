using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreManager : MonoBehaviour
{
    [Header("Display")]
    [SerializeField] TextMeshProUGUI m_scoreDisplay = null;
    [SerializeField] TextMeshProUGUI m_multDisplay = null;

    [Header("Settings")]
    [SerializeField] private int m_scorePerKill = 1;
    [SerializeField] private int m_comboPerMultipler = 5;
    [SerializeField] private int m_multiplierMax = 5;

    private int m_score = 0;
    private int m_multiplier = 1;
    private int m_combo = 0;

    public void IncrementScore() {
        Debug.Log("Score up");
        m_score += m_scorePerKill * m_multiplier;
        ++m_combo;
        if (m_combo > m_comboPerMultipler * m_multiplier)
            m_multiplier = Mathf.Min(m_multiplierMax, m_multiplier + 1);
    }

    public void EndCombo() {
        m_combo = 0;
        m_multiplier = 1;
    }

    private void Start() {
        if(m_scoreDisplay == null || m_multDisplay == null) {
            Debug.LogError("Score Manager must have both score and multiplier display");
            Destroy(this);
            return;
        }
    }

    private void Update() {
        m_scoreDisplay.text = $"{m_score}";
        m_multDisplay.text = $"x{m_multiplier} ({m_combo})";
    }
}
