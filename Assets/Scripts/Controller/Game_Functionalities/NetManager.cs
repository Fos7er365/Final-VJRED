using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetManager : MonoBehaviourPunCallbacks
{

    [SerializeField] Button btnConnection;
    [SerializeField] TextMeshProUGUI connectionStatus;
    [SerializeField] InputFieldHandler inputFieldHandler;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        btnConnection.interactable = false;

        connectionStatus.text = "Connecting to Master";

    }

    public override void OnConnectedToMaster()
    {
        btnConnection.interactable = false;
        PhotonNetwork.JoinLobby();

        connectionStatus.text = "Connecting to Lobby";
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        connectionStatus.text = "Connection with master has failed, cause was: " + cause;
    }

    public override void OnJoinedLobby()
    {
        btnConnection.interactable = true;

        connectionStatus.text = "Connected to Lobby";
    }
    public override void OnLeftLobby()
    {
        base.OnLeftLobby();

        connectionStatus.text = "Disconnected from lobby";
    }

    public void Connect()
    {

        RoomOptions options = new RoomOptions();
        byte maxPlayer;
        Debug.Log("AAAAAAAAAAAA");
        if (byte.TryParse(inputFieldHandler.MaxPlayers.text, out maxPlayer))
        {
            options.MaxPlayers = maxPlayer;

            options.IsOpen = true;
            options.IsVisible = true;

            PhotonNetwork.JoinOrCreateRoom(inputFieldHandler.RoomName.text, options, TypedLobby.Default);
            Debug.Log("Connected and room created");
            btnConnection.interactable = false;
        }
    }

    public override void OnCreatedRoom()
    {

        connectionStatus.text = "Room " + inputFieldHandler.RoomName.text + " was created!";
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {

        connectionStatus.text = "Failed to create room " + inputFieldHandler.RoomName.text;

        btnConnection.interactable = true;

    }

    public override void OnJoinedRoom()
    {
        connectionStatus.text = "Joined room";
        PhotonNetwork.LoadLevel("Hybrid");
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        connectionStatus.text = "Failed to join room " + inputFieldHandler.RoomName.text;
        btnConnection.interactable = true;
    }

}
