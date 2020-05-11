using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    void Start() {
        ScoreManager.instance.AddPiece(this);
    }
}
