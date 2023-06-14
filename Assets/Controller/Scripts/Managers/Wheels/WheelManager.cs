using System;
using System.Security.Cryptography;
using Controller.Scripts.Managers.Movement;
using UnityEngine;

namespace Controller.Scripts.Managers.Wheels
{
    public class WheelManager: MonoBehaviour
    {
        public float wheelRadius;
        
        public float leftTorque;
        public float rightTorque;
        
        public float leftDrag;
        public float rightDrag;
        
        public float leftTargetSpeed;
        public float rightTargetSpeed;
        
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
            leftTargetSpeed = _movementManager.GetLeftTrackSpeed() / (2.0f * wheelRadius * Mathf.PI) * 360.0f * Mathf.Deg2Rad;
            rightTargetSpeed = _movementManager.GetRightTrackSpeed() / (2.0f * wheelRadius * Mathf.PI) * 360.0f * Mathf.Deg2Rad;
            leftTorque = _movementManager.GetLeftTorque();
            rightTorque = _movementManager.GetRightTorque();
            leftDrag = _movementManager.GetLeftDrag();
            rightDrag = _movementManager.GetRightDrag();
        }
    }
}