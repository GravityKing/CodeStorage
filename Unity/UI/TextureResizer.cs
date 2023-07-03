using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TextureResizer : MonoBehaviour
{
    public Texture2D ResizeTexture2D(Texture2D originalTexture, int resizedWidth, int resizedHeight, int limitWidth, int limitHeight)
    {
        resizedWidth = resizedWidth < limitWidth ? limitWidth : resizedWidth;
        resizedHeight = resizedHeight < limitHeight ? limitHeight : resizedHeight;

        RenderTexture renderTexture = new RenderTexture(resizedWidth, resizedHeight, 32);
        RenderTexture.active = renderTexture;
        Graphics.Blit(originalTexture, renderTexture);
        Texture2D resizedTexture = new Texture2D(resizedWidth, resizedHeight);

        resizedTexture.ReadPixels(new Rect(0, 0, resizedWidth, resizedHeight), 0, 0);
        resizedTexture.Apply();
        resizedTexture.name = $"{filePath}ResizedTexture";
        DestroyImmediate(renderTexture);
        return resizedTexture;
    }

}
