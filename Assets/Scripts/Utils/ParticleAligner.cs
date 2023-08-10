using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleAligner : MonoBehaviour
{
    private ParticleSystem.MainModule psMain;

    void Start()
    {
        psMain = GetComponent<ParticleSystem>().main;
    }

    void Update()
    {
        psMain.startRotation = -transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
    }
}
