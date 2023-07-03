using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMouseCursor : MonoBehaviour
{
    public DialogManager2 dialogManager2;
    [SerializeField] Texture2D cursorImg;
    [SerializeField] DialogManager dialogManager;
    void Start()
    {
        Cursor.SetCursor(cursorImg, Vector2.zero, CursorMode.ForceSoftware);


    }

    void Update()
    {
        if (dialogManager.dialogNum == 5)
        {
            Cursor.visible = false;
        }
    }
}
