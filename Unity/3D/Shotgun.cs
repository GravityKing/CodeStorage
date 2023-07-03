using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet2 : MonoBehaviour
{
    public GameObject hitEffect;
    public float PlayerBulletDamage = 20f;
    public float PlayerBulletSpeed = 1000f;
    public Transform playerFirePos2;
    //public Camera playerCamera2;

    //랜덤 총알 변수 포지션을 여기서 구해야겠네요
    //카메라 최종 좌표 * ram값.

    void Start()
    {
        
        float bulletRanX = Random.Range(-5f, 5f);
        float bulletRanY = Random.Range(-5f, 5f);

        Vector3 ranVec = new Vector3(bulletRanX, bulletRanY, 0);

        var Point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
        //var Point = playerCamera2.ScreenToWorldPoint(new Vector3(Input.mousePosition.x , Input.mousePosition.y , -playerCamera2.transform.position.z));
        //Vector3 vec = new Vector3(0.75f, 0.22f, 1.22f); // 차후 값 상속
        Point += ranVec;


        GetComponent<Rigidbody>().AddForce((Point - playerFirePos2.position) * PlayerBulletSpeed * Time.deltaTime, ForceMode.Impulse);
        //Debug.Log(playerCamera2.transform.position);

        

    }


    void Update()
    {
        SelfDestroy(); //총알 자동 파괴
    }


    void OnTriggerEnter(Collider other) // Enemy랑 destroy존 부딪히면 자동으로 총알 없애기 => 나중에 오브젝트 풀링 적용.
    {
        if (other.gameObject.tag == "Enemy" || other.gameObject.name == "PlayerBulletDesroyZone") {
            Destroy(gameObject);
            GameObject Effect = Instantiate(hitEffect, transform.position, transform.rotation);
            //Debug.Log(other.gameObject.name);

        }



    }

    void SelfDestroy() // 트리거 엔터 이후에 파괴되는거 파괴
    {
        Destroy(gameObject, 2f);
    }


}
