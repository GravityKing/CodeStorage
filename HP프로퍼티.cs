using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 현재체력을 최대체력으로 가득 채우고 싶다
// 체력이 변경될 때 UI도 함께 바꾸고 싶다
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
