/*
작성자: 최재호(cjh0798@gmail.com)
기능: UVRect 조정
 */
using System;
using UnityEngine;
using UnityEngine.UI;

public class UVRectHelper : MonoBehaviour
{
    protected float m_aspectRatio = 1.0f;
    protected float m_rectAspectRatio = 1.0f;

    // RawImage uvRect 맞추기
    public void AdjustAspect(RawImage m_image)
    {
        SetupImage(m_image);

        bool fitY = m_aspectRatio < m_rectAspectRatio;

        SetAspectFitToImage(m_image, fitY, m_aspectRatio);
    }

    private void SetupImage(RawImage m_image)
    {
        CalculateImageAspectRatio(m_image);
        CalculateTextureAspectRatio(m_image);
    }

    private void CalculateImageAspectRatio(RawImage m_image)
    {
        RectTransform rt = m_image.GetComponent<RectTransform>();
        m_rectAspectRatio = rt.sizeDelta.x / rt.sizeDelta.y;
    }

    private void CalculateTextureAspectRatio(RawImage m_image)
    {
        if (m_image == null)
        {
            Debug.Log("CalculateAspectRatio: m_image is null");
            return;
        }

        Texture2D texture = (Texture2D)m_image.texture;
        if (texture == null)
        {
            Debug.Log("CalculateAspectRatio: texture is null");
            return;
        }


        m_aspectRatio = (float)texture.width / texture.height;
        //Debug.Log("textW=" + texture.width + " h=" + texture.height + " ratio=" + m_aspectRatio);
    }


    private void SetAspectFitToImage(RawImage _image, bool yOverflow, float displayRatio)
    {
        if (_image == null)
        {
            return;
        }

        Rect rect = new Rect(0, 0, 1, 1);   // default
        if (yOverflow)
        {

            rect.height = m_aspectRatio / m_rectAspectRatio;
            rect.y = (1 - rect.height) * 0.5f;
        }
        else
        {
            rect.width = m_rectAspectRatio / m_aspectRatio;
            rect.x = (1 - rect.width) * 0.5f;

        }
        _image.uvRect = rect;
    }
}
