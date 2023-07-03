/*
작성자: 최재호(cjh0798@gmail.com)
기능: AVPro를 활용한 비디오 재생
 */
using Cysharp.Threading.Tasks;
using DG.Tweening;
using RenderHeads.Media.AVProVideo;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

public enum RentalcarVideoKind
{
    Aewol,
    Sanbang,
    Sungsan
}

public class AvProVideoController : MonoBehaviour
{
    public RentalCarPullupController pullupController;
    public Image initImageSet;
    public RectTransform safeArea;
    public string VideoPath { get; set; }
    public RentalcarVideoKind VideoKind { get; set; }
    public MediaPlayer videoPlayer;
    public MainSceneLoading loading;

    private bool isOff;
    private bool isFinished;

    public async void CheckNetwork()
    {
        Canvas videoPage = transform.root.GetComponent<Canvas>();
        if (videoPage.enabled == true)
        {
            await UniTask.WaitUntil(() => videoPlayer.Control.IsPlaying());
        }

        while (videoPlayer.Control.IsFinished() == false)
        {
            if (videoPlayer.enabled == false || videoPlayer.Control.IsFinished())
            {
                loading.SetActiveMidLoading(false);
                return;
            }

            if (videoPlayer.Info.IsPlaybackStalled() && !isFinished)
            {
                Debug.Log("videoPlayer.Info.IsPlaybackStalled()");
                loading.SetActiveMidLoading(true);

            }
            else
            {
                loading.SetActiveMidLoading(false);
            }
            await UniTask.Yield();
        }
    }

    private async void ShowEndPopup()
    {
        if (!Application.isPlaying)
            return;

        var canvasNav = GameObject.FindGameObjectWithTag("GameController").GetComponent<MainCanvasNavi>();
        CancellationTokenSource cts = new CancellationTokenSource();
        cts.CancelAfter(60000); // 60초
        await UniTask.WaitUntil(() => videoPlayer.Control.IsFinished() == true, PlayerLoopTiming.Update, cts.Token);

        if (videoPlayer.Control.IsFinished() == true && canvasNav.subUIStack.Peek().name == "RentalCarVideoPage")
        {
            canvasNav.GetComponent<MainPopupManager>().ShowPopup("Panel_RentalCarVideo", OffVideo);
        }
    }

    // 체험하기 비디오 On
    public async void PlayVideo()
    {
        isOff = false;

        Screen.fullScreen = true;
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        pullupController.OffPullupMenu();
        pullupController.gameObject.SetActive(false);

        var canvasNav = GameObject.FindGameObjectWithTag("GameController").GetComponent<MainCanvasNavi>();
        canvasNav.Push("RentalCarVideoPage");

        initImageSet.DOPause();
        initImageSet.color = new Color(0, 0, 0, 1);
        initImageSet.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
        initImageSet.gameObject.SetActive(true);
        Debug.Log(VideoPath);

        videoPlayer.enabled = true;
        videoPlayer.Events.AddListener(OnFinishedPlaying);
        videoPlayer.OpenMedia(MediaPathType.AbsolutePathOrURL, VideoPath, false);
        videoPlayer.Play();

        CheckNetwork();

        CancellationTokenSource videoCts = new CancellationTokenSource();
        float time = 0;
        await UniTask.WaitUntil(() =>
        {
            time += Time.deltaTime;
            if (canvasNav.subUIStack.Peek().name != "RentalCarVideoPage")
            {
                videoCts.Cancel();
                return false;
            }
            else if (time >= 2.0f && videoPlayer.Control.IsPlaying())
            {
                return true;
            }
            else
            {
                return false;
            }

        }, PlayerLoopTiming.Update, videoCts.Token);

        if (pullupController.gameObject.activeSelf)
        {
            pullupController.gameObject.SetActive(false);
        }

        FadeOutInitImage();



        CancellationTokenSource showPopupCts = new CancellationTokenSource();
        showPopupCts.CancelAfter(10000); // 10초
        try
        {
            await UniTask.WaitUntil(() => videoPlayer.Control.IsPlaying(), PlayerLoopTiming.Update, showPopupCts.Token);
            ShowEndPopup();
        }
        catch when (showPopupCts.IsCancellationRequested)
        {
            ShowEndPopup();
        }

        AudioSource audio = GetComponent<AudioSource>();
        audio.PlayOneShot(audio.clip);
    }

    // 체험하기 비디오 Off
    public void OffVideo()
    {
        if (isOff == true)
            return;

        isOff = true;
        Screen.fullScreen = false;
        Screen.orientation = ScreenOrientation.Portrait;

        videoPlayer.Stop();
        videoPlayer.Events.RemoveListener(OnFinishedPlaying);
        videoPlayer.enabled = false;
        isFinished = false;

        AudioSource audio = GetComponent<AudioSource>();
        audio.Stop();

        initImageSet.transform.GetChild(0).GetComponent<Image>().DOPause();

        var canvasNav = GameObject.FindGameObjectWithTag("GameController").GetComponent<MainCanvasNavi>();
        canvasNav.pop();
        canvasNav.pop();
    }

    // 360도 안내 이미지 페이드 아웃
    private void FadeOutInitImage()
    {
        initImageSet.DOFade(0, 1).onComplete = () =>
        {
            initImageSet.color = new Color(0, 0, 0, 1);
        };
        initImageSet.transform.GetChild(0).GetComponent<Image>().DOFade(0, 1).onComplete = () =>
        {
            initImageSet.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
            initImageSet.gameObject.SetActive(false);
        };
    }

    private void OnFinishedPlaying(MediaPlayer mp, MediaPlayerEvent.EventType type, ErrorCode code)
    {
        Debug.Log($"Event: {type}");
        if (type == MediaPlayerEvent.EventType.FinishedPlaying || type == MediaPlayerEvent.EventType.PlaylistFinished)
        {
            Debug.Log("OnFinishedPlaying");
            isFinished = true;
            loading.SetActiveMidLoading(false);
        }
    }

    public void DoReset()
    {
        pullupController.gameObject.SetActive(true);
        pullupController.OnPullupMenu();
    }

}
