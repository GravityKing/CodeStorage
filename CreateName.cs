using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CreateName : MonoBehaviour
{
    static string playerNamePrefKey = "PlayerName";

    private void Start()
    {
        string defaultName = "";
        InputField inputField = GetComponent<InputField>();
        if (inputField != null)
        {
            if (PlayerPrefs.HasKey(playerNamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                inputField.text = defaultName;
            }
        }

        PhotonNetwork.playerName = defaultName;
    }

    public void OnClickSetPlayerName(string value)
    {
        // 만약 값이 null일 때 발생하는 오류를 방지하기 위해 " " 공간 추가
        PhotonNetwork.playerName = value + " ";
        PlayerPrefs.SetString(playerNamePrefKey, value);
    }

  
}
