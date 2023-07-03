/*
작성자: 최재호(cjh0798@gmail.com)
기능: 피드 새로고침
 */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;
using System.Threading;

public class FeedRefresh : MonoBehaviour, IBeginDragHandler
{
    private ScrollRect scrollRect;
    private float beginY;
    [SerializeField] private float targetDeltaValue;
    [SerializeField] private MainTextureDisposer texDisposer;

    private void Start()
    {
        scrollRect = GetComponentInChildren<ScrollRect>();

    }

    public void RefreshFeed()
    {
        GetComponentInParent<FeedFactory>().InitFeed();
    }

    
    public async void OnBeginDrag(PointerEventData eventData)
    {
        beginY = Mathf.Abs(Input.mousePosition.y);
        CancellationTokenSource cts = new CancellationTokenSource();
        cts.CancelAfter(10000);
        await UniTask.WaitUntil(() => !Input.GetMouseButton(0),PlayerLoopTiming.Update,cts.Token);

        float curY = Mathf.Abs(Input.mousePosition.y);
        float deltaY = beginY > curY ? beginY - curY : 0;
        if (deltaY > targetDeltaValue && scrollRect.verticalNormalizedPosition >= 1)
        {
            RefreshFeed();
            texDisposer.DisposePageTexture(MainPage.FeedMain);

            await UniTask.Yield();
            scrollRect.OnBeginDrag(eventData);
            scrollRect.OnDrag(eventData);
            scrollRect.OnEndDrag(eventData);
            cts.Dispose();
        }
    }
}
