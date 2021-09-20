using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ClientManager : Photon.PunBehaviour
{
    public PhotonLogLevel logLevel;
    string gameVersion = "1";
    byte maxPlayerPerRoom = 4;
    private void Awake()
    {
        PhotonNetwork.logLevel = logLevel;
    }
    private void Start()
    {
        PhotonNetwork.autoJoinLobby = false;
        PhotonNetwork.automaticallySyncScene = true;
    }
    public override void OnConnectedToMaster()
    {
        print("OnConnectedToMaster");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = maxPlayerPerRoom }, null);
    }

    public override void OnJoinedRoom()
    {
        print("OnJoinedRoom");
        //PhotonNetwork.LoadLevel("LobbyRoom");
        PhotonNetwork.LoadLevel("Room");
    }

    public override void OnDisconnectedFromPhoton()
    {
        print("OnDisconnectedFromPhoton : Client");
    }

    public void OnClickJoinLobby()
    {
        if (PhotonNetwork.connected)
        {
            PhotonNetwork.JoinRandomRoom();
        }


        else
        {
            PhotonNetwork.ConnectUsingSettings(gameVersion);
        }
    }
}
