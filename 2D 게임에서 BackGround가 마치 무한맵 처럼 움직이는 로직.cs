using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    // 배경이미지를 스크롤 하고 싶다.
public class BackGround : MonoBehaviour
{
    public float speed = 0.1f;
    Material mat; // 이것을 캐시라고 한다

    void Start()
    {
        // Mesh Renderer 안에 있는 Material을 가져오기 위해
        // Gameobject에 있는 MeshRenderer를 먼저 가져온다
        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        mat = renderer.material; //클래스 안에 있는 Material에 Gameobject의 Material을 넣는다
        
    }

    void Update()
    {
        // 배경이미지 스크롤을 하고 싶다.
        mat.mainTextureOffset += Vector2.up * speed * Time.deltaTime;
    }
}
