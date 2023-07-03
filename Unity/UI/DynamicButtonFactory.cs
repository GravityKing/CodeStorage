using Cysharp.Threading.Tasks;
using Metalive;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class DynamicButtonFactory : MonoBehaviour
{
    [Header("[View]")]
    [SerializeField] private InformationBoothView view;

    [Header("[UI Settings]")]
    [SerializeField] private float buttonSpacing;

    private AsyncOperationHandle<InteractiveEventData> data;
    private void Start()
    {
        ShowPopup();
    }

    private void OnDestroy()
    {
        Addressables.Release(data);
    }

    private async void ShowPopup()
    {
        data = Addressables.LoadAssetAsync<InteractiveEventData>(Setting.Interactive.code);
        await data;
        List<InteractiveTable> interactiveTable = data.Result.eventTable;

        view.title.text = interactiveTable.Find(x => x.key == "title").value;
        view.subTitle.text = interactiveTable.Find(x => x.key == "subTitle").value;

        List<InteractiveTable> buttonTable = interactiveTable.Where(x => x.key.Contains("Button")).ToList();
        float topHeight = Mathf.Abs(view.subTitle.rectTransform.anchoredPosition.y) + view.subTitle.rectTransform.rect.height;
        float buttonHeight = view.buttonPrefab.rectTransform.rect.height;
        float padding = ((view.content.rect.height - topHeight) - (buttonHeight * buttonTable.Count) - (buttonSpacing * (buttonTable.Count - 1))) / 2;
        float startPos = -(topHeight + padding);
        float buttonYPos = startPos;
        for (int i = 0; i < buttonTable.Count; i++)
        {
            InformationBoothButton infoButton = Instantiate(view.buttonPrefab, view.content);
            infoButton.rectTransform.anchoredPosition = new Vector2(0, buttonYPos);
            infoButton.description.text = buttonTable[i].value;
            string link = buttonTable[i].link;
            infoButton.button.onClick.AddListener(() => Application.OpenURL(link));
            buttonYPos -= infoButton.rectTransform.rect.height + buttonSpacing;
            infoButton.gameObject.SetActive(true);
        }
    }
}
