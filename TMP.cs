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
        // ������ ���� �ø���
        transform.Translate(new Vector3(0,moveSpeed * Time.deltaTime,0));
        
        // ������ �ؽ�Ʈ ���̵� ȿ��
        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed);
        
        dmgText.color = alpha;
        
    }
    void DestroyDmgText()
    {
        Destroy(gameObject);    
    }
}
