using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DealDamageOnContact : MonoBehaviour
{
    [SerializeField] private Projectile projectile;
    [SerializeField] private int damage = 5;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.attachedRigidbody == null) {return;}

        if(projectile.TeamIndex != -1)
        {
            if(other.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player))
            {
                if(player.TeamIndex.Value == projectile.TeamIndex)
                {
                    return;
                }
            }
        }

        if(other.attachedRigidbody.TryGetComponent<Health>(out Health health))
        {
            health.TakeDamage(damage);
        }
    }
}
