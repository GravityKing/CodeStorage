/*
작성자: 최재호(cjh0798@gmail.com)
기능: 피드 작성 InputField 추가 기능 
1. Caret 위치 이동 기능
2. 드래그 기능
3. 키보드 취소 시 텍스트 저장 기능
 */
using Cysharp.Threading.Tasks;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FeedInputController : MonoBehaviour, IDragHandler, IBeginDragHandler, IPointerClickHandler, ISelectHandler
{
    [SerializeField] ScrollRect sr;

    private float preY;
    private float deltaY;
    private bool isDragging;
    private TMP_InputField input;
    private int caretIndex;
    private string preText;
    private string ppreText;
    private bool isCanceled;
    private void Start()
    {
        input = GetComponent<TMP_InputField>();
        input.onTouchScreenKeyboardStatusChanged.AddListener(CheckKeyboardStatus);
    }

    private void CheckKeyboardStatus(TouchScreenKeyboard.Status _status)
    {
        if (_status == TouchScreenKeyboard.Status.Canceled)
        {
            isCanceled = true;
        }
    }


    public void OnSelect(BaseEventData eventData)
    {
        ppreText = input.text;
        input.caretPosition = caretIndex;
    }

    public async void OnPointerClick(PointerEventData eventData)
    {
        // 비속어가 빨간 글씨일 경우 -> 비속어를 검정 글씨로 초기화
        transform.root?.GetComponent<FeedPost>()?.ResetSlangText();

        if (input.isFocused)
        {
            input.caretWidth = 0;
            input.DeactivateInputField();
            await UniTask.Delay(200);
            input.caretWidth = 3;
            SetCaretPos(eventData);

            input.ActivateInputField();
        }
        else if (input.enabled == false && isDragging == false)
        {
            input.enabled = true;
            // Caret 위치 이동
            SetCaretPos(eventData);
            input.ActivateInputField();
        }
    }

    public async void OnBeginDrag(PointerEventData eventData)
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        cts.CancelAfter(10000);
        await UniTask.WaitUntil(() => !Input.GetMouseButton(0), PlayerLoopTiming.Update, cts.Token);
        await UniTask.Yield();
        preY = 0;
        isDragging = false;
    }

    // 드래그 기능
    public void OnDrag(PointerEventData eventData)
    {

        float curY = Input.mousePosition.y;
        float contentY = sr.content.anchoredPosition.y;
        isDragging = true;

        if (preY == 0)
        {
            preY = Input.mousePosition.y;
        }

        // 위
        if (preY < curY)
        {
            deltaY = Mathf.Abs(curY) - Mathf.Abs(preY);
            sr.content.anchoredPosition = new Vector2(0, contentY + deltaY);
            input.enabled = false;
        }
        // 아래
        else if (preY > curY)
        {
            deltaY = Mathf.Abs(preY) - Mathf.Abs(curY);
            sr.content.anchoredPosition = new Vector2(0, contentY - deltaY);
            input.enabled = false;
        }

        preY = Input.mousePosition.y;
    }

    public void OnValueChanged()
    {
        ppreText = preText;
        preText = input.text;
    }

    public void OnEndEdit()
    {
        if (isCanceled == true)
        {
            input.text = ppreText;
            isCanceled = false;
        }
        else
        {
            input.text = preText;
        }
        ppreText = preText;

    }

    // Caret 위치 이동
    private void SetCaretPos(PointerEventData eventData)
    {
        var text = input.textComponent;

        caretIndex = TMP_TextUtilities.FindNearestCharacter(text, eventData.pressPosition, Camera.main, false);
        input.caretPosition = caretIndex;
    }

}
