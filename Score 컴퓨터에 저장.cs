using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
public class ScroeManager : MonoBehaviour
{
    private int score;
    public Text scoreText;

    public int SCORE
    {
        get { return score; }
        set
        {
            score = value;
            scoreText.text = "Score : " + score.ToString();

            // ���̽��ھ� ���� ����
            if (score > highScore)
            {
                HIGHSCORE = score;

                // ���̽��ھ� ���Ϸ� ����
                PlayerPrefs.SetInt("highScore", highScore);
            }
        }
    }

    private int highScore;
    public Text highScoreText;
    public int HIGHSCORE
    {
        get { return highScore; }
        set
        {
            highScore = value;
            highScoreText.text = "HighScore : " + highScore.ToString();
        }
    }

    public static ScroeManager instance;
    private void Awake()
    {
        ScroeManager.instance = this; // �̱���ȭ
    }

    void Start()
    {
        HIGHSCORE = PlayerPrefs.GetInt("highScore",0);
        SCORE = 0; // set�Լ� ȣ��
    }

    void Update()
    {
        
    }

}
