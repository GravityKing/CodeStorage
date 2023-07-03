using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RectSizeHelper : MonoBehavior
{
    // Rect 사이즈 1:1로 조절
    private Rect AdjustTexture(Texture2D texture)
    {
        Rect rect;
        if (texture.height > texture.width)
        {
            float y = (texture.height - texture.width) / 2;
            rect = new Rect(0, y, texture.width, texture.width);
        }

        else if (texture.height == texture.width)
        {
            rect = new Rect(0, 0, texture.width, texture.width);
        }

        else
        {
            float x = (texture.width - texture.height) / 2;
            rect = new Rect(x, 0, texture.height, texture.height);
        }

        return rect;
    }

}
