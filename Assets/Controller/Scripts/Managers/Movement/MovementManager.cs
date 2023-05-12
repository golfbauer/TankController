using System;
using System.Collections.Generic;
using Controller.Scripts.Managers.PlayerInput;
using Controller.Scripts.Managers.PlayerInput.Movement;
using Controller.Scripts.Managers.Wheels;
using UnityEngine;

namespace Controller.Scripts.Managers.Movement
{
    public class MovementManager: MonoBehaviour
    {
        public MovementInputType inputType;
        public float maxSpeed;
        public float torque;
        public float angularDrag;
        
        
        public bool breakTank;
        public float verticalInput;
        public float horizontalInput;
        
        
        public float leftDrag;
        public float rightDrag;

        public float leftTorque;
        public float rightTorque;
        
        private MovementInputManager _movementInputManager;

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
            UpdateMovement();
        }

        private void UpdateMovement()
        {
            leftTorque = verticalInput * torque;
            rightTorque = verticalInput * torque;
        }
    }
}