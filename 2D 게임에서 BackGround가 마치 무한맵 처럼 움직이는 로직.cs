using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    // ����̹����� ��ũ�� �ϰ� �ʹ�.
public class BackGround : MonoBehaviour
{
    public float speed = 0.1f;
    Material mat; // �̰��� ĳ�ö�� �Ѵ�

    void Start()
    {
        // Mesh Renderer �ȿ� �ִ� Material�� �������� ����
        // Gameobject�� �ִ� MeshRenderer�� ���� �����´�
        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        mat = renderer.material; //Ŭ���� �ȿ� �ִ� Material�� Gameobject�� Material�� �ִ´�
        
    }

    void Update()
    {
        // ����̹��� ��ũ���� �ϰ� �ʹ�.
        mat.mainTextureOffset += Vector2.up * speed * Time.deltaTime;
    }
}
