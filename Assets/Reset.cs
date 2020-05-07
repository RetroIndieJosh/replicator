using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Reset : MonoBehaviour
{
    public void DoReset() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
