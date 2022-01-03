using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JH_Singletone : MonoBehaviour
{
    private static JH_Singletone instance;
    public static JH_Singletone Instance
    {
        get
        {
            // ¡ﬂ∫π ΩÃ±€≈Ê πÊ¡ˆ
            if (instance == null)
            {
                var obj = FindObjectOfType<JH_Singletone>();
                if (obj != null)
                {
                    instance = obj;
                }
                else
                {
                    var newObj = new GameObject().AddComponent<JH_Singletone>();
                    instance = newObj;
                }
            }
            return instance;
        }

    }

    public int a;
    public int b;

    private void Awake()
    {
        // ¡ﬂ∫π ΩÃ±€≈Ê πÊ¡ˆ
        var objs = FindObjectsOfType<JH_Singletone>();

        if (objs.Length != 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

}
