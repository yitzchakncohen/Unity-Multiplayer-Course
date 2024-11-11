using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Multiplayer.Playmode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
// using Unity.Multiplayer.Playmode;

public class NameSelector : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private Button connectButton;
    [SerializeField] private int minNameLength = 1;
    [SerializeField] private int maxNameLength = 12;

    public const string PlayerNameKey = "PlayerName";
    public static string PlayModeTag { get; private set; }

    private void Start() 
    {
        if(CurrentPlayer.ReadOnlyTags().Length > 0)
        {
            PlayModeTag = CurrentPlayer.ReadOnlyTags()[0];
        }
        else
        {
            PlayModeTag = "None";
            Debug.LogWarning("No playmode tag set for this instance.");
        }

        // Check for headless server
        if(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            return;
        }
        
        nameField.text = PlayerPrefs.GetString(PlayerNameKey + PlayModeTag, string.Empty);
        HandleNameChanged();
    }

    public void HandleNameChanged()
    {
        connectButton.interactable = nameField.text.Length >= minNameLength && nameField.text.Length < maxNameLength;
    }

    public void Connect()
    {
        PlayerPrefs.SetString(PlayerNameKey + NameSelector.PlayModeTag, nameField.text);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
