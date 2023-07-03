using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Metalive;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Linq;

public class GetLastTextPosition : MonoBehaviour
{
    public class TheaterUI
    {
        public TMP_Text head;
        public RectTransform body;
        public TMP_Text discription;
        public Button link;
        [HideInInspector]
        public float fontHeight;
    }

    // 링크 버튼 업데이트
    private void UpdateLinkButton(TheaterUI discription, string linkPath)
    {
        discription.link.onClick.AddListener(() => Application.OpenURL(linkPath));
        discription.discription.ForceMeshUpdate();

        bool isTextEmpty = string.IsNullOrEmpty(discription.discription.text);
        if (!isTextEmpty)
        {
            // 해당 discription의 마지막 글자 Position 가져오기
            int characterCount = discription.discription.textInfo.characterCount;
            int index = characterCount == 0 ? 0 : discription.discription.textInfo.characterCount - 1;
            TMP_CharacterInfo charInfo = discription.discription.textInfo.characterInfo[index];
            Vector3 linkPos = discription.discription.transform.TransformPoint((charInfo.bottomLeft + charInfo.topRight) / 2);
            
            // LinkButton 해당 position으로 변경
            discription.link.transform.position = linkPos;
        }
        else
        {
            discription.link.GetComponent<RectTransform>().anchoredPosition = new Vector2(-35, -35);
        }
        discription.link.gameObject.SetActive(true);
    }
}
