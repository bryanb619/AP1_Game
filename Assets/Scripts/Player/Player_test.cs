using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_test : MonoBehaviour
{
    //Acceleration Variables
    private const float FORWARD_ACCELERATION = 10.0f;
    private const float BACKWARD_ACCELERATION = 10.0f;
    private const float STRAFE_ACCELERATION = 10.0f;
    private const float GRAVITY_ACCELERATION = 10.0f;

    //Max Velocity Variables
    private float MAX_FORWARD_VELOCITY = 4.0f;
    private const float MAX_BACKWARD_VELOCITY = 2.0f;
    private const float MAX_STRAFE_VELOCITY = 3.0f;
    //private const float MAX_FALL_VELOCITY = 100.0f;
    //private const float MAX_JUMP_VELOCITY = 200.0f;

    //Rotation Variables
    private const float ROTATION_VELOCITY_FACTOR = 2.0f;
    private const float MIN_TILT_ROTATION = 70.0f;
    private const float MAX_TILT_ROTATION = 290.0f;

    private Camera _camera;
    private CharacterController _controller;
    private Transform _cameraTransform;
    private Vector3 _acceleration;
    private Vector3 _velocity;
    private Vector3 _originalplayercenter;

    [Header("Crouch Settings")]
    [SerializeField]
    private float standingHeight = 1.8f;

    [SerializeField]
    private float crouchingHeight = 1.50f;

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _cameraTransform = GetComponentInChildren<Camera>().transform;
        _acceleration = Vector3.zero;
        _velocity = Vector3.zero;
        _originalplayercenter = _controller.center;
        HideCursor();
    }

    private void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        UpdateRotation();
        UpdateTilt();
    }

    private void UpdateRotation()
    {
        float rotation = Input.GetAxis("Mouse X") * ROTATION_VELOCITY_FACTOR;

        transform.Rotate(0f, rotation, 0f);
    }

    private void UpdateTilt()
    {
        Vector3 cameraRotation = _cameraTransform.localEulerAngles;

        cameraRotation.x -= Input.GetAxis("Mouse Y") * ROTATION_VELOCITY_FACTOR;

        if (cameraRotation.x < 180f)
            cameraRotation.x = Mathf.Min(cameraRotation.x, MIN_TILT_ROTATION);
        else
            cameraRotation.x = Mathf.Max(cameraRotation.x, MAX_TILT_ROTATION);

        _cameraTransform.localEulerAngles = cameraRotation;
    }

    void FixedUpdate()
    {
        UpdateAcceleration();
        UpdateVelocity();
        UpdatePosition();
        CheckForCrouch();
    }

    private void CheckForCrouch()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            _controller.height = Mathf.Lerp(_controller.height, crouchingHeight, Time.deltaTime * 3);
            _controller.center = Vector3.down * (standingHeight - _controller.height) / 2.0f;
        }
        else
        {
            _controller.height = Mathf.Lerp(_controller.height, standingHeight, Time.deltaTime * 3);
            _controller.center = Vector3.down * (standingHeight - _controller.height) / 2.0f;
        }
    }

            private void UpdateAcceleration()
    {
        _acceleration.z = Input.GetAxis("Vertical");
        _acceleration.z *= (_acceleration.z > 0f) ? FORWARD_ACCELERATION : BACKWARD_ACCELERATION;

        _acceleration.x = Input.GetAxis("Horizontal") * STRAFE_ACCELERATION;
        
        _acceleration.y = -GRAVITY_ACCELERATION;
    }

    private void UpdateVelocity()
    {
        _velocity += _acceleration * Time.fixedDeltaTime;

        _velocity.z = (_acceleration.z == 0f || _acceleration.z * _velocity.z < 0f) ?
            0f : Mathf.Clamp(_velocity.z, -MAX_BACKWARD_VELOCITY, MAX_FORWARD_VELOCITY);

        _velocity.x = (_acceleration.x == 0f || _acceleration.x * _velocity.x < 0f) ?
            0f : Mathf.Clamp(_velocity.x, -MAX_STRAFE_VELOCITY, MAX_STRAFE_VELOCITY);

       /* _velocity.y = (_acceleration.y == 0f) ?
            -0.1f : Mathf.Clamp(_velocity.y, -MAX_FALL_VELOCITY, MAX_JUMP_VELOCITY);
        */
    }

    private void UpdatePosition()
    {
        Vector3 motion = _velocity * Time.fixedDeltaTime;

        _controller.Move(transform.TransformVector(motion));
    }
}