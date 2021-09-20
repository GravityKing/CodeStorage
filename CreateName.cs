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
        // ���� ���� null�� �� �߻��ϴ� ������ �����ϱ� ���� " " ���� �߰�
        PhotonNetwork.playerName = value + " ";
        PlayerPrefs.SetString(playerNamePrefKey, value);
    }

  
}
