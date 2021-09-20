using System;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
// ����Ǿ� �ִ� �Ӽ� �� ���� �����ϱ�
public class RoomManager : Photon.PunBehaviour
{
    Hashtable hash = new Hashtable();
    public static RoomManager instance;
    public int[] pos;
    public Transform[] spawnPos;
    public int myIndex;
    Vector3[] playerpos = {Vector3.up * 5, Vector3.right * 5, Vector3.down * 5,
        Vector3.left * 5};


    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        if (PhotonNetwork.isMasterClient)
        {
            // pos�ʱ�ȭ
            pos = new int[4];
            // hash ����
            SavePos();
        }
        // �ʱ�ȭ�� ������ �޾ƿ�.
        UpdatePos();
        // �� �ڸ� ã��
        myIndex = Array.FindIndex(pos, i => i == 0);
        // ȭ�鿡 �� ��ȣ ǥ��
        GameObject.Find("PlayerIDText").GetComponent<Text>().text = (myIndex + 1).ToString();
        pos[myIndex] = PhotonNetwork.player.ID;
        // hash ����
        SavePos();

        // �� ĳ���� �ڵ����� �����
        GameObject cc = PhotonNetwork.Instantiate("Cube",playerpos[myIndex],Quaternion.identity,0);
    }

    public override void OnPhotonCustomRoomPropertiesChanged(Hashtable propertiesThatChanged)
    {

        UpdatePos();
        print("����� pos�ڷḦ �޾ƿ�.");

    }


    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        // ���常 pos ������ ������Ʈ
        if (false == PhotonNetwork.isMasterClient)
            return;

        int playerIndexNum = Array.FindIndex(pos, i => i == otherPlayer.ID);
        pos[playerIndexNum] = 0;
        // hash ����
        SavePos();
    }
    void SavePos()
    {
        hash.Clear();
        hash["Pos_Index"] = pos;
        PhotonNetwork.room.SetCustomProperties(hash);
    }

    void UpdatePos()
    {
        pos = (int[])PhotonNetwork.room.CustomProperties["Pos_Index"];
    }
}
