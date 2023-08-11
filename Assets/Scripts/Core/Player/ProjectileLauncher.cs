using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject serverProjectilePrfab;
    [SerializeField] private GameObject clientProjectilePrfab;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private CoinWallet coinWallet;
    [SerializeField] private TankPlayer player;

    [Header("Settings")]
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float fireRate;
    [SerializeField] private float muzzleFlashDuration;
    [SerializeField] private int costToFire;
    private bool isPointerOverUI;
    private bool shouldFire;
    private float timer;
    private float muzzleFlashTimer;

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
        if(muzzleFlashTimer > 0)
        {
            muzzleFlashTimer -= Time.deltaTime;

            if(muzzleFlashTimer <= 0f)
            {
                muzzleFlash.SetActive(false);
            }
        }

        if(!IsOwner) { return; }
        
        isPointerOverUI = EventSystem.current.IsPointerOverGameObject();

        if(timer > 0)
        {
            timer -= Time.deltaTime;
        }

        if(!shouldFire) { return; }

        if(timer > 0 ) { return; }

        if(coinWallet.TotalCoins.Value < costToFire) {return;}

        PrimaryFireServerRPC(projectileSpawnPoint.position, projectileSpawnPoint.up);
        
        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up, player.TeamIndex.Value);

        timer = 1 / fireRate;
    }

    private void HandlePrimaryFire(bool shouldFire)
    {
        if(shouldFire)
        {
            if(isPointerOverUI) { return; } 
        }
        
        this.shouldFire = shouldFire;
    }

    [ServerRpc]
    private void PrimaryFireServerRPC(Vector3 spawnPosition, Vector3 direction)
    {
        if(coinWallet.TotalCoins.Value < costToFire) {return;}
        
        coinWallet.SpendCoins(costToFire);

        GameObject projectileInstance = Instantiate(serverProjectilePrfab, spawnPosition, Quaternion.identity);

        projectileInstance.transform.up = direction;

        Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());

        if(projectileInstance.TryGetComponent<Projectile>(out Projectile projectile))
        {
            projectile.Initialise(player.TeamIndex.Value);
        }

        if(projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * projectileSpeed;
        }

        SpawnDummyProjectileClientRPC(spawnPosition, direction, player.TeamIndex.Value);
    }

    [ClientRpc]
    private void SpawnDummyProjectileClientRPC(Vector3 spawnPosition, Vector3 direction, int teamIndex)
    {
        if(IsOwner){ return; }

        SpawnDummyProjectile(spawnPosition, direction, teamIndex);
    }

    private void SpawnDummyProjectile(Vector3 spawnPosition, Vector3 direction, int teamIndex)
    {
        muzzleFlash.SetActive(true);
        muzzleFlashTimer = muzzleFlashDuration;

        GameObject projectileInstance = Instantiate(clientProjectilePrfab, spawnPosition, Quaternion.identity);
        
        projectileInstance.transform.up = direction;

        Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());

        if(projectileInstance.TryGetComponent<Projectile>(out Projectile projectile))
        {
            projectile.Initialise(teamIndex);
        }

        if(projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * projectileSpeed;
        }
    }
}
