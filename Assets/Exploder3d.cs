using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Exploder3d : MonoBehaviour
{
    [SerializeField] private Rigidbody m_piecePrefab = null;
    [SerializeField] private int m_pieceCount = 10;

    [Header("Force")]
    [SerializeField] private float m_explosionForce = 10f;
    [SerializeField] private float m_explosionRadius = 1f;

    private List<Rigidbody> m_pieceList = new List<Rigidbody>();

    public void Explode() {
        for(var i = 0; i < m_pieceCount; ++i) {
            var pos = Random.insideUnitSphere * m_explosionRadius + transform.position;
            var rot = Random.rotationUniform;
            var piece = Instantiate(m_piecePrefab, pos, rot);
            m_pieceList.Add(piece);
        }
        foreach (var piece in m_pieceList)
            piece.AddExplosionForce(m_explosionForce, transform.position, m_explosionRadius);
        Destroy(gameObject);
    }
}
