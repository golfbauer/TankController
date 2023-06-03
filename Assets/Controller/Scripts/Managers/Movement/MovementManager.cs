using System;
using System.Collections.Generic;
using Controller.Scripts.Managers.PlayerInput;
using Controller.Scripts.Managers.PlayerInput.Movement;
using Controller.Scripts.Managers.Wheels;
using Unity.VisualScripting;
using UnityEngine;

namespace Controller.Scripts.Managers.Movement
{
    public class MovementManager: MonoBehaviour
    {
        public TankSteeringMode steeringMode;

        public MovementInputType inputType;
        public bool allowPivotSteering;
        
        public float torque;
        
        public float turningDrag;
        public float minTurningDrag;
        public float breakDrag;
        public float rollingDrag;
        
        public float maxForwardSpeed;
        public float maxReverseSpeed;

        public float accelerationTime;
        public AnimationCurve accelerationCurve;
        
        public float decelerationTime;
        public AnimationCurve decelerationCurve;
        
        
        private MovementInputManager _movementInputManager;
        private bool _breakTank;
        
        private float _currentSpeed;
        private float _leftTorque;
        private float _rightTorque;
        private float _leftDrag;
        private float _rightDrag;

        private void Awake()
        {
            SetUpInputManager();
        }

        private void SetUpInputManager()
        {
            if (_movementInputManager != null)
                return;

            switch (inputType)
            {
                case MovementInputType.Keyboard:
                    _movementInputManager = gameObject.AddComponent<KeyboardInputMovementManager>();
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            _movementInputManager.MovementManager = this;
        }

        private void Update()
        {
            _movementInputManager.MovementInput();

            if (_breakTank)
            {
                ApplyBreak();
                return;
            }

            if (allowPivotSteering && _movementInputManager.verticalInput == 0)
            {
                PivotTurn();
                return;
            }

            CalculateVerticalMovement();
            
            float playerHorizontalInput = _movementInputManager.horizontalInput;

            if (steeringMode == TankSteeringMode.TrackAcceleration)
            {
                CalculateHorizontalAcceleration(playerHorizontalInput);

                if (playerHorizontalInput != 0)
                {
                    _leftDrag = minTurningDrag;
                    _rightDrag = minTurningDrag;
                }
                else
                {
                    _leftDrag = 0;
                    _rightDrag = 0;
                }
                
            }


            if (steeringMode == TankSteeringMode.TrackDeceleration)
            {
                CalculateHorizontalDeceleration(playerHorizontalInput);
                
                _leftTorque = torque;
                _rightTorque = torque;
            }
            

            if (steeringMode == TankSteeringMode.TrackAccelerationAndDeceleration)
            {
                CalculateHorizontalAcceleration(playerHorizontalInput);
                CalculateHorizontalDeceleration(playerHorizontalInput);
            }
            
            Debug.Log(_currentSpeed);
        }
        
        public void ToggleBreak()
        {
            _breakTank = !_breakTank;
        }

        public void ApplyBreak()
        {
            float decelerationRate = breakDrag;
            _currentSpeed = Mathf.Max(0, _currentSpeed - decelerationRate * Time.deltaTime);

            _leftTorque = 0;
            _rightTorque = 0;

            _leftDrag = breakDrag;
            _rightDrag = breakDrag;
        }

        public void PivotTurn()
        {
            float playerHorizontalInput = _movementInputManager.horizontalInput;

            if(playerHorizontalInput > 0)
            {
                // turn right
                _leftTorque = torque;
                _rightTorque = -torque;
            }
            else if (playerHorizontalInput < 0)
            {
                // turn left
                _leftTorque = -torque;
                _rightTorque = torque;
            }
            else
            {
                // no input, stop turning
                _leftTorque = 0;
                _rightTorque = 0;
            }
        }

        public void CalculateVerticalMovement()
        {
            float playerVerticalInput = _movementInputManager.verticalInput;

            if (playerVerticalInput == 0)
            {
                _currentSpeed *= (1 - rollingDrag * Time.deltaTime);
            }

            if (playerVerticalInput > 0)
            {
                _currentSpeed = Mathf.Lerp(_currentSpeed, maxForwardSpeed,
                    accelerationCurve.Evaluate(Time.deltaTime / accelerationTime));
            }
            
            if (playerVerticalInput < 0)
            {
                _currentSpeed = Mathf.Lerp(_currentSpeed, -Mathf.Abs(maxReverseSpeed),
                    decelerationCurve.Evaluate(Time.deltaTime / decelerationTime));
            }
            
            _currentSpeed = Mathf.Clamp(_currentSpeed, -Mathf.Abs(maxReverseSpeed), maxForwardSpeed);
        }

        public void CalculateHorizontalAcceleration(float playerHorizontalInput)
        {
            
            // If the player is not pressing the horizontal input, then apply the default torque to both sides.
            if (playerHorizontalInput == 0)
            {
                _leftTorque = torque;
                _rightTorque = torque;
            }

            // Adjust torque to turn the tank right.
            if(playerHorizontalInput > 0)
            {
                _leftTorque = torque;
                _rightTorque = Mathf.Lerp(_rightTorque, 0, Time.deltaTime);
            }

            // Adjust torque to turn the tank left.
            if (playerHorizontalInput < 0)
            {
                _rightTorque = torque;
                _leftTorque = Mathf.Lerp(_leftTorque, 0, Time.deltaTime);
            }
    
            _leftTorque = Mathf.Clamp(_leftTorque, 0, torque);
            _rightTorque = Mathf.Clamp(_rightTorque, 0, torque);
        }

        public void CalculateHorizontalDeceleration(float playerHorizontalInput)
        {
            // If player is not pressing the horizontal input, then no drag at all.
            if (playerHorizontalInput == 0)
            {
                _leftDrag = 0;
                _rightDrag = 0;
                return;
            }
            
            // If player is steering to the right, apply drag to the right.
            if (playerHorizontalInput > 0)
            {
                _leftDrag = minTurningDrag;
                _rightDrag = Mathf.Lerp(_rightDrag, turningDrag, Time.deltaTime);
            }
            
            // If the player is steering to the left, apply drag to the left.
            if (playerHorizontalInput < 0)
            {
                _leftDrag = Mathf.Lerp(_leftDrag, turningDrag, Time.deltaTime);
                _rightDrag = minTurningDrag;
            }

            _leftDrag = Mathf.Clamp(_leftDrag, minTurningDrag, turningDrag);
            _rightDrag = Mathf.Clamp(_rightDrag, minTurningDrag, turningDrag);
        }

        public float GetCurrentSpeed()
        {
            return _currentSpeed;
        }
        
        public float GetLeftTorque()
        {
            return _leftTorque;
        }
        
        public float GetRightTorque()
        {
            return _rightTorque;
        }
        
        public float GetLeftDrag()
        {
            return _leftDrag;
        }
        
        public float GetRightDrag()
        {
            return _rightDrag;
        }
    }
}