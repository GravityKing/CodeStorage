using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyHP : MonoBehaviour
{
    public Text enemyName;
    public Slider hpSlider;
    public static EnemyHP enemyHP;
    public float maxHP;
    public float curHP;
    void Start()
    {
        EnemyHP.enemyHP = this;

        
        hpSlider.value = (float)curHP / (float)maxHP;
        gameObject.SetActive(false);
    }

    void Update()
    {
        
        HPdown();
    }

    void HPdown()
    {
        hpSlider.value = Mathf.Lerp(hpSlider.value,(float)curHP / (float) maxHP, Time.deltaTime * 6);
    }
}
