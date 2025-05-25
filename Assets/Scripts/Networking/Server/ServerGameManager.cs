using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerGameManager : IDisposable
{
    private NetworkObject playerPrefab;
    private string serverIP;
    private int serverPort;
    private int queryPort;
    private MatchplayBackfiller backfiller;
    #if UNITY_SERVER
    private MultiplayAllocationService multiplayAllocationService;
    #endif
    private Dictionary<string, int> teamIdToTeamIndex = new Dictionary<string, int>();  
    public NetworkServer NetworkServer { get; private set; }

    public ServerGameManager(string serverIP, int serverPort, int queryPort, NetworkManager manager, NetworkObject playerPrefab)
    {
        this.serverIP = serverIP;
        this.serverPort = serverPort;
        this.queryPort = queryPort;
        this.playerPrefab = playerPrefab;   
        NetworkServer = new NetworkServer(manager, playerPrefab);
    #if UNITY_SERVER
        multiplayAllocationService = new MultiplayAllocationService();
    #endif
    }

    public async Task StartGameServerAsync()
    {
#if UNITY_SERVER
        
        await multiplayAllocationService.BeginServerCheck();

        try
        {
            MatchmakingResults matchmakerPayload = await GetMatchmakerPayload();

            if(matchmakerPayload != null)
            {
                await StartBackfill(matchmakerPayload);
                NetworkServer.OnUserJoined += UserJoined;
                NetworkServer.OnUserLeft += UserLeft;
            }
            else
            {
                Debug.LogWarning("Matchmaker payload timed out");
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        }

        if(!NetworkServer.OpenConnection(serverIP, serverPort))
        {
            Debug.LogWarning("NetworkServer did not start as expected");
            return;
        }
#endif
    }


#if UNITY_SERVER
    private async Task<MatchmakingResults> GetMatchmakerPayload()
    {
        Task<MatchmakingResults> matchmakerPayloadTask = multiplayAllocationService.SubscribeAndAwaitMatchmakerAllocation();
        if(await Task.WhenAny(matchmakerPayloadTask, Task.Delay(20000)) == matchmakerPayloadTask)
        {
            return matchmakerPayloadTask.Result;
        }

        return null;
    }

    private async Task StartBackfill(MatchmakingResults payload)
    {
        backfiller = new MatchplayBackfiller($"{serverIP}:{serverPort}", payload.QueueName, payload.MatchProperties, 20);

        if(backfiller.NeedsPlayers())
        {
            await backfiller.BeginBackfilling();
        }
    }

    private void UserJoined(UserData user)
    {
        Team team = backfiller.GetTeamByUserId(user.userAuthId);
        if(!teamIdToTeamIndex.TryGetValue(team.TeamId, out int teamIndex))
        {
            teamIndex = teamIdToTeamIndex.Count;
            teamIdToTeamIndex.Add(team.TeamId, teamIndex);
        }
        user.teamIndex = teamIndex;

        multiplayAllocationService.AddPlayer();
        if(!backfiller.NeedsPlayers() && backfiller.IsBackfilling)
        {
            _ = backfiller.StopBackfill();
        }
    }

    private void UserLeft(UserData user)
    {
        int playerCount = backfiller.RemovePlayerFromMatch(user.userAuthId);
        multiplayAllocationService.RemovePlayer();

        if(playerCount <= 0)
        {
            CloseServer();
            return;
        }
        
        if(backfiller.NeedsPlayers() && !backfiller.IsBackfilling)
        {
             _ = backfiller.BeginBackfilling();
        }
    }

    private async void CloseServer()
    {
        await backfiller.StopBackfill();
        Dispose();
        Application.Quit(queryPort);
    }
#endif

    public void Dispose()
    {
#if UNITY_SERVER
        NetworkServer.OnUserJoined -= UserJoined;
        NetworkServer.OnUserLeft -= UserLeft;

        multiplayAllocationService?.Dispose();  
        NetworkServer?.Dispose();   
        backfiller?.Dispose();
#endif
    }
}
