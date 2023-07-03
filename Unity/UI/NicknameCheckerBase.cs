/*
작성자: 최재호 (cjh0798@gmail.com)
기능: InputFiled의 비속어 필터링
 */
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.RegularExpressions;

public class NicknameCheckerBase : MonoBehaviour
{
    public TMP_InputField input;
    public TMP_Text warningText;

    protected bool isCreatable;

    // 비속어 및 닉네임 길이 체크
    virtual public void CheckNickname()
    {
        CheckNicknameLength();
        CheckWord();
    }

    // 닉네임 길이 체크
    virtual public void CheckNicknameLength()
    {
        byte[] nicknameByte = Encoding.Unicode.GetBytes(input.text);
        int length = nicknameByte.Length;
        
        if(length == 0)
        {
            isCreatable = false;
            warningText.text = "닉네임을 입력해 주세요.";
        }
        else if (length < 4)
        {
            isCreatable = false;
            warningText.text = "2~10자의 한글과 영문을 입력해 주세요.";
        }
        else if (length > 20)
        {
            isCreatable = false;
            warningText.text = "2~10자의 한글과 영문을 입력해 주세요.";
        }
        else
        {
            warningText.text = "";
            isCreatable = true;
        }
    }

    // 띄어쓰기 및 특수문자 체크
    virtual public void CheckWord()
    {
        string idChecker = Regex.Replace(input.text, @"[^a-zA-Z가-힣]", "", RegexOptions.Singleline);
        if (!input.text.Equals(idChecker))
        {
            isCreatable = false;
            warningText.text = "특수문자, 초성, 숫자, 공백은 사용할 수 없습니다."; 
        }
    }
}
