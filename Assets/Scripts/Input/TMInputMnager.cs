using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class TMInputMnager : SingletonMonoBehaviour<TMInputMnager>
{
    public Action<List<KeyCode>> OnKey;
    public Action<List<KeyCode>> OnKeyDown;
    public Action<List<KeyCode>> OnKeyUP;
    public Action<Vector2> OnMoveMouse;


    /// <summary>
    /// 処理負荷軽減のため、KeyCodeの全ての値を保持する配列
    /// </summary>
    static Array KeyCodeList;

    private bool InputMnagerFlag;

    protected override void Init()
    {
        Cursor.lockState = CursorLockMode.Locked;
        InputMnagerFlag = true;
        KeyCodeList = Enum.GetValues(typeof(KeyCode));
        base.Init();
        Debug.Log("インプットマネージャー");
    }

    void Update()
    {
        if (!InputMnagerFlag)
        {
            return;
        }
        var tmpList = new List<KeyCode>();
        var tmpUpList = new List<KeyCode>();
        var tmpDownList = new List<KeyCode>();

        foreach (KeyCode code in KeyCodeList)
        {//キーがあるかチェック
            if (Input.GetKey(code))
            {
                tmpList.Add(code);
            }
            if (Input.GetKeyUp(code))
            {
                tmpUpList.Add(code);
            }
            if (Input.GetKeyDown(code))
            {
                tmpDownList.Add(code);
            }
        }

        if (tmpUpList.Count != 0 && OnKeyUP != null)
        {//1つでも押上キーがあれば
            OnKeyUP(tmpUpList);
        }

        if (tmpList.Count != 0 && OnKey != null)
        {//1つでもキーがあれば
            OnKey(tmpList);
        }
       
        if (tmpDownList.Count != 0 && OnKeyDown != null)
        {//1つでも押下キーがあれば
            OnKeyDown(tmpDownList);
        }

        var mouse = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        if (OnMoveMouse != null)
        {
            OnMoveMouse(mouse);
        }
        


    }

    public void OnInputMnager()
    {
        Init();
    }
    public void OffInputMnager()
    {
        Init();
        InputMnagerFlag = false;
    }

    
}
