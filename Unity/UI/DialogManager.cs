using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class DialogManager : MonoBehaviour
{
    public Text dialog;
    public GameObject loading;
    public int dialogNum = 0;
    void Start()
    {
        StartCoroutine(IEChat());
    }

    private void Update()
    {
        if (dialogNum < 5 && Input.GetButtonDown("Fire1"))
        {
            if (chatTextIndex < chatText[dialogNum].Length)
            {
                chatTextIndex = chatText[dialogNum].Length;
            }
            else
            {
                dialogNum++;
                chatTextIndex = 1;
            }
        }
    }

    string[] chatText = {
        "안녕하세요 저는 당신의 부관입니다.",
        "현재 상황이 매우 긴박하니 짧게 말하겠습니다.",
         "현재 적군이 저희 도시를 마구 파괴하며 심각한 피해를 입히고 사령관님의 병력 외에는 도시 안에 병력이 없는 상황입니다.",
         "따라서 당장 병력을 투입시켜 적군을 막아야 합니다.",
         "건투를 빌겠습니다 사령관.",
    };
    int chatTextIndex = 1;

    IEnumerator IEChat()
    {
        while (true)
        {
            if (dialogNum < 5)
            {
                dialog.text = chatText[dialogNum].Substring(0, chatTextIndex);
                chatTextIndex = Mathf.Min(++chatTextIndex, chatText[dialogNum].Length);
                yield return new WaitForSeconds(0.01f);
            }
            else
            {
                dialog.gameObject.SetActive(false);
                loading.gameObject.SetActive(true);

                yield return new WaitForSeconds(4f);

                Time.timeScale = 0;
                SceneManager.LoadScene(1);
                //loading.gameObject.SetActive(false);
            }
            yield return 0;
        }
    }

    public void OnClickSkip()
    {
        print("스킵 버튼");
        dialogNum = 5;
    }
}
