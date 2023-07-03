using Cysharp.Threading.Tasks;
using Metalive;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class HistoryPresenter : MonoBehaviour
{
    [Header("[View]")]
    [SerializeField] private HistoryMainPopupView mainPopupView;
    [SerializeField] private HistoryDetailPopupView detailPopupView;

    [Header("[MainPopupView Setting]")]
    [SerializeField] private float mainPopupWidth;
    [SerializeField] private float mainPopupHeight;
    [SerializeField] private float mainPopupTopheight;
    [SerializeField] private float mainPopupBottomheight;
    [SerializeField] private float mainPopupButtonPosY;
    [SerializeField] private float mainPopupButtonPaddingX;
    [SerializeField] private float mainPopupButtonSpacing;
    [SerializeField] private float mainPopupButtonWidth;
    [SerializeField] private float mainPopupButtonHeight;

    [Header("[DetailPopupView Setting]")]
    [SerializeField] private float detailPopupWidth;
    [SerializeField] private float detailPopupHeight;
    [SerializeField] private float detailPopupLeftWidth;
    [SerializeField] private float detailPopupRightWidth;
    [SerializeField] private float detailPopupTextPaddingY;
    [SerializeField] private float detailPopupTextSpacingY;
    [SerializeField] private float detailPopupTextPosX;
    [SerializeField] private float detailPopupTextDefaultWidth;
    [SerializeField] private float detailPopupTitlePosX;

    private readonly float targetCanvasWidth = 2960;
    private readonly float targetCanvasHeight = 1440;

    private AsyncOperationHandle<InteractiveEventData> data;
    private List<HistoryButton> historyButtons;

    private void Start()
    {
        Init();
    }

    private void OnDestroy()
    {
        Addressables.Release(data);
    }

    private async void Init()
    {
        historyButtons = new List<HistoryButton>();
        InitUI();

        // 어드레서블에서 model 가져오기
        data = Addressables.LoadAssetAsync<InteractiveEventData>(Setting.Interactive.code);
        await data;


        if (data.Status == AsyncOperationStatus.Succeeded)
        {
            ShowMainPopup();
        }
    }

    private void InitUI()
    {
        // MainPopupView
        mainPopupView.mainPopup.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mainPopupWidth);
        mainPopupView.mainPopup.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mainPopupHeight);
        mainPopupView.topRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mainPopupTopheight);
        mainPopupView.bottomRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mainPopupBottomheight);
        mainPopupView.historyButtonPrefab.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mainPopupButtonWidth);
        mainPopupView.historyButtonPrefab.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mainPopupButtonHeight);

        // DetailPopupView
        float canvasWidthDiff = targetCanvasWidth - detailPopupView.canvasRectTransform.rect.width;
        float canvasHeightDiff = targetCanvasHeight - detailPopupView.canvasRectTransform.rect.height;
        detailPopupView.detailPopup.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, detailPopupWidth - canvasWidthDiff);
        detailPopupView.detailPopup.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, detailPopupHeight - canvasHeightDiff);
        detailPopupView.leftRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, detailPopupLeftWidth - canvasWidthDiff);
        detailPopupView.rightRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, detailPopupRightWidth - canvasWidthDiff);
    }
    private void ShowMainPopup()
    {
        List<InteractiveTable> mainTable = data.Result.eventTable;
        List<InteractiveTable> buttonTable = mainTable.Where(x => x.key.Contains("history")).ToList();
        List<InteractiveSpriteTable> spriteTable = data.Result.eventSpriteTable;
        mainPopupView.title.text = mainTable.Find(x => x.key == "title").value;
        for (int i = 0; i < buttonTable.Count; i++)
        {
            HistoryButton historyButton = Instantiate(mainPopupView.historyButtonPrefab, mainPopupView.bottomScrollRect.content);
            historyButton.gameObject.SetActive(true);
            historyButton.title.text = buttonTable[i].value;
            historyButton.image.sprite = spriteTable[i].value;
            int index = i;
            historyButton.button.onClick.AddListener(() => ShowDetailPopup(index));

            float buttonPosX = mainPopupButtonPaddingX + (i * (mainPopupButtonSpacing + historyButton.rectTransform.rect.width));
            historyButton.rectTransform.anchoredPosition = new Vector2(buttonPosX, mainPopupButtonPosY);

            historyButtons.Add(historyButton);
        }
        int lastIndex = spriteTable.Count - 1;
        float videoUIWith = historyButtons[lastIndex].rectTransform.rect.width;
        float videoPos = historyButtons[lastIndex].rectTransform.anchoredPosition.x;
        float bottomContentWidth = videoPos + videoUIWith + mainPopupButtonPaddingX;
        mainPopupView.bottomScrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, bottomContentWidth);
    }

    private async void ShowDetailPopup(int index)
    {
        mainPopupView.canvas.enabled = false;
        detailPopupView.canvas.enabled = true;

        List<InteractiveTable> subTable = data.Result.eventSubTable;
        int descriptionCount = int.Parse(subTable.Find(x => x.key == "descriptionCount").value);
        List<InteractiveTable> descriptionTable = subTable.Where(x => x.key.Contains($"Description_{index}")).Take(descriptionCount).ToList();
        List<InteractiveTable> imageTable = subTable.Where(x => x.key.Contains("image")).ToList();
        List<TMP_Text> titles = detailPopupView.titles;
        List<TMP_Text> descriptions = detailPopupView.descriptions;

        float posY = -detailPopupTextPaddingY;
        float titleWidthDiff = 0;
        detailPopupView.rightImage.sprite = null;
        for (int i = 0; i < descriptionTable.Count; i++)
        {
            if (string.IsNullOrEmpty(descriptionTable[i].value) || string.IsNullOrWhiteSpace(descriptionTable[i].value))
            {
                descriptions[i].text = "";
                titles[i].gameObject.SetActive(false);
                descriptions[i].gameObject.SetActive(false);
                continue;
            }

            titles[i].gameObject.SetActive(true);
            descriptions[i].gameObject.SetActive(true);
            descriptions[i].text = descriptionTable[i].value;
            LayoutRebuilder.ForceRebuildLayoutImmediate(descriptions[i].rectTransform);

            titleWidthDiff = GetTitleWidthDiff(titles[i].rectTransform.rect.width);
            descriptions[i].rectTransform.anchoredPosition = new Vector2(detailPopupTextPosX + titleWidthDiff, posY);
            titles[i].rectTransform.anchoredPosition = new Vector2(detailPopupTitlePosX, posY);
            posY -= descriptions[i].rectTransform.rect.height + detailPopupTextSpacingY;
        }
        detailPopupView.rightScrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Abs(posY) - detailPopupTextSpacingY);
        detailPopupView.rightImage.sprite = await GetSprite(imageTable[index].value);
    }

    // detailPopupTextDefaultWidth보다 width가 크면 해당 diff를 반환
    private float GetTitleWidthDiff(float titleWidth)
    {
        float diff = detailPopupTextDefaultWidth - titleWidth;
        bool isDefaultBigger = diff > 0;
        float titleWidthDiff = isDefaultBigger ? 0 : Mathf.Abs(diff);

        return titleWidthDiff;
    }

    private async UniTask<Sprite> GetSprite(string url)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            await request.SendWebRequest();
            Texture2D tex = DownloadHandlerTexture.GetContent(request);
            if (tex != null)
            {

                Rect rect = new Rect(0, 0, tex.width, tex.height);
                Vector2 pivot = new Vector2(0.5f, 0.5f);
                Sprite sprite = Sprite.Create(tex, rect, pivot);
                return sprite;
            }

            else
            {
                return null;
            }
        }
    }
}
