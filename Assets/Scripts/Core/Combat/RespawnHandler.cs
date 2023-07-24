using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{
    [SerializeField] private TankPlayer playerPrefab;
    [SerializeField] private float keptCoinPercentage = 50f;

    public override void OnNetworkSpawn()
    {
        if(!IsServer) { return; }

        TankPlayer[] players = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);
        foreach (TankPlayer player in players)
        {
            HandlePlayerSpawned(player);
        }

        TankPlayer.OnPlayerSpawned += HandlePlayerSpawned;
        TankPlayer.OnPlayerSpawned += HandlePlayerDespawned;
    }

    public override void OnNetworkDespawn()
    {
        if(!IsServer) { return; }

        TankPlayer.OnPlayerSpawned -= HandlePlayerSpawned;
        TankPlayer.OnPlayerSpawned -= HandlePlayerDespawned;
    }


    private void HandlePlayerSpawned(TankPlayer player)
    {
        player.Health.OnDie += (health) => HandlePlayerDie(player);
    }

    private void HandlePlayerDespawned(TankPlayer player)
    {
        player.Health.OnDie -= (health) => HandlePlayerDie(player);
    }

    private void HandlePlayerDie(TankPlayer player)
    {
        int keptCoins = (int)(player.Wallet.TotalCoins.Value * (keptCoinPercentage / 100));

        Destroy(player.gameObject);

        StartCoroutine(RespawnPlayer(player.OwnerClientId, keptCoins));
    }

    private IEnumerator RespawnPlayer(ulong ownerClientID, int keptCoins)
    {
        yield return null;

        TankPlayer playerInstance = Instantiate(playerPrefab, SpawnPoint.GetRandomSpawnPosition(), Quaternion.identity);

        playerInstance.NetworkObject.SpawnAsPlayerObject(ownerClientID);
        playerInstance.Wallet.TotalCoins.Value += keptCoins;
    }
}
