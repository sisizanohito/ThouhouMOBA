// BEGIN MIT LICENSE BLOCK //
//
// Copyright (c) 2016 dskjal
// This software is released under the MIT License.
// http://opensource.org/licenses/mit-license.php
//
// END MIT LICENSE BLOCK   //
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TPVCamera : MonoBehaviour
{
    public Transform Target;
    public float DistanceToPlayerM = 2f;    // カメラとプレイヤーとの距離[m]
    public float SlideDistanceM = 0f;       // カメラを横にスライドさせる；プラスの時右へ，マイナスの時左へ[m]
    public float HeightM = 1.2f;            // 注視点の高さ[m]
    public float RotationSensitivity = 100f;// 感度
    private Vector2 mouse;

    void Start()
    {
        if (Target == null)
        {
            Debug.LogError("ターゲットが設定されていない");
            Application.Quit();
        }
        TMInputMnager.Instance.OnMoveMouse += OnMoveMouse;
        mouse = new Vector2();
    }

    void FixedUpdate()
    {
        //var rotX = Input.GetAxis("Mouse X") * Time.deltaTime * RotationSensitivity;
        var rotY = mouse.y * Time.fixedDeltaTime * RotationSensitivity;

        var lookAt = Target.position + Vector3.up * HeightM;

        // 回転
        transform.RotateAround(lookAt, Vector3.up, 0);
        // カメラがプレイヤーの真上や真下にあるときにそれ以上回転させないようにする
        if (transform.forward.y > 0.9f && rotY < 0)
        {
            rotY = 0;
        }
        if (transform.forward.y < -0.9f && rotY > 0)
        {
            rotY = 0;
        }
        transform.RotateAround(lookAt, transform.right, rotY);

        // カメラとプレイヤーとの間の距離を調整
        transform.position = lookAt - transform.forward * DistanceToPlayerM;

        // 注視点の設定
        transform.LookAt(lookAt);

        // カメラを横にずらして中央を開ける
        transform.position = transform.position + transform.right * SlideDistanceM;
    }

    private void OnDisable()
    {
        if (TMInputMnager.IsInstance())
        {
            TMInputMnager.Instance.OnMoveMouse -= OnMoveMouse;
        }
    }

    private void OnMoveMouse(Vector2 mouse)
    {
        this.mouse = mouse;
    }
}