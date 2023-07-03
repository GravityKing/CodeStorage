/*
작성자: 최재호(cjh0798@gamil.com)
기능: 메인 씬 팝업 관리자
*/
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MainPopupManager : MonoBehaviour
{
    public List<GameObject> popupList;

    public void HidePopup(string _key)
    {
        GameObject popup = popupList.Find(p => p.name == _key);

        if (popup == null)
        {
            Debug.LogError(_key + "이름을 가진 팝업이 없습니다.");
        }
        else
        {
            popup.gameObject.SetActive(false);
        }
    }

    public GameObject GetPopup(string _key)
    {
        GameObject popup = popupList.Find(p => p.name == _key);
        return popup;
    }

    public void ShowPopup(string _key)
    {
        GameObject popup = popupList.Find(p => p.name == _key);
        OnPopup(popup, _key);
    }

    public void ShowPopup(string _key, string _title)
    {
        GameObject popup = popupList.Find(p => p.name == _key);
        popup.GetComponent<MainPopup>().title.text = _title;
        OnPopup(popup, _key);
    }

    public void ShowPopup(string _key, string _title, string _subTitle)
    {
        GameObject popup = popupList.Find(p => p.name == _key);
        popup.GetComponent<MainPopup>().title.text = _title;
        popup.GetComponent<MainPopup>().subTitle.text = _subTitle;
        OnPopup(popup, _key);
    }

    /// <summary>
    /// _btnAction = 왼쪽 버튼에 사용할 함수
    /// </summary>
    public void ShowPopup(string _key, UnityAction _btnAction)
    {
        GameObject obj = popupList.Find(p => p.name == _key);
        MainPopup popup = obj.GetComponent<MainPopup>();
        OnPopupWithAction(popup, name, _btnAction);
    }

    /// <summary>
    /// _btnAction = 왼쪽 버튼에 사용할 함수
    /// </summary>
    public void ShowPopup(string _key, string _title,UnityAction _btnAction)
    {
        GameObject obj = popupList.Find(p => p.name == _key);
        MainPopup popup = obj.GetComponent<MainPopup>();
        popup.title.text = _title;
        OnPopupWithAction(popup, name, _btnAction);
    }

    /// <summary>
    /// _btnAction = 왼쪽 버튼에 사용할 함수
    /// </summary>
    public void ShowPopup(string _key, string _title, string _subTitle, UnityAction _btnAction)
    {
        GameObject obj = popupList.Find(p => p.name == _key);
        MainPopup popup = obj.GetComponent<MainPopup>();
        popup.title.text = _title;
        popup.subTitle.text = _subTitle;
        OnPopupWithAction(popup, name, _btnAction);
    }

    private void OnPopup(GameObject _popup, string _key)
    {

        if (_popup == null)
        {
            Debug.LogError(_key + "이름을 가진 팝업이 없습니다.");
        }
        else
        {
            _popup.gameObject.SetActive(true);
        }
    }

    private async void OnPopupWithAction(MainPopup _popup, string _key, UnityAction _action)
    {
        if (_popup != null)
        {
            OnPopup(_popup.gameObject, _key);
            UnityAction removeAction = () => _popup.lBtn.onClick.RemoveListener(_action);
            _popup.lBtn?.onClick.AddListener(_action);
            _popup.rBtn?.onClick.AddListener(removeAction);

            await _popup.lBtn.OnClickAsync();

            _popup.lBtn?.onClick.RemoveListener(_action);
            _popup.rBtn?.onClick.RemoveListener(removeAction);

        }
        else
        {
            Debug.LogError(_key + "팝업에 MainPopup 스크립트가 없습니다.");
        }
    }
}
