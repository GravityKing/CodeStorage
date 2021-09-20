using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexManager : Photon.PunBehaviour,IPunObservable
{
    public static IndexManager instance;
    public int index;
    public int indexForShare;

    bool isDontDestroyOnLoad;
    private void Awake()
    {
        if (isDontDestroyOnLoad)
            return;

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void OnClickMatchingBtn()
    {
        index = indexForShare;
        indexForShare++;
        photonView.RPC("ShareIndex", PhotonTargets.Others,indexForShare);
        isDontDestroyOnLoad = true;
    }
    // 추후 GameScene의 Manager에서
    // index = IndexManager.instance.index;
    // 위와 같이 index 값을 넘겨받는다.

    [PunRPC]
    public void ShareIndex(int value)
    {
        indexForShare = value;
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(indexForShare);
        }
        else
        {
            indexForShare = (int)stream.ReceiveNext();
        }
    }
}
