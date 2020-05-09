using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public Controls Controls { get; private set; } = null;

    private void Awake() {
        Controls = new Controls();
    }

    private void Start() {
        Controls.Enable();
    }
}
