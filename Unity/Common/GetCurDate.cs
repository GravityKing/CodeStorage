using System;
using UnityEngine;

public class GetCurDate : MonoBehaviour
{
    // 서버에 날짜 및 시간을 보낼 때 사용
    private string GetCurDate()
    {
        string format = "yyyy-MM-dd HH:mm:ss";
        string date = DateTime.Now.ToString(format);

        return date;
    }

}
