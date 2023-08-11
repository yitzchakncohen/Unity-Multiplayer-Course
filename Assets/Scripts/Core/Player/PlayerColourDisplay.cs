using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColourDisplay : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] playersSprites;
    [SerializeField] private TankPlayer player;
    [SerializeField] private TeamColourLookup teamColourLookup;

    private void Start() 
    {
        HandleTeamIndexChanged(-1, player.TeamIndex.Value);

        player.TeamIndex.OnValueChanged += HandleTeamIndexChanged;
    }

    private void OnDestroy() 
    {
        player.TeamIndex.OnValueChanged -= HandleTeamIndexChanged;
    }

    private void HandleTeamIndexChanged(int oldTeamIndex, int newTeamIndex)
    {
        Color teamColour = teamColourLookup.GetTeamColour(newTeamIndex);

        foreach (SpriteRenderer spriteRenderer in playersSprites)
        {
            spriteRenderer.color = teamColour;
        }
    }    
}
