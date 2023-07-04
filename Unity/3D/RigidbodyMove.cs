using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class PlayerMove : MonoBehaviour
{
    public float speed;
    public float camRotSpeed = 0.1f;
    public float jumpPower;
    public Rigidbody rb;
    public Animator anim;
    public PlayerCamera mainCam;
    public Transform camTarget;

    private Vector3 moveVector;
    private Vector3 dir;
    private StickControl leftStick;
    private const string MouseScrollInput = "Mouse ScrollWheel";
    private const string MouseXInput = "Mouse X";
    private const string MouseYInput = "Mouse Y";
    private const string animatorState = "State";

    private bool isJump;

    enum State
    {
        Idle = 0,
        Run = 1
    }

    private void Start()
    {
        mainCam.SetFollowTransform(camTarget, false);
        leftStick = Gamepad.current.leftStick;
        EnhancedTouchSupport.Enable();


    }

    private void Update()
    {
        RotateCamera();
        RotateCameraPC();
    }

    private void FixedUpdate()
    {
        
        Jump();

        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

        Vector3 pcDir = new Vector3(inputX, 0, inputY);
        // PC
        if (pcDir != Vector3.zero)
        {
            dir = mainCam.Camera.transform.rotation * pcDir;
        }
        // Mobile
        else if (leftStick.IsActuated() == true)
        {
            Vector3 joystickValue = leftStick.ReadValue();
            dir = new Vector3(joystickValue.x, 0, joystickValue.y);
            dir = mainCam.Camera.transform.rotation * dir;
        }
        else
        {
            PlayAnimation(State.Idle);
            return;
        }

        Rotate();
        Move();
        PlayAnimation(State.Run);
        
    }

    private void LateUpdate()
    {
        mainCam.UpdateWithInput(Time.deltaTime, 0, Vector3.zero);
    }

    private void Jump()
    {
        if (isJump && rb.velocity.y <= 0)
            isJump = false;

        if (Input.GetKeyDown(KeyCode.Space) && isJump == false)
        {
            isJump = true;
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
    }

    private void Move()
    {
        Vector3 curPos = rb.position;

        rb.MovePosition(curPos + (dir * speed * Time.fixedDeltaTime));
    }

    public bool isTemp;
    private void Rotate()
    {
        Vector3 dirVector = Vector3.ClampMagnitude(new Vector3(dir.x, 0f, dir.z), 1f);
        moveVector = dirVector.normalized;

        if (moveVector == Vector3.zero)
            return;

        Quaternion rotation = Quaternion.LookRotation(moveVector);
        rb.rotation = Quaternion.Slerp(rb.rotation, rotation, Time.deltaTime * 10f);
    }

    private void PlayAnimation(State value)
    {
        anim.SetInteger(animatorState, UnsafeUtility.EnumToInt(value));
    }

    private void RotateCamera()
    {
        int count = Touch.activeFingers.Count;
        if (count == 0)
            return;

        Touch touch = Touch.activeTouches[0];
        if (count == 1)
        {
            if (Gamepad.current.leftStick.IsActuated())
            {
                return;
            }
            else
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {

                    Vector2 touchVector = touch.delta * camRotSpeed;
                    Vector3 lookInputVector = new Vector3(touchVector.x, touchVector.y, 0f);
                    Vector3 originRotVector = mainCam.transform.eulerAngles;
                    float scrollInput = -Input.GetAxis(MouseScrollInput);
                    mainCam.UpdateWithInput(Time.deltaTime, scrollInput, lookInputVector);
                }
            }
        }
        else if (count == 2)
        {
            if (Gamepad.current.leftStick.IsActuated())
            {
                touch = Touch.activeTouches[1];
                Vector2 touchVector = touch.delta * camRotSpeed;
                Vector3 lookInputVector = new Vector3(touchVector.x, touchVector.y, 0f);
                Vector3 originRotVector = mainCam.transform.eulerAngles;
                float scrollInput = -Input.GetAxis(MouseScrollInput);
                mainCam.UpdateWithInput(Time.deltaTime, scrollInput, lookInputVector);
            }
            else
                return;

        }
    }

    private void RotateCameraPC()
    {
        if (Application.isMobilePlatform)
            return;

        if (Input.GetMouseButton(0))
        {
            if (!Gamepad.current.leftStick.IsActuated() && !EventSystem.current.IsPointerOverGameObject())
            {
                Vector3 lookInputVector = new Vector3(Input.GetAxisRaw(MouseXInput), Input.GetAxisRaw(MouseYInput), 0f);
                float scrollInput = -Input.GetAxis(MouseScrollInput);
                mainCam.UpdateWithInput(Time.deltaTime, scrollInput, lookInputVector);
            }

        }
    }
}
