using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeAll : MonoBehaviour
{
    public void Explode() {
        foreach (var exploder in FindObjectsOfType<Exploder3d>())
            exploder.Explode();
    }
}
