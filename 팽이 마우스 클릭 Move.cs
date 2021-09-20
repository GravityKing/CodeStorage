using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : Photon.PunBehaviour
{
    [SerializeField] float speed = 10;
    [SerializeField] float boosterSpeed = 60;
    [SerializeField] Text name;


    Rigidbody rigid;
    Vector3 dir;
    Vector3 preDir;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        name.text = photonView.owner.NickName;
    }

    private void Start()
    {
        if(photonView.isMine)
        StartCoroutine(MyCo());
    }
    private void Update()
    {
        if (false == photonView.isMine)
            return;

        Move();
        Booster();

    }

    void Move()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            dir = new Vector3(hit.point.x - transform.position.x, 0, hit.point.z - transform.position.z);
            if (Input.GetMouseButtonDown(1))
            {
                preDir = dir;
                //rigid.AddForce(dir.normalized * speed * Time.deltaTime, ForceMode.Impulse);

                photonView.RPC("Addforce", PhotonTargets.All,dir,transform.position,transform.rotation);
                transform.LookAt(hit.point);
            }
        }
    }
    void Booster()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            photonView.RPC("ChangeVelo", PhotonTargets.All,preDir,transform.position,transform.rotation);
        }
    }

    [PunRPC]
    void Addforce(Vector3 value,Vector3 pos,Quaternion rot)
    {
        //rigid.velocity = Vector3.zero;
        transform.rotation = rot;
        transform.position = pos;
        rigid.AddForce(value.normalized * speed * Time.deltaTime, ForceMode.Impulse);
        //rigid.velocity = value * Time.deltaTime;
    }

    [PunRPC]
    void ChangeVelo(Vector3 value, Vector3 pos, Quaternion rot)
    {
        //rigid.velocity = Vector3.zero;
        transform.rotation = rot;
        transform.position = pos;
        rigid.velocity = value * boosterSpeed * Time.deltaTime;
    }

    [PunRPC]
    void ChangeVelo2(Vector3 value, Vector3 pos, Quaternion rot)
    {
        rigid.velocity = value;
        transform.rotation = rot;
        transform.position = pos;
    }

    [PunRPC]
    void ShareVelocity(Vector3 value,Collision coll)
    {
        //coll.transform.GetComponent<Rigidbody>().velocity;
        rigid.velocity = value;
    }

    IEnumerator MyCo()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            photonView.RPC("ChangeVelo2",PhotonTargets.Others,rigid.velocity,transform.position,transform.rotation);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        Vector3 velo = rigid.velocity;
        rigid.velocity = Vector3.zero;
        if (other.transform.CompareTag("Player") && photonView.isMine)
        {
            photonView.RPC("ShareVelocity", PhotonTargets.All, velo,other);
        }

    }



}
