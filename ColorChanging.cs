using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ColorChanging : MonoBehaviour
{
    public GameObject[] skinedChild;
    public GameObject[] child;
    public GameObject damageText;

    void Start()
    {

    }

    void Update()
    {

    }
    // Player에게 피격
    IEnumerator OnDamaged()
    {
        // 피격 시 반짝임
        for (int i = 0; i < child.Length; i++)
        {
            changeAlpha(child[i], 0f);

            for (int j = 0; j < skinedChild.Length; j++)
            {
                Material a = skinedChild[j].GetComponent<SkinnedMeshRenderer>().material;
                a.color = Color.red;
            }
        }

        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < child.Length; i++)
        {
            BackToOriginAlpha(child[i], 1f);

            for (int j = 0; j < skinedChild.Length; j++)
            {
                Material a = skinedChild[j].GetComponent<SkinnedMeshRenderer>().material;
                a.color = Color.white;
            }
        }
    }

    IEnumerator OnDamagedFromUlti()
    {
        // 피격 시 반짝임
        for (int i = 0; i < child.Length; i++)
        {
            changeAlpha(child[i], 0f);

            for (int j = 0; j < skinedChild.Length; j++)
            {
                Material a = skinedChild[j].GetComponent<SkinnedMeshRenderer>().material;
                a.color = Color.red;
            }
        }

        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < child.Length; i++)
        {
            BackToOriginAlpha(child[i], 1f);

            for (int j = 0; j < skinedChild.Length; j++)
            {
                Material a = skinedChild[j].GetComponent<SkinnedMeshRenderer>().material;
                a.color = Color.white;
            }
        }
    }
    void changeAlpha(GameObject targetObj, float newAlpha)
    {
        MeshRenderer[] all = targetObj.GetComponents<MeshRenderer>();
        for (int i = 0; i < all.Length; i++)
        {
            Material cMat = all[i].GetComponent<MeshRenderer>().material;
            all[i].GetComponent<MeshRenderer>().material.color = Color.red;
        }

    }

    void BackToOriginAlpha(GameObject targetObj, float newAlpha)
    {
        MeshRenderer[] all = targetObj.GetComponents<MeshRenderer>();
        for (int i = 0; i < all.Length; i++)
        {
            Material cMat = all[i].GetComponent<MeshRenderer>().material;
            all[i].GetComponent<MeshRenderer>().material.color = Color.white;
        }

    }
}
