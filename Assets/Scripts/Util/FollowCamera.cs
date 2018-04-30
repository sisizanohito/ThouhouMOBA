using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour {
    
    private GameObject Player;
    Transform standardPos;			// the usual position for the camera, specified by a transform in the game
    [SerializeField]
    private float smooth = 3f;

    // スムーズフラグ
    public bool smoothSwitch = true; 

    // Use this for initialization
    void Start () {

        // 各参照の初期化
        standardPos = GameObject.Find("CamPos").transform;


        //カメラをスタートする
        transform.position = standardPos.position;
        transform.forward = standardPos.forward;

    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (smoothSwitch == true)
        {
            // the camera to standard position and direction
            transform.position = Vector3.Lerp(transform.position, standardPos.position, Time.fixedDeltaTime * smooth);
            transform.forward = Vector3.Lerp(transform.forward, standardPos.forward, Time.fixedDeltaTime * smooth);
        }
        else
        {
            // the camera to standard position and direction / Quick Change
            transform.position = standardPos.position;
            transform.forward = standardPos.forward;
            smoothSwitch = false;
        }
    }
}
