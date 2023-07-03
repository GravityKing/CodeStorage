/*
작성자: 최재호(cjh0798@gmail.com) 
기능: 메인 씬 로딩 생성
*/
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
public class MainSceneLoading : MonoBehaviour
{
    [SerializeField] RectTransform bg;
    [SerializeField] RectTransform icon;


    // 위쪽 로딩
    public void SetActiveTopLoading(bool _value)
    {
        icon.anchorMin = new Vector2(0.5f, 1);
        icon.anchorMax = new Vector2(0.5f, 1);
        icon.anchoredPosition = new Vector2(0, -400);
        gameObject.SetActive(_value);

    }

    // 가운데쪽 로딩
    public void SetActiveMidLoading(bool _value)
    {
        icon.anchorMin = new Vector2(0.5f, 0.5f);
        icon.anchorMax = new Vector2(0.5f, 0.5f);
        icon.anchoredPosition = new Vector2(0, 0);
        gameObject.SetActive(_value);

    }

    public void SetActiveMidLoadingWithBG(bool _value)
    {
        icon.anchorMin = new Vector2(0.5f, 0.5f);
        icon.anchorMax = new Vector2(0.5f, 0.5f);
        icon.anchoredPosition = new Vector2(0, 0);
        gameObject.SetActive(_value);
        bg.gameObject.SetActive(_value);

    }

    // 아래쪽 로딩 에러 콜백 포함
    public void SetActiveBotLoading(bool _value)
    {
        icon.anchorMin = new Vector2(0.5f, 0);
        icon.anchorMax = new Vector2(0.5f, 0);
        icon.anchoredPosition = new Vector2(0, 480);
        gameObject.SetActive(_value);

        DoErrorExceptionAction();
    }

    // 위쪽 로딩 에러 콜백 포함
    public void SetActiveTopLoading(bool _value, Action errorException)
    {
        icon.anchorMin = new Vector2(0.5f, 1);
        icon.anchorMax = new Vector2(0.5f, 1);
        icon.anchoredPosition = new Vector2(0, -400);
        gameObject.SetActive(_value);

        DoErrorExceptionAction(errorException);
    }

    // 가운데쪽 로딩 에러 콜백 포함
    public void SetActiveMidLoading(bool _value, Action errorException)
    {
        icon.anchorMin = new Vector2(0.5f, 0.5f);
        icon.anchorMax = new Vector2(0.5f, 0.5f);
        icon.anchoredPosition = new Vector2(0, 0);
        gameObject.SetActive(_value);

        DoErrorExceptionAction(errorException);
    }

    public void SetActiveMidLoadingWithBG(bool _value, Action errorException)
    {
        icon.anchorMin = new Vector2(0.5f, 0.5f);
        icon.anchorMax = new Vector2(0.5f, 0.5f);
        icon.anchoredPosition = new Vector2(0, 0);
        gameObject.SetActive(_value);
        bg.gameObject.SetActive(_value);

        DoErrorExceptionAction(errorException);
    }

    // 아래쪽 로딩 에러 콜백 포함
    public void SetActiveBotLoading(bool _value, Action errorException)
    {
        icon.anchorMin = new Vector2(0.5f, 0);
        icon.anchorMax = new Vector2(0.5f, 0);
        icon.anchoredPosition = new Vector2(0, 480);
        gameObject.SetActive(_value);

        DoErrorExceptionAction(errorException);
    }

    private async void DoErrorExceptionAction(Action _callback)
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        cts.CancelAfter(10000); // 10초

        try
        {
            await UniTask.WaitUntil(() => gameObject.activeSelf == false, PlayerLoopTiming.Update, cts.Token);
        }
        catch when (cts.Token.IsCancellationRequested)
        {
            bg.gameObject.SetActive(false);
            gameObject.SetActive(false);

            Dictionary<string, string> message = new Dictionary<string, string>()
            {
                ["head"] = "알림",
                ["body"] = "서비스 접속이 원활하지 않습니다.\n잠시 후 다시 시도해 주세요.",
                ["yes"] = "재시도",
                ["no"] = "취소",
            };
            ChoicePopupMessage.Send(message, _callback);
        }
    }

    private async void DoErrorExceptionAction()
    {
        MainCanvasNavi canvasNav = GameObject.FindGameObjectWithTag("GameController").GetComponent<MainCanvasNavi>();
        if (canvasNav.subDic == null || canvasNav.subDic.Count == 0)
            await UniTask.WaitUntil(() => canvasNav.subDic.Count != 0);

        CancellationTokenSource cts = new CancellationTokenSource();
        if (IsImagePost() == true)
        {
            cts.CancelAfter(30000); // 30초
        }
        else
        {
            cts.CancelAfter(15000); // 15초
        }

        try
        {
            await UniTask.WaitUntil(() => gameObject.activeSelf == false, PlayerLoopTiming.Update, cts.Token);
        }
        catch when (cts.Token.IsCancellationRequested)
        {
            bg.gameObject.SetActive(false);
            gameObject.SetActive(false);

            Dictionary<string, string> message = new Dictionary<string, string>()
            {
                ["head"] = "알림",
                ["body"] = "서비스 접속이 원활하지 않습니다.\n잠시 후 다시 시도해 주세요.",
                ["yes"] = "확인",
                ["no"] = "취소",
            };
            ChoicePopupMessage.Send(message, null);
        }
    }

    // 이미지를 POST하는 API인지 확인
    private bool IsImagePost()
    {
        MainCanvasNavi canvasNav = GameObject.FindGameObjectWithTag("GameController").GetComponent<MainCanvasNavi>();

        bool isOnFeedWrite = canvasNav.subDic["FeedWrite_Page"].GetComponent<Canvas>().enabled;
        bool isOnPlaceRegist = canvasNav.subDic["PlaceRegistPage"].GetComponent<Canvas>().enabled;
        if (isOnFeedWrite || isOnPlaceRegist)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
