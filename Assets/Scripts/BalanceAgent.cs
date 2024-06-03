using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalanceAgent : MonoBehaviour
{
    [Header("Stabilizer")]
    [Range(1000, 10000)]
    [SerializeField] float m_stabilizerTorque = 4000f;
    float m_minStabilizerTorque = 1000;
    float m_maxStabilizerTorque = 10000;
    [SerializeField] Stabilizer hipsStabilizer;
    [SerializeField] Stabilizer spineStabilizer;

    // Start is called before the first frame update
    void Start()
    {
        hipsStabilizer.uprightTorque = m_stabilizerTorque;
        spineStabilizer.uprightTorque = m_stabilizerTorque;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
