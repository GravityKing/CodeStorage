/*
작성자: 최재호(cjh0798@gmail.com)
기능: 피드,여행 게시물의 유틸 기능
1. 해쉬태그 추출
2. 링크 텍스트 추출
3. 날자 계산
 */
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
public class ContentUtil : MonoBehaviour
{
    // 해쉬태그 텍스트를 Link텍스트로 변환
    public string ConvertToLink(string text)
    {
        if (string.IsNullOrEmpty(text))
            return "";

        List<string> tagList = HashTagSplit(text);

        text = text.Replace("#", "");
        for (int j = 0; j < tagList.Count; j++)
        {
            text = text.Replace(tagList[j], $"<color=grey><link={tagList[j]}>#{tagList[j]}</link></color>");

        }
        return text;
    }

    // 태그 검출기
    public List<string> HashTagSplit(string Words)
    {
        string word;
        string temp;
        char[] delimiterChars = { ' ', ',' };

        List<string> _regPlaceTag = new List<string>();

        word = Words.Replace("#", " #");
        string[] split = word.Split(delimiterChars);
        for (int i = 0; i < split.Length; i++)
        {
            if (Regex.IsMatch(split[i], @"^#"))
            {
                temp = split[i].Replace("#", string.Empty);
                if (!Regex.IsMatch(temp, @"[^a-zA-Z0-9가-힇ㄱ-ㅎㅏ-ㅣ_]{160}$"))
                {
                    temp = temp.Replace(" ", string.Empty);
                    if (!temp.Equals(string.Empty))
                    {
                        _regPlaceTag.Add(temp);
                    }
                }
            }
        }
        return _regPlaceTag;
    }

    // 날자 계산기
    public string ChangeTimeList(string startDateTime)
    {
        string resultDateTime = "";

        TimeSpan timespan = DateTime.Now - Convert.ToDateTime(startDateTime);

        if (timespan.Days >= 7)
        {
            resultDateTime = Convert.ToDateTime(startDateTime).ToString("yy.MM.dd");
        }
        else if (timespan.Days > 0)
        {
            resultDateTime = timespan.Days.ToString() + "일 전";
        }
        else if (timespan.Hours > 0)
        {
            resultDateTime = timespan.Hours + "시간 전";
        }
        else if (timespan.Minutes > 0)
        {
            resultDateTime = timespan.Minutes + "분 전";
        }
        else if (timespan.Seconds >= 0)
        {
            resultDateTime = "방금";
        }
        else
        {
            resultDateTime = ("");
        }

        return resultDateTime;
    }

    // 숫자를 K,M으로 변환
    public string ConvertNumberToKM(int _num)
    {
        float newNum = 0;
        float _decimal = 0;
        string result = null;

        // M
        if (_num >= 1000000)
        {
            newNum = (float)_num / 1000000;
            _decimal = newNum - (float)Math.Truncate(newNum); // newNum - 소수점을 버린 newNum

            // 소수점 표시
            if (_decimal >= 0.1f)
            {
                result = string.Format("{0:0.0}", newNum) + "M";
            }
            // 소수점 미표시
            else
            { 
                result = string.Format("{0:0}", newNum) + "M"; ;
            }
            return result;
        }
        // K
        else if (_num >= 1000)
        {
            newNum = (float)_num / 1000;
            _decimal = newNum - (float)Math.Truncate(newNum); // newNum - 소수점을 버린 newNum

            // 소수점 표시
            if (_decimal >= 0.1f)
            {
                result = string.Format("{0:0.0}", newNum) + "K";
            }
            // 소수점 미표시
            else
            {
                result = string.Format("{0:0}", newNum) + "K";
            }
            return result;
        }
        else
            return _num.ToString();
    }

}
