using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject serverProjectilePrfab;
    [SerializeField] private GameObject clientProjectilePrfab;

    [Header("Settings")]
    [SerializeField] private float projectileSpeed;
    private bool shouldFire;

    public override void OnNetworkSpawn()
    {
        if(!IsOwner) { return; }

        inputReader.PrimaryFireEvent += HandlePrimaryFire;
    }


    public override void OnNetworkDespawn()
    {
        if(!IsOwner) { return; }

        inputReader.PrimaryFireEvent -= HandlePrimaryFire;
    }

    private void Update() 
    {
        if(!IsOwner) { return; }

        if(!shouldFire) { return; }

        PrimaryFireServerRPC(projectileSpawnPoint.position, projectileSpawnPoint.up);
        
        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);
    }

    private void HandlePrimaryFire(bool shouldFire)
    {
        this.shouldFire = shouldFire;
    }

    [ServerRpc]
    private void PrimaryFireServerRPC(Vector3 spawnPosition, Vector3 direction)
    {
        GameObject projectileInstance = Instantiate(serverProjectilePrfab, spawnPosition, Quaternion.identity);

        projectileInstance.transform.up = direction;

        SpawnDummyProjectileClientRPC(spawnPosition, direction);
    }

    [ClientRpc]
    private void SpawnDummyProjectileClientRPC(Vector3 spawnPosition, Vector3 direction)
    {
        if(IsOwner){ return; }

        SpawnDummyProjectile(spawnPosition, direction);
    }

    private void SpawnDummyProjectile(Vector3 spawnPosition, Vector3 direction)
    {
        GameObject projectileInstance = Instantiate(clientProjectilePrfab, spawnPosition, Quaternion.identity);
        
        projectileInstance.transform.up = direction;
    }
}
