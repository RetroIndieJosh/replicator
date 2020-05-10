using JoshuaMcLean;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class FireControl : MonoBehaviour, Controls.IFireActions
{
    [SerializeField] private GameObject m_energyWeaponPrefab = null;
    [SerializeField] private float m_speed = 10f;

    public void OnFire(InputAction.CallbackContext context) {
        if (context.started == false)
            return;

        if (ScoreManager.instance.IsCharged) {
            ScoreManager.instance.ResetCharge();
            Instantiate(m_energyWeaponPrefab, transform.position, m_energyWeaponPrefab.transform.rotation);
            Debug.Log("Fire energy weapon");
            return;
        }

        var bullet = GetComponent<Spawner>().SpawnNext();
        var origin = transform.position + transform.forward * Camera.main.nearClipPlane * 1.1f;
        //var origin = transform.position - Vector3.forward * 2f;
        var dir = transform.forward;
        bullet.GetComponent<Rigidbody>().velocity = dir.normalized * m_speed;

        bullet.transform.rotation = transform.rotation;
        bullet.transform.Rotate(Vector3.right, 90f);

        bullet.transform.position = origin;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.forward * 10f);
    }

    private void Start() {
        var controller = GetComponent<Controller>();
        if (controller == null) {
            Destroy(this);
            return;
        }
        controller.Controls.Fire.SetCallbacks(this);
    }
}
