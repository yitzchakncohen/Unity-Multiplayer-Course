using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private ParticleSystem dustCloud;
    
    [Header("Settings")]
    [SerializeField] private float movementSpeed = 4f;
    [SerializeField] private float turningRate = 270f;
    [SerializeField] private float particleEmmissionValue = 10f;

    private ParticleSystem.EmissionModule emissionModule;

    private Vector2 previousMovementInput;
    private Vector3 previousPosition;

    private const float ParticleStopThreshold = 0.005f;

    private void Awake() 
    {
        emissionModule = dustCloud.emission;
    }

    public override void OnNetworkSpawn()
    {
        if(!IsOwner) { return; }
        
        inputReader.MoveEvent += HandleMove;
    }


    public override void OnNetworkDespawn()
    {
        if(!IsOwner) { return; }
        
        inputReader.MoveEvent -= HandleMove;
    }

    private void FixedUpdate() 
    {
        if((transform.position - previousPosition).sqrMagnitude > ParticleStopThreshold)
        {
            emissionModule.rateOverTime = particleEmmissionValue;
        }
        else
        {
            emissionModule.rateOverTime = 0f;
        }
        previousPosition = transform.position;

        if(!IsOwner) { return; }

        rb.velocity = (Vector2)bodyTransform.up * previousMovementInput.y * movementSpeed;
    }

    private void Update() 
    {
        if(!IsOwner) { return; }

        float zRotation = previousMovementInput.x * - turningRate * Time.deltaTime;

        bodyTransform.Rotate(0f, 0f, zRotation);
    }

    private void HandleMove(Vector2 movementInput)
    {
        previousMovementInput = movementInput;
    }    
}
