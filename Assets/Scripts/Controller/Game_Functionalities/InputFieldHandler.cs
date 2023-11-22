using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldHandler : MonoBehaviourPun
{
    [SerializeField] TMP_InputField characterNickName;
    [SerializeField] TMP_InputField roomName;
    [SerializeField] TMP_InputField maxPlayers;
    public const string playerNamePrefKey = "Charango";
    public const string roomNamePrefKey = "Room";
    public const string maxPlayersPrefKey = "3";

    public TMP_InputField CharacterNickName { get => characterNickName; set => characterNickName = value; }
    public TMP_InputField RoomName { get => roomName; set => roomName = value; }
    public TMP_InputField MaxPlayers { get => maxPlayers; set => maxPlayers = value; }

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            HandleName(characterNickName, playerNamePrefKey);
            HandleName(roomName, roomNamePrefKey);
            HandleName(maxPlayers, maxPlayersPrefKey);
        }
    }

    public void HandleName(TMP_InputField textInputName, string defaultInputName)
    {

        string defaultName = string.Empty;
        TMP_InputField _inputField = textInputName;
        if (_inputField != null)
        {
            if (PlayerPrefs.HasKey(defaultInputName))
            {
                defaultName = PlayerPrefs.GetString(defaultInputName);
                _inputField.text = defaultName;
            }
        }

    }

    public void SetPlayerInputName()
    {
        PlayerPrefs.SetString(playerNamePrefKey, characterNickName.text);
        // #Important
        if (string.IsNullOrEmpty(characterNickName.text))
        {
            return;
        }
        PhotonNetwork.NickName = characterNickName.text;
        PlayerPrefs.SetString(playerNamePrefKey, characterNickName.text);
    }

    public void SetRoomInputName()
    {

        PlayerPrefs.SetString(roomNamePrefKey, roomName.text);
        // #Important
        if (string.IsNullOrEmpty(roomName.text))
        {
            return;
        }
        //PhotonNetwork.CurrentRoom.Name = room.text;
        PlayerPrefs.SetString(roomNamePrefKey, roomName.text);
    }
    public void SetRoomMaxPlayers()
    {

        PlayerPrefs.SetString(maxPlayersPrefKey, maxPlayers.text);
        // #Important
        if (string.IsNullOrEmpty(maxPlayers.text))
        {
            return;
        }
        //PhotonNetwork.CurrentRoom.Name = room.text;
        PlayerPrefs.SetString(maxPlayersPrefKey, maxPlayers.text);
    }
}
