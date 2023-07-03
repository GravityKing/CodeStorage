/*
작성자: 최재호(cjh0798@gmail.com)
기능: 피드 길찾기 기능
 */
using Cysharp.Threading.Tasks;
using System;
using System.Text;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;

public class FeedPathFinder : MonoBehaviour
{

    [SerializeField] private PullupController pullupController;
    [HideInInspector] public string Latitude { get; private set; }
    [HideInInspector] public string Longitude { get; private set; }
    private string pathfinderAppKind;

    // 이미지에서 길찾기 버튼 클릭
    public void OnClickPathFinder(GeoData _data)
    {
        Debug.Log("_data: " + _data);
        pathfinderAppKind = null;
        Latitude = _data.latitude;
        Longitude = _data.longitude;

        string savedAppKind = PlayerPrefs.GetString(nameof(pathfinderAppKind));
        if (string.IsNullOrEmpty(savedAppKind))
        {
            pullupController.OnPullupMenu();
        }
        else
        {
            OpenMapApp();
        }
    }

    // 해당 앱으로 한 번만 Open
    public void OnClickOpenMapApp(bool isAlways)
    {
        // 항상 해당 앱으로 Open할 경우 데이터 저장
        if (isAlways)
        {
            PlayerPrefs.SetString(nameof(pathfinderAppKind), pathfinderAppKind);
        }

        if (string.IsNullOrEmpty(pathfinderAppKind))
        {
            if (Application.platform != RuntimePlatform.Android)
                return;

#if UNITY_ANDROID
            // 하단 토스트 띄우기
            using (AndroidJavaClass pluginClass = new AndroidJavaClass("com.example.plugin.UnityPlugin"))
            {
                using (AndroidJavaObject instance = pluginClass.CallStatic<AndroidJavaObject>("instance"))
                {
                    instance.Call("showToast", "앱을 선택해주세요");
                    pluginClass.Dispose();
                }
            }
#endif
        }
        else
        {
            OpenMapApp();
        }
    }

    // 맵 어플 Open
    private async void OpenMapApp()
    {
        string savedAppKind = PlayerPrefs.GetString(nameof(pathfinderAppKind));
        string kind = string.IsNullOrEmpty(savedAppKind) ? pathfinderAppKind : savedAppKind;
        if (Latitude.Contains("/") && Longitude.Contains("/"))
        {
            ConvertLatLong(true);
            ConvertLatLong(false);
        }

        string dest = await GetDest(Latitude, Longitude);

#if UNITY_ANDROID
        switch (kind)
        {
            case "Google":
                Application.OpenURL($"https://www.google.com/maps/dir/?api=1&destination={Latitude}%2C{Longitude}");

                break;

            case "KaKao":
                Application.OpenURL($"https://map.kakao.com/link/to/{dest},{Latitude},{Longitude}");
                break;

            case "TMap":
                Application.OpenURL($"https://apis.openapi.sk.com/tmap/app/routes?appKey=l7xxc7a798bcef3d469c8b7963e0582c31dd&name={dest}&lon={Longitude}&lat={Latitude}");
                break;
        }
#elif UNITY_IOS
        dest = UnityWebRequest.EscapeURL(dest);
        switch (kind)
        {
            case "Google":
                Application.OpenURL($"https://www.google.com/maps/search/?api=1&query={dest}");
                break;

            case "KaKao":
                Application.OpenURL($"https://map.kakao.com/link/to/{dest},{Latitude},{Longitude}");
                break;

            case "TMap":
                Application.OpenURL($"https://apis.openapi.sk.com/tmap/app/routes?appKey=l7xxc7a798bcef3d469c8b7963e0582c31dd&name={dest}&lon={Longitude}&lat={Latitude}");
                break;

            case "AppleMap":
                Application.OpenURL($"http://maps.apple.com/maps?q={dest}&ll={Latitude},{Longitude}");
                break;
        }
#endif

        pullupController.OffPullupMenu();
    }

    // 도분초 > 위경도 변환
    public void ConvertLatLong(bool isLat)
    {
        string[] data = isLat == true ? Latitude.Split(' ') : Longitude.Split(' ');
        double d = 0;
        double m = 0;
        double s = 0;
        for (int i = 0; i < data.Length; i++)
        {
            string[] dmsArr = data[i].Split('/');

            double result = double.Parse(dmsArr[0]) / double.Parse(dmsArr[1]);
            if (i == 0)
            {
                d = result;
            }
            else if (i == 1)
            {
                m = result;
            }
            else
            {
                s = result;
            }
        }


        if (isLat)
        {
            float result = (float)(d + (m / 60) + (s / 3600));
            Latitude = result.ToString();
        }
        else
        {
            float result = (float)(d + (m / 60) + (s / 3600));
            Longitude = result.ToString();
        }

    }

    // 목적지 가져오기
    private async UniTask<string> GetDest(string _latitude, string _longitude)
    {
        string path = $"https://nominatim.openstreetmap.org/reverse?format=xml&lat={_latitude}&lon={_longitude}&zoom=20&addressdetails=1&accept-language=ko";
        using (UnityWebRequest request = UnityWebRequest.Get(path))
        {
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                return null;
            }
            else
            {
                string data = request.downloadHandler.text;
                Debug.Log(data);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(data);

                XmlNodeList nodes = xmlDoc.SelectNodes("reversegeocode/addressparts");
                StringBuilder dest = new StringBuilder();

                foreach (XmlNode node in nodes)
                {
                    string[] address = new string[]
                    {
                        node.SelectSingleNode("province")?.InnerText,
                        node.SelectSingleNode("city")?.InnerText,
                        node.SelectSingleNode("town")?.InnerText,
                        node.SelectSingleNode("suburb")?.InnerText,
                        node.SelectSingleNode("borough")?.InnerText,
                        node.SelectSingleNode("county")?.InnerText,
                        node.SelectSingleNode("village")?.InnerText,
                        node.SelectSingleNode("neighbourghood")?.InnerText,
                        node.SelectSingleNode("road")?.InnerText,
                        node.SelectSingleNode("shop")?.InnerText,
                        node.SelectSingleNode("amenity")?.InnerText,
                        node.SelectSingleNode("office")?.InnerText,
                    };

                    for (int i = 0; i < address.Length; i++)
                    {
                        if (string.IsNullOrEmpty(address[i]) == false)
                            dest.Append(address[i] + " ");
                    }
                }

                return dest.ToString();
            }
        }

    }

    // 실행할 앱 종류 Set
    public void SetAppKind(string _kind)
    {
        // 앱 아이콘 두 번 눌렀을 때
        if (pathfinderAppKind == _kind)
        {
            OpenMapApp();
        }
        // 앱 아이콘 한 번 눌렀을 때
        else
        {
            pathfinderAppKind = _kind;
        }
    }
}
