using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Rigidbody))]
public class Frandle : MonoBehaviour
{

    [SerializeField] private float Speed = 0.04f, DashSpeed = 0.1f;
    private float fSpeed;
    private Vector3 Position;
    private Rigidbody2D rigidbody2d;
    private float targetAngle;
    public bool GoalFlag;

    public float GetSpeed { get { return Speed; } }
    public float GetDashSpeed { get { return DashSpeed; } }
    public float GetfSpeed { get { return fSpeed; } }
    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        
    }

    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
       
    }
}
