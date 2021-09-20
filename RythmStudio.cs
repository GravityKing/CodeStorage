using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Ho_RythmStudio : MonoBehaviour
{

    public static Ho_RythmStudio instance;
    public Text textTime;
    public Button startBtn;
    public Button stopBtn;
    public Button restartBtn;
    public Button normalBtn;
    public Button suicideBtn;
    public Button swingBtn;
    public Button continueBtn;
    public Button deleteBtn;
    AudioSource audio;

    private void Awake()
    {

    }
    void Start()
    {
        instance = this;
        Time.timeScale = 0;
        audio = GetComponent<AudioSource>();
        list = new List<string>();
    }

    float playTime;
    int min;
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            list.Add("���� " + min + "�� " + playTime.ToString("#0.00") + "��");
        }

        playTime += Time.deltaTime;

        if (playTime >= 60)
        {
            min++;
            playTime -= 60;
        }

        //textTime.text = string.Format("PlayTime : " + "{0:D2} : {1:D2}", min, (int)playTime);
        textTime.text = min.ToString() + " : " + playTime.ToString("#.00");
    }

    List<string> list;
    public void OnClickNormal()
    {
        Time.timeScale = 0;
        audio.Pause();
        list.Add("�븻���� " + min + "�� " + playTime.ToString("#0.00") + "��");
    }
    public void OnClickSuicide()
    {
        Time.timeScale = 0;
        audio.Pause();
        list.Add("�ڻ����� " + min + "�� " + playTime.ToString("#0.00") + "��");

    }
    public void OnClickSwing()
    {
        Time.timeScale = 0;
        audio.Pause();
        list.Add("�������� " + min + "�� " + playTime.ToString("#0.00") + "��");

    }
    public void OnClickStart()
    {
        Time.timeScale = 1;
        audio.Play();
        startBtn.gameObject.SetActive(false);
        stopBtn.gameObject.SetActive(true);
    }

    public void OnClickContinue()
    {
        Time.timeScale = 1;
        audio.UnPause();
        stopBtn.gameObject.SetActive(true);
        continueBtn.gameObject.SetActive(false);
    }

    public void OnClickStop()
    {
        Time.timeScale = 0;
        audio.Pause();
        continueBtn.gameObject.SetActive(true);
        stopBtn.gameObject.SetActive(false);
    }

    public void OnClickRestart()
    {
        SceneManager.LoadScene(0);
    }

    public void OnClickDelete()
    {
        list.RemoveAt(list.Count - 1);
    }

    // wave�� ���� �� �����
    // �ش� wave�� ���۵Ǹ� bool ���� true�� �Ҵ��Ѵ�
    // Back��ư�� ���� �� true�� bool �� ������ �ڷ�ƾ���� �̵��Ѵ�
    // ������ ���� �ڷ�ƾ�� �ð� ���� �Է¹޴´� .
    public bool isBack;
    public void OnClickBack()
    {
        StartCoroutine(backLogic());
    }

    // �� �ܰ� �� ���̺�� �ڷΰ���
    IEnumerator backLogic()
    {
        Time.timeScale = 0;

        if (Ho_StageManager.instance.isWave2)
        {
            Ho_StageManager.instance.StopAllCoroutines();
            isBack = true;
            audio.Pause();
            audio.time = 0;
            //playTime = 0;

            yield return new WaitForSeconds(0.1f);
            isBack = false;
            Ho_StageManager.instance.StartCoroutine(Ho_StageManager.instance.wave1());
        }

        else if (Ho_StageManager.instance.isWave3)
        {
            Ho_StageManager.instance.StopAllCoroutines();
            isBack = true;
            audio.Pause();
            audio.time = 6.38f;
            //playTime = 6.38f;

            yield return new WaitForSeconds(0.1f);
            isBack = false;
            Ho_StageManager.instance.StartCoroutine(Ho_StageManager.instance.wave2());
        }

        else if (Ho_StageManager.instance.isWave4)
        {
            Ho_StageManager.instance.StopAllCoroutines();
            isBack = true;
            audio.Pause();
            audio.time = 21.09f;

            yield return new WaitForSeconds(0.1f);
            isBack = false;
            Ho_StageManager.instance.StartCoroutine(Ho_StageManager.instance.wave3());
        }

        else if (Ho_StageManager.instance.isWave5)
        {
            Ho_StageManager.instance.StopAllCoroutines();
            isBack = true;
            audio.Pause();
            audio.time = 24.24f;

            yield return new WaitForSeconds(0.1f);
            isBack = false;
            Ho_StageManager.instance.StartCoroutine(Ho_StageManager.instance.wave4());
        }

        else if (Ho_StageManager.instance.isWave6)
        {
            Ho_StageManager.instance.StopAllCoroutines();
            isBack = true;
            audio.Pause();
            audio.time = 26.96f;

            yield return new WaitForSeconds(0.1f);
            isBack = false;
            Ho_StageManager.instance.StartCoroutine(Ho_StageManager.instance.wave5());
        }

        else if (Ho_StageManager.instance.isWave7)
        {
            Ho_StageManager.instance.StopAllCoroutines();
            isBack = true;
            audio.Pause();
            audio.time = 27.72f;

            yield return new WaitForSeconds(0.1f);
            isBack = false;
            Ho_StageManager.instance.StartCoroutine(Ho_StageManager.instance.wave6());
        }

        else if (Ho_StageManager.instance.isWave8)
        {
            Ho_StageManager.instance.StopAllCoroutines();
            isBack = true;
            audio.Pause();
            audio.time = 28.60f;

            yield return new WaitForSeconds(0.1f);
            isBack = false;
            Ho_StageManager.instance.StartCoroutine(Ho_StageManager.instance.wave7());
        }

        else if (Ho_StageManager.instance.isWave9)
        {
            Ho_StageManager.instance.StopAllCoroutines();
            isBack = true;
            audio.Pause();
            audio.time = 28.98f;

            yield return new WaitForSeconds(0.1f);
            isBack = false;
            Ho_StageManager.instance.StartCoroutine(Ho_StageManager.instance.wave8());
        }

        else if (Ho_StageManager.instance.isWave10)
        {
            Ho_StageManager.instance.StopAllCoroutines();
            isBack = true;
            audio.Pause();
            audio.time = 30.58f;

            yield return new WaitForSeconds(0.1f);
            isBack = false;
            Ho_StageManager.instance.StartCoroutine(Ho_StageManager.instance.wave9());
        }

        else if (Ho_StageManager.instance.isWave11)
        {
            Ho_StageManager.instance.StopAllCoroutines();
            isBack = true;
            audio.Pause();
            audio.time = 30.61f;

            yield return new WaitForSeconds(0.1f);
            isBack = false;
            Ho_StageManager.instance.StartCoroutine(Ho_StageManager.instance.wave10());
        }

        else if (Ho_StageManager.instance.isWave12)
        {
            Ho_StageManager.instance.StopAllCoroutines();
            isBack = true;
            audio.Pause();
            audio.time = 38.16f;

            yield return new WaitForSeconds(0.1f);
            isBack = false;
            Ho_StageManager.instance.StartCoroutine(Ho_StageManager.instance.wave11());
        }

        else if (Ho_StageManager.instance.isWave13)
        {
            Ho_StageManager.instance.StopAllCoroutines();
            isBack = true;
            audio.Pause();
            audio.time = 46.62f;

            yield return new WaitForSeconds(0.1f);
            isBack = false;
            Ho_StageManager.instance.StartCoroutine(Ho_StageManager.instance.wave12());

        }

        else if (Ho_StageManager.instance.isWave14)
        {
            Ho_StageManager.instance.StopAllCoroutines();
            isBack = true;
            audio.Pause();
            audio.time = 50.57f;

            yield return new WaitForSeconds(0.1f);
            isBack = false;
            Ho_StageManager.instance.StartCoroutine(Ho_StageManager.instance.wave13());
        }

        else if (Ho_StageManager.instance.isWave15)
        {
            Ho_StageManager.instance.StopAllCoroutines();
            isBack = true;
            audio.Pause();
            audio.time = 52.98f;
            min = 0;
            playTime = 52.98f;

            yield return new WaitForSeconds(0.1f);
            isBack = false;
            Ho_StageManager.instance.StartCoroutine(Ho_StageManager.instance.wave14());
        }

        else if (Ho_StageManager.instance.isWave16)
        {
            Ho_StageManager.instance.StopAllCoroutines();
            isBack = true;
            audio.Pause();
            audio.time = 53.83f;
            min = 0;
            playTime = 53.83f;

            yield return new WaitForSeconds(0.1f);
            isBack = false;
            Ho_StageManager.instance.StartCoroutine(Ho_StageManager.instance.wave15());
        }

        else if (Ho_StageManager.instance.isWave17)
        {
            Ho_StageManager.instance.StopAllCoroutines();
            isBack = true;
            audio.Pause();
            audio.time = 57.42f;
            min = 0;
            playTime = 57.42f;

            yield return new WaitForSeconds(0.1f);
            isBack = false;
            Ho_StageManager.instance.StartCoroutine(Ho_StageManager.instance.wave16());
        }

        else if (Ho_StageManager.instance.isWave18)
        {
            Ho_StageManager.instance.StopAllCoroutines();
            isBack = true;
            audio.Pause();
            audio.time = 64.05f;
            min = 0;
            playTime = 64.05f;

            yield return new WaitForSeconds(0.1f);
            isBack = false;
            Ho_StageManager.instance.StartCoroutine(Ho_StageManager.instance.wave17());
        }
    }
    // �� ����Ʈ�� �ð��� ���� ������ ���ؼ� ���
    public void OnClickGetLists()
    {

        for (int i = 0; i < list.Count; i++)
        {
            print(list[i]);
        }
    }
}



