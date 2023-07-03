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
        "�ȳ��ϼ��� ���� ����� �ΰ��Դϴ�.",
        "���� ��Ȳ�� �ſ� ����ϴ� ª�� ���ϰڽ��ϴ�.",
         "���� ������ ���� ���ø� ���� �ı��ϸ� �ɰ��� ���ظ� ������ ��ɰ����� ���� �ܿ��� ���� �ȿ� ������ ���� ��Ȳ�Դϴ�.",
         "���� ���� ������ ���Խ��� ������ ���ƾ� �մϴ�.",
         "������ ���ڽ��ϴ� ��ɰ�.",
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
        print("��ŵ ��ư");
        dialogNum = 5;
    }
}
