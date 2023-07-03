using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MinuteSecondText : MonoBehaviour
{
    public Text textTime;
    float playTime;

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
}
