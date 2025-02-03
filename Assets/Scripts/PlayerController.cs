using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Quaternion = System.Numerics.Quaternion;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float rotationSpeed = 10f;

    private CharacterController _controller;
    private Transform _camera;
    
    // Inputs
    private PlayerInput _playerInput;
    private InputAction _move;
    
    private Vector2 _input;
    private Vector2 _lookInput;
    private Vector2 _direction;
    private Vector3 _moveDirection;
    private Vector2 _velocity;

    private Vector3 _currentPosition;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _camera = GameObject.FindGameObjectWithTag("MainCamera").transform;

        transform.rotation = _camera.transform.rotation;

        //Setup input
        _playerInput = GameObject.FindGameObjectWithTag("InputManager").GetComponent<PlayerInput>();
        _playerInput.SwitchCurrentActionMap("Player");
        _move = _playerInput.actions["Move"];
    }

    // Update is called once per frame
    private void Update()
    {
        CheckGrounded();
        GetMovementInput();
        
        //Update player object rotation so it matches the camera
        UnityEngine.Quaternion cameraObjectRotation = UnityEngine.Quaternion.Euler(0f, _camera.transform.eulerAngles.y, 0f);
        transform.rotation = UnityEngine.Quaternion.Lerp(transform.rotation, cameraObjectRotation, rotationSpeed * Time.deltaTime);
        
        _controller.Move(transform.rotation * new Vector3(_direction.x, 0f, _direction.y) * (Time.deltaTime * moveSpeed));
    }

    private void CheckGrounded()
    {
        Debug.Log("Is Grounded: " + _controller.isGrounded);
        if (_controller.isGrounded)
            _velocity.y = 0f;
        else
            _velocity.y += gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }

    private void GetMovementInput()
    {
        _input = _move.ReadValue<Vector2>();
        Debug.Log("_input value: " + _input);
        _direction = new Vector3(_input.x, _input.y);
    }
}
