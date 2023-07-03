/*
작성자: 최재호(cjh0798@gmail.com)
기능: 링크 메인 씬 접속 시 페이지 컨트롤
 */
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

public class LinkPageController : MonoBehaviour
{
    MainCanvasNavi canvasNav;
    public RoutePlaceMorePage routePlaceMorePage;
    public RouteCourseMorePage routeCourseMorePage;
    public Button placeBackBtn;
    public Button courseBackBtn;

    private void Awake()
    {
        canvasNav = GetComponent<MainCanvasNavi>();
        if (canvasNav == null)
            FindObjectOfType<MainCanvasNavi>();
    }

    private void Start()
    {
        Application.deepLinkActivated += (x) => ShowPageFromLink();
    }

    public void ShowFeedPage(int _feedNo)
    {
        canvasNav.Push("FeedDetail_Page");

        var page = canvasNav.subDic["FeedDetail_Page"].GetComponentInChildren<FeedDetailPage>();
        Feed feed = new Feed() { contentNo = _feedNo };
        page.UpdatePage(feed);
    }

    public async void ShowPlacePage(int _placeNo)
    {
        // 최적화 ShowPlacePage() -> 최적화 후 주석처리
        //while (!routePlaceMorePage.placeLoadCheck || !MyPickManager.instance.placeLoadCheck)
        //{
        //    await UniTask.WaitForFixedUpdate();
        //}

        RoutePlaceDetailPage page = canvasNav.subDic["PlaceDetailPage"].GetComponent<RoutePlaceDetailPage>();
        page.tripPlaceNo = _placeNo;
        canvasNav.Push("PlaceDetailPage");

        //page.PlaceDetailPageSetting(_placeNo);

        var toggles = FindObjectsOfType<MainBasicToggle>();
        var routeToggle = Array.Find(toggles, x => x.name.Contains("Route"));
        routeToggle.GetComponent<Toggle>().isOn = true;

        placeBackBtn.onClick.AddListener(() =>
        {
            var homeToggle = Array.Find(toggles, x => x.name.Contains("Home"));
            homeToggle.GetComponent<Toggle>().isOn = true;
        });
    }

    public async void ShowCoursePage(int _courseNo)
    {
        // 최적화 ShowCoursePage() -> 최적화 후 주석처리
        //while (!routeCourseMorePage.courseLoadCheck || !MyPickManager.instance.courseLoadCheck)
        //{
        //    await UniTask.WaitForFixedUpdate();
        //}


        RouteCourseDetailPage page = canvasNav.subDic["CourseDetailPage"].GetComponent<RouteCourseDetailPage>();
        RouteCourseDetailPageLowNavi routeCourseDetailPageLowNavi = canvasNav.subDic["CourseDetailPage"] as RouteCourseDetailPageLowNavi;
        page.tripCourseNo = _courseNo;
        //page.CourseDetailPageSetting(_courseNo);
        canvasNav.Push("CourseDetailPage");

        var toggles = FindObjectsOfType<MainBasicToggle>();
        var routeToggle = Array.Find(toggles, x => x.name.Contains("Route"));
        routeToggle.GetComponent<Toggle>().isOn = true;

        courseBackBtn.onClick.AddListener(() =>
        {
            var homeToggle = Array.Find(toggles, x => x.name.Contains("Home"));
            homeToggle.GetComponent<Toggle>().isOn = true;
        });
    }

    // 링크를 통한 페이지 이동
    private void ShowPageFromLink()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<MainCanvasNavi>().Push("HomeMain");

        (string kind, string no) data = GetLinkData();
        Debug.Log("data.kind: " + data.kind);
        Debug.Log("data.no: " + data.no);
        LinkPageController controller = GetComponent<LinkPageController>();
        if (controller == null)
            controller = FindObjectOfType<LinkPageController>();

        switch (data.kind)
        {
            // 피드 페이지로 이동
            case "feedNo":
                controller.ShowFeedPage(int.Parse(data.no));
                break;

            // 여행지 페이지로 이동
            case "placeNo":
                controller.ShowPlacePage(int.Parse(data.no));
                break;

            // 코스 페이지로 이동
            case "courseNo":
                controller.ShowCoursePage(int.Parse(data.no));
                break;
        }
    }

    // 링크 데이터 가져오기
    private (string, string) GetLinkData()
    {
        string url = Application.absoluteURL;
        Debug.Log("@@@@@@@@DeepLinkUrl: " + url);
        string[] data = url.Split("?"[0])[1].Split('=');
        string kind = data[0];
        string no = data[1].Split('&')[0];

        return (kind, no);
    }
}
