/*
작성자: 최재호(cjh0798@gmail.com)
기능: 안드로이드 설정 화면으로 이동
 */
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class AndroidApplicationSetting : MonoBehaviour
{
    [SerializeField]
    private bool isOpened;

    // 안드로이드 설정 화면으로 이동
    public void AndroidApplicationSetting()
    {

        using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (AndroidJavaObject currentActivityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity"))
        {
            string packageName = currentActivityObject.Call<string>("getPackageName");

            using (var uriClass = new AndroidJavaClass("android.net.Uri"))
            using (AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromParts", "package", packageName, null))
            using (var intentObject = new AndroidJavaObject("android.content.Intent", "android.settings.APPLICATION_DETAILS_SETTINGS", uriObject))
            {
                intentObject.Call<AndroidJavaObject>("addCategory", "android.intent.category.DEFAULT");
                intentObject.Call<AndroidJavaObject>("setFlags", 0x10000000);
                currentActivityObject.Call("startActivity", intentObject);
            }
        }
        isOpened = true;
    }


    // 안드로이드 설정 화면에서 유니티로 돌아왔을 때
    private async void OnApplicationFocus(bool hasFocus)
    {
        if (isOpened == false)
            return;

        if (isOpened == false)
        {
            return;
        }
        isOpened = false;
    }
}
