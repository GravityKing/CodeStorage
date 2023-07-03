using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class GetImageMetaData : MonoBehaviour
{
    // 이미지 메타 정보 가져오기
    private string[] GetImageMetaData(string _path)
    {
        using (AndroidJavaClass pluginClass = new AndroidJavaClass("com.example.plugin.UnityPlugin"))
        {
            using (AndroidJavaObject instance = pluginClass.CallStatic<AndroidJavaObject>("instance"))
            {
                try
                {
                    string[] result = instance.Call<string[]>("getAllMeta", _path);

                    Debug.Log("Successed To Get Meta Data");
                    return result;
                }
                catch
                {
                    return null;
                }

            }

        }
    }

}
