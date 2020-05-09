using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FireControl : MonoBehaviour, Controls.IFireActions
{
    public void OnFire(InputAction.CallbackContext context) {
        var origin = transform.position;
        var dir = transform.forward;
        Debug.Log($"Fire {origin} => {dir}");
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
