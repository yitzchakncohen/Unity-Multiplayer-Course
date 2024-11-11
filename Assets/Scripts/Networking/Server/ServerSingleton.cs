using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Core;
using UnityEngine;

public class ServerSingleton : MonoBehaviour
{
    private static ServerSingleton instance;
    public ServerGameManager GameManager {get; private set;}

    public static ServerSingleton Instance
    {
        get
        {
            if(instance != null)
            {
                return instance;
            }

            instance = FindAnyObjectByType<ServerSingleton>();

            if(instance == null)
            {
                // Debug.Log("No ServerSingleton in the scene!");
                return null;
            }

            return instance;
        }
    }

    private void Start() 
    {
        DontDestroyOnLoad(gameObject);
    }

    public async Task CreateServer(NetworkObject playerPrefab)
    {
        await UnityServices.InitializeAsync();
        GameManager = new ServerGameManager(
            ApplicationData.IP(),
            ApplicationData.Port(),
            ApplicationData.QPort(),
            NetworkManager.Singleton,
            playerPrefab
        );
    }

    private void OnDestroy() 
    {
        GameManager?.Dispose();
    }
}
