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

    //���� �Ѿ� ���� �������� ���⼭ ���ؾ߰ڳ׿�
    //ī�޶� ���� ��ǥ * ram��.

    void Start()
    {
        
        float bulletRanX = Random.Range(-5f, 5f);
        float bulletRanY = Random.Range(-5f, 5f);

        Vector3 ranVec = new Vector3(bulletRanX, bulletRanY, 0);

        var Point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
        //var Point = playerCamera2.ScreenToWorldPoint(new Vector3(Input.mousePosition.x , Input.mousePosition.y , -playerCamera2.transform.position.z));
        //Vector3 vec = new Vector3(0.75f, 0.22f, 1.22f); // ���� �� ���
        Point += ranVec;


        GetComponent<Rigidbody>().AddForce((Point - playerFirePos2.position) * PlayerBulletSpeed * Time.deltaTime, ForceMode.Impulse);
        //Debug.Log(playerCamera2.transform.position);

        

    }


    void Update()
    {
        SelfDestroy(); //�Ѿ� �ڵ� �ı�
    }


    void OnTriggerEnter(Collider other) // Enemy�� destroy�� �ε����� �ڵ����� �Ѿ� ���ֱ� => ���߿� ������Ʈ Ǯ�� ����.
    {
        if (other.gameObject.tag == "Enemy" || other.gameObject.name == "PlayerBulletDesroyZone") {
            Destroy(gameObject);
            GameObject Effect = Instantiate(hitEffect, transform.position, transform.rotation);
            //Debug.Log(other.gameObject.name);

        }



    }

    void SelfDestroy() // Ʈ���� ���� ���Ŀ� �ı��Ǵ°� �ı�
    {
        Destroy(gameObject, 2f);
    }


}
