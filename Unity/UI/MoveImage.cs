using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MoveImage : MonoBehaviour,IDragHandler
{
    public enum State
    {
        feed,
        profile,
    }
    
    public State state;
    public float speed = 100.0f;
    private Vector3 preMousePos;
    private RectTransform rectTransform;
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        preMousePos = Input.mousePosition;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (Application.isMobilePlatform)
        {
            if (Input.touchCount == 1)
            {
                Vector3 mousePos = eventData.position;
                mousePos -= preMousePos;
                GetComponent<RectTransform>().transform.position += mousePos * speed * Time.deltaTime * transform.localScale.x;
            }
        }
        else
        {
            Vector3 mousePos = eventData.position;
            mousePos -= preMousePos;
            rectTransform.position += mousePos * speed * Time.deltaTime * transform.localScale.x;
        }

    }


}
