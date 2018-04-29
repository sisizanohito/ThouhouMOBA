using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMnager : SingletonMonoBehaviour<InputMnager>
{
    public UTouchPhase Phase { get; private set; } //タッチの状態を保持
    public Vector2 Position { get; private set; }  //タッチ時のワールド座標を保持
    public Vector2 WindowPosition { get; private set; }  //タッチ時のワールド座標を保持
    public bool DashFlag { get; private set; }
    public float SwipeSpeed { get; private set; }
    public Vector2 MovePosition { get; private set; }
    [SerializeField] private float Delta = 200;
    private Vector2 Tposition;
    private int DragCount;
    private bool InputMnagerFlag;
    protected override void Init()
    {
        base.Init();
        Phase = UTouchPhase.None;
        Position = new Vector2(0,0);
        WindowPosition = new Vector2(0, 0);
        InputMnagerFlag = true;
        DragCount = 0;
        Debug.Log("インプットマネージャー");
    }

    void Update()
    {
        if (!InputMnagerFlag)
        {
            return;
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

    public enum UTouchPhase
    {
        // 概要:
        //     ///
        //     A finger touched the screen.
        //     ///
        Began = 0,
        //
        // 概要:
        //     ///
        //     A finger moved on the screen.
        //     ///
        Moved = 1,
        //
        // 概要:
        //     ///
        //     A finger is touching the screen but hasn't moved.
        //     ///
        Stationary = 2,
        //
        // 概要:
        //     ///
        //     A finger was lifted from the screen. This is the final phase of a touch.
        //     ///
        Ended = 3,
        //
        // 概要:
        //     ///
        //     The system cancelled tracking for the touch.
        //     ///
        Canceled = 4,
        // 概要:
        //     ///
        //     何もしてない
        //     ///
        None = 5
    }
}
