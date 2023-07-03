using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Events;

public class CustomScrollRect : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public RectTransform content;
    public UnityEvent OnValueMax;
    private float preY;
    private float deltaY;
    private float endValue = 0;
    private float duration = 0;
    private bool isUp;

    private void Update()
    {
        preY = Input.mousePosition.y;
    }


    public void OnDrag(PointerEventData eventData)
    {
        float curY = Input.mousePosition.y;
        float contentY = content.anchoredPosition.y;

        // 위
        if (preY < curY)
        {
            isUp = true;
            if (IsSpace(contentY + deltaY) == false)
                return;

            deltaY = Mathf.Abs(curY) - Mathf.Abs(preY);
            content.anchoredPosition = new Vector2(0, contentY + deltaY);
        }
        // 아래
        else if (preY > curY)
        {
            isUp = false;
            if (IsSpace(contentY - deltaY) == false)
                return;

            deltaY = Mathf.Abs(preY) - Mathf.Abs(curY);
            content.anchoredPosition = new Vector2(0, contentY - deltaY);
        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 위
        if (isUp)
        {
            endValue = deltaY;
            duration = Mathf.Abs(endValue) / 50;

            if (duration > 1)
            {
                Slide();
            }
            else if (duration > 0.15f)
            {
                duration = 0.5f;
                Slide();
            }
            else
            {
                return;
            }

        }
        // 아래
        else
        {
            endValue = -deltaY;
            duration = Mathf.Abs(endValue) / 50;
            if (duration > 1)
            {
                Slide();
            }
            else if (duration > 0.15f)
            {
                duration = 0.5f;
                Slide();
            }
            else
            {
                return;
            }
        }
    }

    private void Slide()
    {
        Vector2 curPos = content.anchoredPosition;
        float speed = endValue / 3;
        float targetY = curPos.y + (endValue * Mathf.Abs(speed));

        var tween = content.DOAnchorPosY(targetY, duration);
        tween.onUpdate += () =>
        {
            if (Input.GetMouseButton(0) || IsSpace(content.anchoredPosition.y) == false)
                tween.Pause();
        };
    }

    private bool IsSpace(float _targetY)
    {

        if (isUp)
        {
            float maxY = content.rect.height - content.parent.GetComponent<RectTransform>().rect.height;
            if (_targetY >= maxY)
            {

                content.anchoredPosition = new Vector2(0, maxY);
                OnValueMax.Invoke();
                return false;
            }
            else
            {
                return true;
            }

        }
        else
        {
            if (_targetY <= 0)
            {
                content.anchoredPosition = new Vector2(0, 0);
                return false;
            }

            else
            {
                return true;
            }
        }
    }
}
