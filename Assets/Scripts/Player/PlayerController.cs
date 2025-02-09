using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float maxSpeed = 8f;
        [SerializeField] private float acceleration = 20f;
        [SerializeField] private float maxSprintTime = 4.5f;
        [SerializeField] private float rotationSpeed = 10f;

        private CharacterController _controller;
        private Transform _camera;
    
        // Inputs
        private PlayerInput _playerInput;
        private InputAction _move;
        private InputAction _sprint;
    
        private Vector2 _input;
        private Vector2 _direction;
        private Vector2 _velocity;

        private bool _isSprinting;
        private IEnumerator _sprintRechargeCoroutine;
        
        // Backup the original values for speed and sprint time
        private float _originalMoveSpeed;
        private float _originalMaxSprintTime;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            _originalMoveSpeed = moveSpeed;
            _originalMaxSprintTime = maxSprintTime;
            
            _controller = GetComponent<CharacterController>();
            _camera = GameObjectHelper.FindChildGameObjectByTag(
                gameObject, "PlayerCamera").transform;

            _isSprinting = false;

            transform.rotation = _camera.transform.rotation;

            //Setup input
            _playerInput = GameObject.FindGameObjectWithTag("InputManager").GetComponent<PlayerInput>();
            _playerInput.SwitchCurrentActionMap("Player");
            _move = _playerInput.actions["Move"];
            _sprint = _playerInput.actions["Sprint"];
        }

        // Update is called once per frame
        private void Update()
        {
            CheckGrounded();
            GetMovementInput();
            PlayerSprinting();
            
            //Update player object rotation so it matches the camera
            Quaternion cameraObjectRotation = Quaternion.Euler(0f, _camera.transform.eulerAngles.y, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, cameraObjectRotation, rotationSpeed * Time.deltaTime);
        
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

        private void PlayerSprinting()
        {
            Debug.Log("Is Sprinting: " + _isSprinting);
            
            if (_controller.isGrounded)
            {
                _sprint.performed += SprintPerformed;

                if (_isSprinting && (_direction.x > 0.0f || _direction.y > 0.0f))
                {
                    Debug.Log("Time Sprinting " + maxSprintTime);
                    Debug.Log("Move Speed " + moveSpeed);
                
                    moveSpeed += moveSpeed * acceleration * Time.deltaTime;
                    maxSprintTime -= Time.deltaTime;

                    if (moveSpeed >= maxSpeed)
                    {
                        moveSpeed = maxSpeed;
                    }

                    if (maxSprintTime <= 0.0f)
                    {
                        maxSprintTime = _originalMaxSprintTime;
                        //moveSpeed = _originalMoveSpeed;
                        moveSpeed -= moveSpeed * (acceleration * 2) * Time.deltaTime;

                        _sprintRechargeCoroutine = SprintRecharge(4f);
                        StopCoroutine(_sprintRechargeCoroutine);

                        _isSprinting = false;

                        /*if (moveSpeed <= 4f)
                        {
                            moveSpeed = 4f;
                            _isSprinting = false;
                        }*/
                    }
                }
                else
                {
                    if (moveSpeed > _originalMoveSpeed)
                    {
                        /*if (moveSpeed >= 4f)
                        {
                            moveSpeed -= moveSpeed * Time.deltaTime;

                            if (moveSpeed <= 4f)
                            {
                                moveSpeed = 4f;
                            }
                        }*/
                        
                        maxSprintTime = _originalMaxSprintTime;
                    
                        moveSpeed -= moveSpeed * acceleration * Time.deltaTime * 2;

                        if (moveSpeed <= _originalMoveSpeed)
                        {
                            moveSpeed = _originalMoveSpeed;
                        }
                    
                        _sprintRechargeCoroutine = SprintRecharge(4f);
                        StopCoroutine(_sprintRechargeCoroutine);
                    }
                }

                _sprint.canceled += SprintCanceled;
            }
        }

        private void SprintPerformed(InputAction.CallbackContext ctx)
        {
            _isSprinting = true;
        }
        
        private void SprintCanceled(InputAction.CallbackContext ctx)
        {
            _isSprinting = false;
        }

        private IEnumerator SprintRecharge(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
        }
    }
}
