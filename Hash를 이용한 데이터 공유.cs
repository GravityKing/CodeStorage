using System;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
// 저장되어 있는 속성 값 서로 공유하기
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
            // pos초기화
            pos = new int[4];
            // hash 저장
            SavePos();
        }
        // 초기화된 내용을 받아옴.
        UpdatePos();
        // 빈 자리 찾기
        myIndex = Array.FindIndex(pos, i => i == 0);
        // 화면에 내 번호 표시
        GameObject.Find("PlayerIDText").GetComponent<Text>().text = (myIndex + 1).ToString();
        pos[myIndex] = PhotonNetwork.player.ID;
        // hash 저장
        SavePos();

        // 내 캐릭터 자동으로 만들기
        GameObject cc = PhotonNetwork.Instantiate("Cube",playerpos[myIndex],Quaternion.identity,0);
    }

    public override void OnPhotonCustomRoomPropertiesChanged(Hashtable propertiesThatChanged)
    {

        UpdatePos();
        print("변경된 pos자료를 받아옴.");

    }


    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        // 방장만 pos 데이터 업데이트
        if (false == PhotonNetwork.isMasterClient)
            return;

        int playerIndexNum = Array.FindIndex(pos, i => i == otherPlayer.ID);
        pos[playerIndexNum] = 0;
        // hash 저장
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
