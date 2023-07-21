using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private static List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

    private void OnEnable() 
    {
        spawnPoints.Add(this);
    }

    private void OnDisable() 
    {
        if(spawnPoints.Contains(this))
        {
            spawnPoints.Remove(this);
        }
    }

    public static Vector3 GetRandomSpawnPosition()
    {
        if(spawnPoints.Count == 0) 
        {
            return Vector3.zero;
        }

        return spawnPoints[Random.Range(0, spawnPoints.Count)].transform.position;
    }

    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 1f);
    }
}
