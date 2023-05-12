using System;
using System.Security.Cryptography;
using Controller.Scripts.Managers.Movement;
using UnityEngine;

namespace Controller.Scripts.Managers.Wheels
{
    public class WheelManager: MonoBehaviour
    {
        public float leftTorque;
        public float rightTorque;
        
        public float leftDrag;
        public float rightDrag;
        
        public float maxSpeed;
        
        private MovementManager _movementManager;

        private void Awake()
        {
            _movementManager = transform.parent.GetComponent<MovementManager>();
            if (_movementManager == null)
            {
                Debug.LogError("MovementManager not found!");
            }
            
        }

        private void FixedUpdate()
        {
            leftTorque = _movementManager.leftTorque;
            rightTorque = _movementManager.rightTorque;
            leftDrag = _movementManager.angularDrag;
            rightDrag = _movementManager.angularDrag;
            maxSpeed = _movementManager.maxSpeed;
        }
    }
}