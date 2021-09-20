using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LobbyManager : Photon.PunBehaviour
{
    [SerializeField] PhotonLogLevel logLevel;
    [SerializeField] GameObject matchingPannel;
    [SerializeField] GameObject matchingButton;
    [SerializeField] int countNeededForGame = 2;
    [SerializeField] Text matchingTime;

    public int index;

    bool isClickLeaveLobby;
    private void Awake()
    {
        PhotonNetwork.logLevel = logLevel;
        PhotonNetwork.autoJoinLobby = false;
        PhotonNetwork.automaticallySyncScene = true;
    }
    public override void OnLeftRoom()
    {
        print("OnLeftLobby");
    }

    public override void OnConnectedToMaster()
    {
        if (isClickLeaveLobby)
            PhotonNetwork.LoadLevel("Client");
    }

    public override void OnDisconnectedFromPhoton()
    {
        print("OnDisconnectedFromPhoton : Lobby");
    }

    public void OnClickLeaveLobby()
    {
        isClickLeaveLobby = true;
        print("OnClickLeaveLobby");
        PhotonNetwork.LeaveRoom();
    }

    public void OnClickJoinRoom()
    {
        print("OnClickJoinRoom");
        matchingButton.SetActive(false);
        matchingPannel.SetActive(true);
        matchingTime.gameObject.SetActive(true);

        // 마스터에서 LoadLevel을 하면 클라이언트까지 함께 Load 된다
        // 그런데 클라이언트에서 로드를 한 번 더 했기 때문에 마스터의 오브젝트가 사라진 것이다
        // 따라서 마스터 클라이언트일 때만 LoadLevel을 선언해야 한다
        if(PhotonNetwork.isMasterClient)
        StartCoroutine(MatchGame());
    }

    IEnumerator MatchGame()
    {
        print("MatchGame");
        float time = 0;
        while (time <= 60)
        {
            matchingTime.text = string.Format("{0:D2}", (int)time);
            time += Time.deltaTime;
            if (IndexManager.instance.indexForShare == countNeededForGame)
            {
                // 방 닫기
                IndexManager.instance.indexForShare = 0;
                PhotonNetwork.LoadLevel("GameRoom");
            }

            yield return null;
        }

        print("현재 게임에 Player가 없습니다.");
    }

}
