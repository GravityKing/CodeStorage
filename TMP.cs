using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DamageText : MonoBehaviour
{
    public float destroyTime;
    public float moveSpeed;
    public float alphaSpeed;
    public int damage;

    TextMeshPro dmgText;
    Color alpha;
    void Start()
    {
        dmgText = GetComponent<TextMeshPro>();
        dmgText.text = damage.ToString();
        alpha = dmgText.color;
        Invoke("DestroyDmgText", destroyTime);
        
    }

    // DamageText Flow
    void Update()
    {
        // 데미지 위로 올리기
        transform.Translate(new Vector3(0,moveSpeed * Time.deltaTime,0));
        
        // 데미지 텍스트 페이드 효과
        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed);
        
        dmgText.color = alpha;
        
    }
    void DestroyDmgText()
    {
        Destroy(gameObject);    
    }
}
