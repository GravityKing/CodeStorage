using UnityEngine;
using UnityEngine.Events;
using TMPro;

// InputFiled 컴포넌트에 OnSubmit이 없기 때문에
// 해당 스크립트의 유니티 이벤트로 대체(22-04-27 최재호)
public class InputFiledOnSubmit : MonoBehaviour
{
    public bool isCommentInput;
    [SerializeField] UnityEvent OnSubmit;

    private void Start()
    {
        TMP_InputField input = GetComponent<TMP_InputField>();
        input.onSubmit.AddListener((t) => OnSubmit.Invoke());


    }

#if UNITY_EDITOR
    private void Update()
    {
        if (isCommentInput == false)
            return;

        TMP_InputField input = GetComponent<TMP_InputField>();
        if (Input.GetKeyDown(KeyCode.Return) && string.IsNullOrEmpty(input.text) == false)
        {
            if (transform.root.GetComponent<Canvas>().enabled == false)
                return;

            OnSubmit.Invoke();
        }
    }
#endif
}
