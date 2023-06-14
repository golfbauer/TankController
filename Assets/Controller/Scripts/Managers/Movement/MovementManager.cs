using System;
using Controller.Scripts.Managers.PlayerInput;
using Controller.Scripts.Managers.PlayerInput.Movement;
using UnityEngine;

namespace Controller.Scripts.Managers.Movement
{
    public class MovementManager: MonoBehaviour
    {
        public TankSteeringMode steeringMode;

        public MovementInputType inputType;
        
        public bool allowPivotSteering;
        public float maxPivotSpeed;
        public float pivotTime;
        public AnimationCurve pivotCurve;
        
        public float torque;
        
        public float turningDrag;
        public float minTurningDrag;
        public float breakDrag;
        public float breakDecelerationRate;
        public float rollingDrag;
        
        public float maxForwardSpeed;
        public float maxReverseSpeed;

        public float accelerationTime;
        public AnimationCurve accelerationCurve;
        
        public float decelerationTime;
        public AnimationCurve decelerationCurve;
        
        
        private MovementInputManager _movementInputManager;
        private Rigidbody _rigidbody;
        private bool _breakTank;

        private float _actualSpeed;
        private float _leftTrackSpeed;
        private float _rightTrackSpeed;
        private float _leftTorque;
        private float _rightTorque;
        private float _leftDrag;
        private float _rightDrag;
        
        private bool _pivotTurn;

        private void Awake()
        {
            SetUpInputManager();
            _rigidbody = GetComponent<Rigidbody>();
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
            
            if (allowPivotSteering && _actualSpeed < 0.5f)
            {
                PivotTurn();
            }
            else
                _pivotTurn = false;

            if (!_pivotTurn)
            {
                CalculateVerticalMovement();
                CalculateHorizontalMovement();
                
                _leftTorque = torque;
                _rightTorque = torque;
            }
            
            _leftTorque = _leftTrackSpeed > 0 ? _leftTorque : -_leftTorque;
            _rightTorque = _rightTrackSpeed > 0 ? _rightTorque : -_rightTorque;
        }

        private void FixedUpdate()
        {
            _actualSpeed = _rigidbody.velocity.magnitude;
        }

        public void ToggleBreak()
        {
            _breakTank = !_breakTank;
        }

        public void ApplyBreak()
        {
            _leftTrackSpeed = Mathf.Max(0, _leftTrackSpeed - breakDecelerationRate * Time.deltaTime);
            _rightTrackSpeed = Mathf.Max(0, _rightTrackSpeed - breakDecelerationRate * Time.deltaTime);

            _leftTorque = 0;
            _rightTorque = 0;

            _leftDrag = breakDrag;
            _rightDrag = breakDrag;
        }

        private void PivotTurn()
        {
            float playerHorizontalInput = _movementInputManager.horizontalInput;
            
            if (playerHorizontalInput == 0)
            {
                _pivotTurn = false;
                return;
            }

            _leftTorque = torque;
            _rightTorque = torque;
            _leftDrag = minTurningDrag;
            _rightDrag = minTurningDrag;

            _leftTrackSpeed = Mathf.Lerp(
                _leftTrackSpeed,
                playerHorizontalInput * maxPivotSpeed,
                pivotCurve.Evaluate(Time.deltaTime / pivotTime)
            );

            _rightTrackSpeed = -_leftTrackSpeed;
            _pivotTurn = true;
        }


        public void CalculateVerticalMovement()
        {
            float playerVerticalInput = _movementInputManager.verticalInput;

            if (playerVerticalInput == 0)
            {
                _leftTrackSpeed *= (1 - rollingDrag * Time.deltaTime);
            }

            if (playerVerticalInput > 0)
            {
                _leftTrackSpeed = Mathf.Lerp(_leftTrackSpeed, maxForwardSpeed,
                    accelerationCurve.Evaluate(Time.deltaTime / accelerationTime));
            }
    
            if (playerVerticalInput < 0)
            {
                _leftTrackSpeed = Mathf.Lerp(_leftTrackSpeed, -maxReverseSpeed,
                    decelerationCurve.Evaluate(Time.deltaTime / decelerationTime));
            }
    
            _leftTrackSpeed = Mathf.Clamp(_leftTrackSpeed, -maxReverseSpeed, maxForwardSpeed);
            _rightTrackSpeed = _leftTrackSpeed;
        }

        public void CalculateHorizontalMovement()
        {
            float playerHorizontalInput = _movementInputManager.horizontalInput;
            
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
                _rightDrag = Mathf.Lerp(_rightDrag, turningDrag, Time.deltaTime * 100f);
            }
            
            // If the player is steering to the left, apply drag to the left.
            if (playerHorizontalInput < 0)
            {
                _leftDrag = Mathf.Lerp(_leftDrag, turningDrag, Time.deltaTime * 100f);
                _rightDrag = minTurningDrag;
            }

            _leftDrag = Mathf.Clamp(_leftDrag, minTurningDrag, turningDrag);
            _rightDrag = Mathf.Clamp(_rightDrag, minTurningDrag, turningDrag);
        }

        public float GetLeftTrackSpeed()
        {
            return _leftTrackSpeed;
        }

        public float GetRightTrackSpeed()
        {
            return _rightTrackSpeed;
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