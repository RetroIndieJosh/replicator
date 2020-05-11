using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AimControl : MonoBehaviour, Controls.IAimActions
{
    [SerializeField] private Vector2 m_speed = Vector2.one * 0.5f;
    [SerializeField] private Vector2 m_maxAngle = Vector2.one * 90f;

    private bool m_invertY = true;
    private Vector2 m_sensitivity = Vector2.one;
    private Vector2 m_turnSpeed = Vector2.zero;

    public void ReadSettings() {
        m_invertY = PlayerPrefs.GetInt("Invert Y") == 1;
        m_sensitivity = new Vector2(
            2f * PlayerPrefs.GetInt("Aim Sensitivity X") / 25f + 1f,
            2f * PlayerPrefs.GetInt("Aim Sensitivity Y") / 25f + 1f
        );
        if (m_sensitivity.x < 0 || m_sensitivity.x == Mathf.Infinity)
            m_sensitivity.x = 1f;
        if (m_sensitivity.y < 0 || m_sensitivity.y == Mathf.Infinity)
            m_sensitivity.y = 1f;
    }

    public void OnAim(InputAction.CallbackContext context) {
        if (context.started || context.performed)
            m_turnSpeed = context.ReadValue<Vector2>();
        if (context.canceled)
            m_turnSpeed = Vector2.zero;
    }

    private void Start() {
        var controller = GetComponent<Controller>();
        if (controller == null) {
            Destroy(this);
            return;
        }
        controller.Controls.Aim.SetCallbacks(this);
        ReadSettings();
    }

    // this whole x/y thing is very confusing but it's all correct, knowing
    // "x turn" means "turn on y axis" and vice versa
    private void Update() {
        if (Time.timeScale == 0f || ScoreManager.instance.IsPaused)
            return;

        var x = transform.rotation.eulerAngles.x;
        var y = transform.rotation.eulerAngles.y;
        if (m_turnSpeed.magnitude > Mathf.Epsilon) {
            var turn = new Vector3(m_turnSpeed.y, m_turnSpeed.x, 0f).normalized;
            //Debug.Log($"Turn {turn}");
            if (m_invertY)
                turn.x = -turn.x;
            x -= turn.x * m_speed.x * m_sensitivity.y;
            y += turn.y * m_speed.y * m_sensitivity.x;
        }
        x = ClampAngle(x, m_maxAngle.x);
        y = ClampAngle(y, m_maxAngle.y);
        transform.eulerAngles = new Vector3(x, y, 0f);
    }

    float ClampAngle(float a_angle, float a_maxDistance) {
        while (a_angle < 0)
            a_angle += 360;
        while (a_angle > 360)
            a_angle -= 360;
        if (a_angle > 180)
            return Mathf.Max(a_angle, 360 - a_maxDistance);
        return Mathf.Min(a_angle, a_maxDistance);
    }
}
