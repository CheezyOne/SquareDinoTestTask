using Mirror;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private CharacterController _controller;
    [SerializeField] private Animator _animator;
    [SerializeField] private Vector3 _cameraOffset = new Vector3(0f, 1.5f, -3f);
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _rotationSpeed = 180f; 
    [SerializeField] private float _gravity = -15f;

    private Camera _playerCamera;
    private float _verticalVelocity = 0f;
    private float _horizontalInput;
    private float _verticalInput;
    private bool _isWalking;
    private float _lastClientSendTime;
    private const float SEND_INTERVAL = 0.03f;
    private const float CONSTANT_GRAVITY = -2f;
    private const float MAX_FALL_SPEED = -20f;
    private const float MOVEMENT_THRESHOLD = 0.1f;

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        _playerCamera = Camera.main;

        if (_playerCamera != null)
        {
            _playerCamera.transform.SetParent(transform);
            _playerCamera.transform.localPosition = _cameraOffset;
            _playerCamera.transform.localRotation = Quaternion.identity;
        }

        CmdUpdateServerPosition(transform.position, transform.rotation, false);
    }

    [ClientCallback]
    private void Update()
    {
        if (!isOwned)
            return;

        GetInput();
        HandleGravity();
        MovePlayer();
        RotatePlayer();
        UpdateAnimations();

        if (Time.time - _lastClientSendTime > SEND_INTERVAL)
        {
            CmdUpdateServerPosition(transform.position, transform.rotation, _isWalking);
            _lastClientSendTime = Time.time;
        }
    }

    private void GetInput()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");
        _isWalking = Mathf.Abs(_verticalInput) > MOVEMENT_THRESHOLD || Mathf.Abs(_horizontalInput) > MOVEMENT_THRESHOLD;
    }

    private void MovePlayer()
    {
        if (!isOwned) 
            return;

        Vector3 movement = transform.forward * _verticalInput * _speed * Time.deltaTime;
        Vector3 verticalMovement = Vector3.up * _verticalVelocity * Time.deltaTime;
        _controller.Move(movement + verticalMovement);
    }

    private void RotatePlayer()
    {
        if (!isOwned) 
            return;

        if (Mathf.Abs(_horizontalInput) > MOVEMENT_THRESHOLD)
        {
            transform.Rotate(0, _horizontalInput * _rotationSpeed * Time.deltaTime, 0);
        }
    }

    private void HandleGravity()
    {
        if (_controller.isGrounded)
        {
            if (_verticalVelocity < 0)
                _verticalVelocity = CONSTANT_GRAVITY; 
        }
        else
        {
            _verticalVelocity += _gravity * Time.deltaTime;
            _verticalVelocity = Mathf.Max(_verticalVelocity, MAX_FALL_SPEED);
        }
    }

    private void UpdateAnimations()
    {
        if (_animator != null)
        {
            _animator.SetBool("IsWalking", _isWalking);
        }
    }

    [Command]
    private void CmdUpdateServerPosition(Vector3 position, Quaternion rotation, bool isWalking)
    {
        transform.position = position;
        transform.rotation = rotation;
    }

    private void OnDisable()
    {
        if (_playerCamera != null)
            _playerCamera.transform.SetParent(null);
    }
}