using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawningCoin : Coin
{
    public event Action<RespawningCoin> OnCollected;
    private Vector3 previousPosition;

    public override void OnNetworkSpawn()
    {
        previousPosition = transform.position;
    }

    private void Update() 
    {
        if(IsServer){return;}
        
        if(transform.position != previousPosition)
        {
            Show(true);
            previousPosition = transform.position;
        }
    }

    public override int Collect()
    {
        if(!IsServer)
        {
            Show(false);
            return 0;
        }

        if(alreadyCollected)
        {
            return 0;
        }

        alreadyCollected = true;
        OnCollected?.Invoke(this);
        return coinValue;
    }

    public void Reset()
    {
        alreadyCollected = false;
    }
}
