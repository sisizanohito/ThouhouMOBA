using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Frandle : MonoBehaviour
{
    public float animSpeed = 1.5f;              // アニメーション再生速度設定
    public float lookSmoother = 3.0f;           // a smoothing setting for camera motion
    public bool useCurves = true;               // Mecanimでカーブ調整を使うか設定する
                                                // このスイッチが入っていないとカーブは使われない
    public float useCurvesHeight = 0.5f;        // カーブ補正の有効高さ（地面をすり抜けやすい時には大きくする）

    // 以下キャラクターコントローラ用パラメタ
    // 前進速度
    public float forwardSpeed = 7.0f;
    // 後退速度
    public float backwardSpeed = 2.0f;
    // 旋回速度
    public float rotateSpeed = 2.0f;
    // ジャンプ威力
    public float jumpPower = 3.0f;
    // 回転補完
    public float smoothAngle = 3.0f;
    // キャラクターコントローラ（カプセルコライダ）の参照
    private CapsuleCollider col;
    private Rigidbody rb;

    // CapsuleColliderで設定されているコライダのHeiht、Centerの初期値を収める変数
    private float orgColHight;
    private Vector3 orgVectColCenter;

    private Animator anim;                          // キャラにアタッチされるアニメーターへの参照
    private AnimatorStateInfo currentBaseState;         // base layerで使われる、アニメーターの現在の状態の参照

    private GameObject cameraObject;            // メインカメラへの参照
    private bool forward, back, right, left;    // 移動フラグ
    private bool jumpFlag;                      // ジャンプのフラグ
    private Vector2 mouse;
    private float smoothDirection = 0.0f;

    // アニメーター各ステートへの参照
    static int idleState = Animator.StringToHash("Idle");
    static int locoState = Animator.StringToHash("Locomotion");
    static int jumpState = Animator.StringToHash("Jump");
    static int restState = Animator.StringToHash("Rest");

    // Use this for initialization
    void Start()
    {
        jumpFlag = false;
        forward = back = right = left = false;
        back = false;
        mouse = new Vector2();

        // Animatorコンポーネントを取得する
        anim = GetComponent<Animator>();
        // CapsuleColliderコンポーネントを取得する（カプセル型コリジョン）
        col = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        //メインカメラを取得する
        cameraObject = GameObject.FindWithTag("MainCamera");
        // CapsuleColliderコンポーネントのHeight、Centerの初期値を保存する
        orgColHight = col.height;
        orgVectColCenter = col.center;

        TMInputMnager.Instance.OnKeyDown += OnKeyDown;
        TMInputMnager.Instance.OnKeyUP += OnKeyUp;
        TMInputMnager.Instance.OnMoveMouse += OnMoveMouse;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
            // キャラクターコントローラ（カプセルコライダ）の移動量
        Vector3 velocity = GetVelocity();
        smoothDirection = Mathf.Lerp(smoothDirection, velocity.x, Time.fixedDeltaTime * smoothAngle);
        anim.SetFloat("Speed", velocity.magnitude);                          // Animator側で設定している"Speed"パラメタにvを渡す
        anim.SetBool("Back", back);
        anim.SetFloat("Direction", smoothDirection);                      // Animator側で設定している"Direction"パラメタにhを渡す
        anim.speed = animSpeed;                             // Animatorのモーション再生速度に animSpeedを設定する
        currentBaseState = anim.GetCurrentAnimatorStateInfo(0); // 参照用のステート変数にBase Layer (0)の現在のステートを設定する


        rb.useGravity = true;//ジャンプ中に重力を切るので、それ以外は重力の影響を受けるようにする
        // キャラクターのローカル空間での方向に変換
        var moveVelocity = transform.TransformDirection(velocity).normalized;

        if (!back && moveVelocity.magnitude > 0)
        {
            moveVelocity *= forwardSpeed;       // 移動速度を掛ける
        }
        else if (back && moveVelocity.magnitude > 0)
        {
            moveVelocity *= backwardSpeed;  // 移動速度を掛ける
        }


        if (jumpFlag)
        {   // スペースキーを入力したら

            //アニメーションのステートがLocomotionの最中のみジャンプできる
            if (currentBaseState.shortNameHash == locoState)
            {
                //ステート遷移中でなかったらジャンプできる
                if (!anim.IsInTransition(0))
                {
                    rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
                    anim.SetBool("Jump", true);     // Animatorにジャンプに切り替えるフラグを送る
                }
            }
        }

        Debug.Log(moveVelocity.magnitude);
        // 上下のキー入力でキャラクターを移動させる
        rb.MovePosition(transform.position+(moveVelocity * Time.fixedDeltaTime));

        

        // 以下、Animatorの各ステート中での処理
        // Locomotion中
        // 現在のベースレイヤーがlocoStateの時
        if (currentBaseState.shortNameHash == locoState)
        {
            //カーブでコライダ調整をしている時は、念のためにリセットする
            if (useCurves)
            {
                resetCollider();
            }
        }
        // JUMP中の処理
        // 現在のベースレイヤーがjumpStateの時
        else if (currentBaseState.shortNameHash == jumpState)
        {
            
                                                                    // ステートがトランジション中でない場合
            if (!anim.IsInTransition(0))
            {

                // 以下、カーブ調整をする場合の処理
                if (useCurves)
                {
                    // 以下JUMP00アニメーションについているカーブJumpHeightとGravityControl
                    // JumpHeight:JUMP00でのジャンプの高さ（0〜1）
                    // GravityControl:1⇒ジャンプ中（重力無効）、0⇒重力有効
                    float jumpHeight = anim.GetFloat("JumpHeight");
                    float gravityControl = anim.GetFloat("GravityControl");
                    if (gravityControl > 0)
                        rb.useGravity = false;  //ジャンプ中の重力の影響を切る

                    // レイキャストをキャラクターのセンターから落とす
                    Ray ray = new Ray(transform.position + Vector3.up, -Vector3.up);
                    RaycastHit hitInfo = new RaycastHit();
                    // 高さが useCurvesHeight 以上ある時のみ、コライダーの高さと中心をJUMP00アニメーションについているカーブで調整する
                    if (Physics.Raycast(ray, out hitInfo))
                    {
                        if (hitInfo.distance > useCurvesHeight)
                        {
                            col.height = orgColHight - jumpHeight;          // 調整されたコライダーの高さ
                            float adjCenterY = orgVectColCenter.y + jumpHeight;
                            col.center = new Vector3(0, adjCenterY, 0); // 調整されたコライダーのセンター
                        }
                        else
                        {
                            // 閾値よりも低い時には初期値に戻す（念のため）					
                            resetCollider();
                        }
                    }
                }
                // Jump bool値をリセットする（ループしないようにする）				
                anim.SetBool("Jump", false);
            }
        }

        // IDLE中の処理
        // 現在のベースレイヤーがidleStateの時
        else if (currentBaseState.shortNameHash == idleState)
        {
            //カーブでコライダ調整をしている時は、念のためにリセットする
            if (useCurves)
            {
                resetCollider();
            }
            // スペースキーを入力したらRest状態になる
            if (jumpFlag)
            {
                anim.SetBool("Rest", true);
            }
        }
        // REST中の処理
        // 現在のベースレイヤーがrestStateの時
        else if (currentBaseState.shortNameHash == restState)
        {
            //cameraObject.SendMessage("setCameraPositionFrontView");		// カメラを正面に切り替える
            // ステートが遷移中でない場合、Rest bool値をリセットする（ループしないようにする）
            if (!anim.IsInTransition(0))
            {
                anim.SetBool("Rest", false);
            }
        }
    }

    void Update()
    {
       
    }

    private void OnDisable()
    {
        if (TMInputMnager.IsInstance())
        {
            TMInputMnager.Instance.OnKeyDown -= OnKeyDown;
            TMInputMnager.Instance.OnKeyUP -= OnKeyUp;
            TMInputMnager.Instance.OnMoveMouse -= OnMoveMouse;
        }
    }

    private void OnKeyDown(List<KeyCode> keyCodes)
    {
        Debug.Log("OnKeyDown");
        // 以下、キャラクターの移動処理
        if (keyCodes.Contains(KeyCode.W))
        {
            forward = true; 		// 前
        }
        if (keyCodes.Contains(KeyCode.A))
        {
            left = true; 		// 左
        }
        if (keyCodes.Contains(KeyCode.S))
        {
            back = true; 		// 後
        }
        if (keyCodes.Contains(KeyCode.D))
        {
            right = true; 		// 右
        }
        if (keyCodes.Contains(KeyCode.Space))
        {
            jumpFlag = true;		                // ジャンプ
        }
    }

    private void OnKeyUp(List<KeyCode> keyCodes)
    {

        // 以下、キャラクターの移動処理
        if (keyCodes.Contains(KeyCode.W))
        {
            forward = false; 		// 前
        }
        if (keyCodes.Contains(KeyCode.A))
        {
            left = false; 		// 左
        }
        if (keyCodes.Contains(KeyCode.S))
        {
            back = false; 		// 後
        }
        if (keyCodes.Contains(KeyCode.D))
        {
            right = false; 		// 右
        }
        if (keyCodes.Contains(KeyCode.Space))
        {
            jumpFlag = false;		                // ジャンプ
        }
    }

    private void OnMoveMouse(Vector2 mouse)
    {
        this.mouse = mouse;
        // 入力でキャラクタをY軸で旋回させる
        transform.Rotate(0, mouse.x * rotateSpeed * Time.deltaTime, 0);
    }

    private Vector3 GetVelocity()
    {
        var velocity = new Vector3(0, 0, 0);
        // 以下、キャラクターの移動処理
        if (forward)
        {
            velocity += new Vector3(0, 0, 1);		// 前
        }
        if (left)
        {
            velocity += new Vector3(-1, 0, 0);		// 左
        }
        if (back)
        {
            velocity += new Vector3(0, 0, -1);		// 後
        }
        if (right)
        {
            velocity += new Vector3(1, 0, 0);		// 右
        }

        return velocity;
    }

    // キャラクターのコライダーサイズのリセット関数
    void resetCollider()
    {
        // コンポーネントのHeight、Centerの初期値を戻す
        col.height = orgColHight;
        col.center = orgVectColCenter;
    }

}
