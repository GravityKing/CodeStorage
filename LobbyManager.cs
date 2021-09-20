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

        // �����Ϳ��� LoadLevel�� �ϸ� Ŭ���̾�Ʈ���� �Բ� Load �ȴ�
        // �׷��� Ŭ���̾�Ʈ���� �ε带 �� �� �� �߱� ������ �������� ������Ʈ�� ����� ���̴�
        // ���� ������ Ŭ���̾�Ʈ�� ���� LoadLevel�� �����ؾ� �Ѵ�
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
                // �� �ݱ�
                IndexManager.instance.indexForShare = 0;
                PhotonNetwork.LoadLevel("GameRoom");
            }

            yield return null;
        }

        print("���� ���ӿ� Player�� �����ϴ�.");
    }

}
