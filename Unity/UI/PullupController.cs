/*
작성자: 최재호(cjh0798@gmail.com)
기능: 피드 풀업메뉴 On Off
 */
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.UI.Extensions;
using UnityEngine.EventSystems;
public class PullupController : MonoBehaviour, IEndDragHandler
{
    public Image background;
    private Scrollbar scrollbar;
    protected VerticalScrollSnap scrollSnap;
    protected bool isChanging;

    [SerializeField]
    Button[] PullupArr;

    public virtual void Start()
    {
        scrollbar = GetComponentInChildren<Scrollbar>();
        scrollSnap = GetComponent<VerticalScrollSnap>();
    }

    // 풀업 메뉴 On
    public virtual async void OnPullupMenu()
    {
        if (isChanging == true)
            return;

        gameObject.SetActive(true);

        await UniTask.Yield();

        transform.root.GetComponent<Canvas>().enabled = true;
        isChanging = true;
        Color fadeInColor = new Color(0, 0, 0, 0.8f);
        background.gameObject.SetActive(true);
        if (scrollSnap != null)
        {
            scrollSnap.ChangePage(0);
        }
        background.DOColor(fadeInColor, 0.5f);

        isChanging = false;
        await UniTask.Delay(600);
    }

    // 풀업 메뉴 Off
    public virtual async void OffPullupMenu()
    {
        if (isChanging == true)
            return;

        isChanging = true;
        Color fadeOutColor = new Color(0, 0, 0, 0);
        background.DOColor(fadeOutColor, 0.5f);
        if (scrollSnap != null)
        {
            scrollSnap.ChangePage(1);
        }

        background.raycastTarget = false;
        await UniTask.Delay(600);
        background.gameObject.SetActive(false);
        background.raycastTarget = true;
        isChanging = false;
        if (PullupArr.Length == 0)
        {
            transform.root.GetComponent<Canvas>().enabled = false;
        }
    }

    // 드래그가 끝났을 때, 메뉴가 내려가면 배경화면 페이드아웃
    public async void OnEndDrag(PointerEventData eventData)
    {
        await UniTask.Delay(100);
        if (scrollbar.value > 0.4f)
        {
            OffPullupMenu();
        }
        else
        {
            background.gameObject.SetActive(true);
        }
    }
}

