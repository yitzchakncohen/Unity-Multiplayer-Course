using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class PlayerNameDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TankPlayer player;
    [SerializeField] private TMP_Text playerNameText;

    private void Start() 
    {
        HandlePlayerNameChanged(string.Empty, player.PlayerName.Value);

        player.PlayerName.OnValueChanged += HandlePlayerNameChanged;

    }

    private void OnDestroy() 
    {
        player.PlayerName.OnValueChanged -= HandlePlayerNameChanged;
    }

    private void HandlePlayerNameChanged(FixedString32Bytes oldName, FixedString32Bytes newName)
    {
        playerNameText.text = newName.ToString();
    }

}
