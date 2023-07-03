/*
작성자: 최재호(cjh0798@gamil.com)
기능: 갤러리 오픈 및 사진 선택
 */
using CompactExifLib;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GalleryController : MonoBehaviour
{
    public string filePath;
    protected string fullPath;
    public MainSceneLoading loading;
    public List<RectTransform> images = new List<RectTransform>();
    public Vector2 imageSize;

    public delegate void OnOpenGalleryAction();
    public delegate void OnSelectGalleryImageAction();
    public delegate void OnClosedGalleryAction();
    public delegate void OnCompletedCropImageAction();
    public OnOpenGalleryAction OnOpenGallery;
    public OnSelectGalleryImageAction OnSelectGalleryImage;
    public OnClosedGalleryAction OnClosedGallery;

    [HideInInspector]
    public List<GalleryImage> selectedImages;
    public MainTextureDisposer textureDisposer;
    public MainPage pageName;
    protected List<string> selectedImagePath;
    protected List<Texture> texList;

    #region 이미지 메타데이터 Key

    private const int KEY_LATITUDE = 0;
    private const int KEY_LONGITUDE = 1;
    private const int KEY_ORIENTATION = 2;
    private const int KEY_IMAGE_WIDTH = 3;
    private const int KEY_IMAGE_LENGTH = 4;
    private const int KEY_DATETIME = 5;
    private const int KEY_MODEL = 6;
    private const int KEY_PIXELDIMENSIONX = 7;
    private const int KEY_PIXELDIMENSIONY = 8;
    private const int KEY_MANUFACTURER = 9;
    #endregion

    public virtual void Start()
    {
        OnOpenGallery = DoReset;
        OnSelectGalleryImage = () => { };
        OnClosedGallery = () => { };
        selectedImagePath = new List<string>();
        selectedImages = new List<GalleryImage>();
        texList = new List<Texture>();

        fullPath = $"{Application.persistentDataPath}/Image/{filePath}/";
    }

    public virtual void CheckWritePermission()
    {
        // 파일 접근 권한이 없으면 팝업 띄우기
        if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite) == false)
        {
            Debug.Log("파일 접근 권한이 없음");
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }
    }

    // 로컬 폴더에 있는 이미지 삭제
    public virtual async void DeleteImage(int _fileName)
    {
        string path = $"{Application.persistentDataPath}/Image/{filePath}/";
        List<string> fileList = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).OrderBy(s => new FileInfo(s).Name).ToList();

        await UniTask.RunOnThreadPool(() =>
        {
            fileList.Remove($"{path + _fileName}.jpg");
            File.Delete($"{path + _fileName}.jpg");
        });

        for (int i = _fileName; i < fileList.Count; i++)
        {
            string oldFile = fileList[i];
            string newFile = path + i + ".jpg";
            File.Move(oldFile, newFile);
        }
    }

    // 갤러리 이미지를 클릭 했을 때
    public virtual async void SelectImage()
    {
        string path = null;
        CheckWritePermission();

        OnOpenGallery();

#if UNITY_EDITOR

        path = Directory.GetFiles($"{Application.persistentDataPath}/Image/Editor")[0];
        await UpdateGalleryImage(path, 0);
        //SetActiveOnImage();

        OnClosedGallery();

#elif UNITY_ANDROID || UNITY_IOS
        NativeGallery.GetImageFromGallery(async (_path) =>
        {
            if (string.IsNullOrEmpty(_path))
                return;

            await UpdateGalleryImage(_path, 0);
            //SetActiveOnImage();
            
            OnClosedGallery();
        });
#endif
    }

    public virtual async void SelectImages()
    {
        CheckWritePermission();
        string[] path = null;

        OnOpenGallery();

#if UNITY_EDITOR
        path = Directory.GetFiles($@"{Application.persistentDataPath}/Image/Editor");
        for (int i = 0; i < path.Length; i++)
        {
            selectedImagePath.Add(path[i]);

            OnSelectGalleryImage();

            await UpdateGalleryImage(path[i], i);
            await UniTask.Yield();
        }

        await UniTask.WaitUntil(() => selectedImages.Count == selectedImagePath.Count);
        selectedImages = selectedImages.OrderBy(x => x.index).ToList();

        SetActiveOnImage();
        OnClosedGallery();

#elif UNITY_ANDROID || UNITY_IOS
        NativeGallery.GetImagesFromGallery(async (_path) =>
        {
            GetImagesFromGallery(_path).Forget();
        });

#endif
        //NativeGallery.GetImagesFromGallery(async (_path) =>
        //{
        //    if (_path.Length == 0)
        //        return;

        //    path = _path;
        //    for (int i = 0; i < path.Length; i++)
        //    {
        //        if (i == 10)
        //            break;

        //        selectedImagePath.Add(path[i]);

        //        OnSelectGalleryImage();

        //        await UpdateGalleryImage(path[i], i);
        //        await UniTask.Yield();
        //    }

        //    await UniTask.WaitUntil(() => selectedImages.Count == selectedImagePath.Count);
        //    selectedImages = selectedImages.OrderBy(x => x.index).ToList();


        //    SetActiveOnImage();
        //    OnClosedGallery();
        //});
    }

    private async UniTask GetImagesFromGallery(string[] path)
    {
        if (path.Length == 0)
            return;

        for (int i = 0; i < path.Length; i++)
        {
            if (i == 10)
                break;

            selectedImagePath.Add(path[i]);

            OnSelectGalleryImage();

            await UpdateGalleryImage(path[i], i);
            await UniTask.Yield();
        }

        await UniTask.WaitUntil(() => selectedImages.Count == selectedImagePath.Count);
        selectedImages = selectedImages.OrderBy(x => x.index).ToList();


        SetActiveOnImage();
        OnClosedGallery();
    }

    // 이미지 크기 조정
    public virtual (int, int) ResizeImage(RectTransform _image, Texture _tex)
    {
        float framewidth = 0;
        float frameheight = 0;

        if (_tex.width > imageSize.x * 2)
        {
            int width = ShortenImage(_tex.width, imageSize.x);
            framewidth = width;
        }
        else if (_tex.width > imageSize.x)
        {
            if (_tex.width > _tex.height)
            {
                frameheight = imageSize.y;

                framewidth = (_tex.width * frameheight) / _tex.height;
                framewidth = framewidth <= imageSize.x ? imageSize.x : framewidth;
            }
            else
            {
                framewidth = _tex.width;
            }
        }
        else
        {
            framewidth = imageSize.x;
        }

        frameheight = (_tex.height * framewidth) / _tex.width;
        frameheight = frameheight <= imageSize.y ? imageSize.y : frameheight;

        _image.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, framewidth);
        _image.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, frameheight);
        _image.GetComponent<RawImage>().texture = _tex;

        return ((int)framewidth, (int)frameheight);
    }

    // 로컬 폴더에 이미지 저장
    public virtual void SaveImage(string _originPath, string _fileName, Texture2D _Imgtexture)
    {
        if (string.IsNullOrEmpty(fullPath))
            return;

        if (Directory.Exists(fullPath) == false)
        {
            Directory.CreateDirectory(fullPath);
        }

        Debug.Log("_originPath: " + _originPath);
        File.WriteAllBytes(fullPath + "/" + _fileName + ".jpg", _Imgtexture.EncodeToJPG());
    }


    protected Texture2D ResizeTexture2D(Texture2D originalTexture, int resizedWidth, int resizedHeight, int limitWidth, int limitHeight)
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

    // 갤러리 이미지 업데이트
    protected async UniTask UpdateGalleryImage(string _path, int _imageIndex)
    {
        RawImage targetImage = images[_imageIndex].GetComponent<GalleryImage>().image;
        GalleryImage galleryImage = images[_imageIndex].GetComponent<GalleryImage>();
        FileInfo info = new FileInfo(_path);

#if UNITY_EDITOR
        //Texture2D tex = await GetTexture(_path);
        Texture2D tex = NativeGallery.LoadImageAtPath(_path, int.MaxValue, false, false, false);

#elif UNITY_ANDROID
        if (Application.isMobilePlatform)
        {
            string[] metaData = GetImageMetaData(_path);
            if (metaData != null)
            {
                Debug.Log("metaData != null");
                galleryImage.orientation = metaData[KEY_ORIENTATION] == "null" ? "" : metaData[KEY_ORIENTATION];
                galleryImage.width = metaData[KEY_IMAGE_WIDTH] == "null" ? "" : metaData[KEY_IMAGE_WIDTH];
                galleryImage.height = metaData[KEY_IMAGE_LENGTH] == "null" ? "" : metaData[KEY_IMAGE_LENGTH];
                galleryImage.latitude = metaData[KEY_LATITUDE] == "null" ? "" : metaData[KEY_LATITUDE];
                galleryImage.longitude = metaData[KEY_LONGITUDE] == "null" ? "" : metaData[KEY_LONGITUDE];
                galleryImage.model = metaData[KEY_MODEL] == "null" ? "" : metaData[KEY_MODEL];
                galleryImage.size = info.Length.ToString();
                galleryImage.date = metaData[KEY_DATETIME] == "null" ? "" : metaData[KEY_DATETIME];
                galleryImage._name = info.Name;
                galleryImage.manufacturer = metaData[KEY_MANUFACTURER] == "null" ? "" : metaData[KEY_MANUFACTURER];
                galleryImage.extension = "jpg";
            }
        }
        // Texture2D tex = await GetTexture(_path);
        Texture2D tex = NativeGallery.LoadImageAtPath(_path, int.MaxValue, false, false, false);
#elif UNITY_IOS
        Texture2D tex = NativeGallery.LoadImageAtPath(_path, int.MaxValue, false, false, false);
#endif


        tex.name = $"{filePath}Texture";
        SaveImage(_path, _imageIndex.ToString(), tex);

        // 세로 이미지
        if (galleryImage?.orientation == "6")
        {
            Texture2D rotatedTex = RotateTexture(tex, true);
            tex = rotatedTex;
        }

        galleryImage.imagePath = _path;
        galleryImage.index = _imageIndex;
        galleryImage.isSelected = true;
        selectedImages.Add(galleryImage);

        (int resizedWidth, int resizedHeight) = ResizeImage(targetImage.GetComponent<RectTransform>(), tex);
        Texture2D resultTex = new Texture2D(2, 2, TextureFormat.ASTC_4x4, false);
        resultTex = ResizeTexture2D(tex, resizedWidth, resizedHeight, (int)images[0].rect.width, (int)images[0].rect.height);
        targetImage.texture = resultTex;

        if (filePath != "PlaceRegist")
            targetImage.transform.parent.gameObject.SetActive(true);

        texList.Add(resultTex);
        DestroyImmediate(tex);
    }

    public async UniTask<Texture2D> GetTexture(string _path)
    {
        string uri = $@"file://{_path}";
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(uri))
        {
            try
            {
                Debug.Log("uri: " + uri);
                return await GetTextureAction(uri);

            }
            // file://으로 불러오지 못했을 때 file:///으로 재시도
            catch (UnityWebRequestException e)
            {
                Debug.Log(e.Result);
                uri = $@"file:///{_path}";
                Debug.Log("uri: " + uri);
                return await GetTextureAction(uri);
            }
        }
    }

    private Texture2D RotateTexture(Texture2D originalTexture, bool clockwise)
    {
        Color32[] original = originalTexture.GetPixels32();
        Color32[] rotated = new Color32[original.Length];
        int w = originalTexture.width;
        int h = originalTexture.height;

        int iRotated, iOriginal;

        for (int j = 0; j < h; ++j)
        {
            for (int i = 0; i < w; ++i)
            {
                iRotated = (i + 1) * h - j - 1;
                iOriginal = clockwise ? original.Length - 1 - (j * w + i) : j * w + i;
                rotated[iRotated] = original[iOriginal];
            }
        }

        Texture2D rotatedTexture = new Texture2D(h, w);
        rotatedTexture.SetPixels32(rotated);
        rotatedTexture.Apply();
        return rotatedTexture;
    }

    private async UniTask<Texture2D> GetTextureAction(string _uri)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(_uri))
        {

            await request.SendWebRequest();

            Texture2D tex = DownloadHandlerTexture.GetContent(request);
            textureDisposer.AddTexture(pageName, tex);
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("텍스쳐 데이터 result: " + request.result);
                return null;
            }
            else
            {
                return tex;
            }
        }
    }

    // 리셋
    public virtual void DoReset()
    {
        for (int i = 0; i < images.Count; i++)
        {
            if (selectedImages.Count == 0)
                break;

            images[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < selectedImages.Count; i++)
        {
            selectedImages[i].image.texture = null;
            selectedImages[i].isSelected = false;
            selectedImages[i].isCropped = false;
        }
        for (int i = 0; i < texList.Count; i++)
        {
            DestroyImmediate(texList[i]);
        }

        selectedImagePath.Clear();
        selectedImages.Clear();

        string address = fullPath;
        if (Directory.Exists(address) == false)
        {
            return;
        }

        foreach (var element in Directory.GetFiles(address))
        {
            File.Delete(element);
        }
    }

    // 이미지 사이즈 줄이기
    protected int ShortenImage(int _curSize, float targetSize)
    {
        while (_curSize > targetSize * 2)
        {
            _curSize = _curSize / 2;
        }

        return _curSize;
    }

    protected int StretchImage(int _curSize, float targetSize)
    {
        while (_curSize < targetSize)
        {
            _curSize = _curSize * 2;
        }

        return _curSize;
    }


    // 이미지 On
    protected void SetActiveOnImage()
    {
        for (int i = 0; i < selectedImages.Count; i++)
        {
            selectedImages[i].gameObject.SetActive(true);
        }
    }

    // 이미지 메타 정보 가져오기
    private string[] GetImageMetaData(string _path)
    {
        using (AndroidJavaClass pluginClass = new AndroidJavaClass("com.example.plugin.UnityPlugin"))
        {
            using (AndroidJavaObject instance = pluginClass.CallStatic<AndroidJavaObject>("instance"))
            {
                try
                {
                    string[] result = instance.Call<string[]>("getAllMeta", _path);

                    Debug.Log("Successed To Get Meta Data");
                    return result;
                }
                catch
                {
                    return null;
                }

            }

        }
    }

}
