using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class ClearManager : MonoBehaviour
{
    public static ClearManager instance;

    public Text textTime;
    public int killCount;
    float playTime;

    public GameObject upStar;
    public GameObject upStar2;
    public GameObject upStar3;

    public GameObject downStar;
    public GameObject downStar2;
    public GameObject downStar3;

    public GameObject rankB;
    public GameObject rankP;
    public GameObject rankD;
    void Start()
    {
        instance = this;
        killCount = 0;

        StartCoroutine(IERank());
    }

    int min = 0;
    void Update()
    {
        playTime += Time.deltaTime;

        if (playTime >= 60)
        {
            min++;
            playTime -= 60;
        }

        textTime.text = string.Format("{0:D2} : {1:D2}", min, (int)playTime);



    }
    public void OnClickRestart()
    {
        SceneManager.LoadScene(0);
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }


    int starCount;
    bool killMission;
    bool timeMission;
    bool hpMission;
    IEnumerator IERank()
    {
        while (true)
        {
            if (EnemyManager.enemyManager.toRound4 == 0)
            {

                if (killCount == 17)
                {
                    starCount++;
                    killMission = true;
                }

                if ((min * 60) + playTime < 150)
                {
                    starCount++;
                    timeMission = true;
                }

                if (PlayerHp.instance.playerHp + Player2Hp.instance.player2Hp >= 50)
                {
                    starCount++;
                    hpMission = true;
                }

                print("클리어");
                print(starCount);

                switch (starCount)
                {
                    // 별 0개 브론즈
                    case 0:
                        rankB.gameObject.SetActive(true);
                        break;

                    // 별 1개 브론즈
                    case 1:
                        if (killMission)
                        {
                            upStar.GetComponent<Toggle>().isOn = true;
                            downStar.GetComponent<Toggle>().isOn = true;
                        }

                        else if (timeMission)
                        {
                            upStar2.GetComponent<Toggle>().isOn = true;
                            downStar2.GetComponent<Toggle>().isOn = true;
                        }

                        else if (hpMission)
                        {
                            upStar3.GetComponent<Toggle>().isOn = true;
                            downStar3.GetComponent<Toggle>().isOn = true;
                        }

                        rankB.gameObject.SetActive(true);
                        break;

                    // 별 2개 플래티넘
                    case 2:
                        if (killMission && timeMission)
                        {
                            upStar.GetComponent<Toggle>().isOn = true;
                            downStar.GetComponent<Toggle>().isOn = true;
                            upStar2.GetComponent<Toggle>().isOn = true;
                            downStar2.GetComponent<Toggle>().isOn = true;
                        }

                        else if (timeMission && hpMission)
                        {
                            upStar2.GetComponent<Toggle>().isOn = true;
                            downStar2.GetComponent<Toggle>().isOn = true;
                            upStar3.GetComponent<Toggle>().isOn = true;
                            downStar3.GetComponent<Toggle>().isOn = true;
                        }

                        else if (killMission && hpMission)
                        {
                            upStar3.GetComponent<Toggle>().isOn = true;
                            downStar3.GetComponent<Toggle>().isOn = true;
                            upStar.GetComponent<Toggle>().isOn = true;
                            downStar.GetComponent<Toggle>().isOn = true;
                        }

                        rankP.gameObject.SetActive(true);
                        break;

                    // 별 3개 다이아
                    case 3:
                        upStar.GetComponent<Toggle>().isOn = true;
                        downStar.GetComponent<Toggle>().isOn = true;
                        upStar2.GetComponent<Toggle>().isOn = true;
                        downStar2.GetComponent<Toggle>().isOn = true;
                        upStar3.GetComponent<Toggle>().isOn = true;
                        downStar3.GetComponent<Toggle>().isOn = true;
                        rankD.gameObject.SetActive(true);
                        break;

                }

                break;
            }

            else
                yield return 0;

        }

    }
}
