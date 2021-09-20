using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ����ü���� �ִ�ü������ ���� ä��� �ʹ�
// ü���� ����� �� UI�� �Բ� �ٲٰ� �ʹ�
// 
public class EnemyHP : MonoBehaviour
{
    public int curHP;
    public int maxHP = 3;
    public Slider sliderHP;
    
    public int HP
    {
        get { return curHP;  }
        set { 
            curHP = value;
            sliderHP.value = curHP;
        }
    }
    void Start()
    {
        sliderHP.maxValue = maxHP;
        HP = maxHP;
    }

    void Update()
    {
        
    }
}
