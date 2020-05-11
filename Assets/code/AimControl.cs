using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AimControl : MonoBehaviour, Controls.IAimActions
{
    [SerializeField] private Vector2 m_speed = Vector2.one * 0.5f;
    [SerializeField] private Vector2 m_maxAngle = Vector2.one * 90f;

    private bool m_invertY = true;
    private Vector2 m_turnSpeed = Vector2.zero;

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
        m_invertY = PlayerPrefs.GetInt("Invert Y") == 1;
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
            x -= turn.x * m_speed.x;
            y += turn.y * m_speed.y;
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
