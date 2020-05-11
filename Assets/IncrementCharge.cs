using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncrementCharge : MonoBehaviour
{
    public void DoIt() {
        ScoreManager.instance.IncrementCharge();
    }
}
