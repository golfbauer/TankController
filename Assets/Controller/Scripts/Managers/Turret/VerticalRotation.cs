using System;
using UnityEngine;

namespace Controller.Scripts.Managers.Turret
{
    public class VerticalRotation : MonoBehaviour
    {
        [Tooltip("Draws Ray to show the guns vertical aiming direction")]
        public bool debug;
        public float maxRotationSpeed = 10f;
        public float minMovement = 0.2f;
        public float accelerationTime = 2f;
        public float decelerationTime = 2f;
        public float minAngle = -45f;
        public float maxAngle = 45f;
        public HorizontalRotation horizontalRotation;

        private float _currentRotationSpeed;
        private float _currentVerticalAngle;
        private float _currentRotationVelocity;
        private Vector3 _targetPoint;
        private Vector3 _directionToTarget;
        private float _targetAngle;

        private void Start()
        {
            if (horizontalRotation == null)
            {
                horizontalRotation = transform.parent.GetComponent<HorizontalRotation>();
                Debug.Log("Horizontal Rotation not set, please check if working correctly.");
            }
        }

        private void Update()
        {
            RotateVertically();
            DebugRotation();
        }

        private void RotateVertically()
        {
            _targetPoint = horizontalRotation.GetTargetPoint();
            _directionToTarget = _targetPoint - transform.position;

            // Calculate the angle between the gun's forward direction and the direction to the target.
            _targetAngle = AngleBetweenVectorAndPlane(_directionToTarget, Vector3.up);

            _targetAngle = Mathf.Clamp(_targetAngle, minAngle, maxAngle);
            
            if (Mathf.Abs(_targetAngle) < minMovement)
            {
                _currentRotationSpeed = 0;
                
                return;
            }

            float deceleration = maxRotationSpeed / decelerationTime;
            float decelerationAngle = 0.5f * _currentRotationSpeed * _currentRotationSpeed / deceleration;
            
            if (Mathf.Abs(_currentVerticalAngle - _targetAngle) > decelerationAngle)
            {
                float targetSpeed = maxRotationSpeed;
                _currentRotationSpeed = Mathf.SmoothDamp(_currentRotationSpeed, targetSpeed, ref _currentRotationVelocity, accelerationTime);
            }
            else
            {
                float targetSpeed = 0;
                _currentRotationSpeed = Mathf.SmoothDamp(_currentRotationSpeed, targetSpeed, ref _currentRotationVelocity, decelerationTime);
            }

            // Interpolate between the current angle and the target angle
            float newAngle = Mathf.Lerp(_currentVerticalAngle, -_targetAngle, _currentRotationSpeed * Time.deltaTime);

            // Update the current angle
            _currentVerticalAngle = newAngle;

            // Apply the new angle to the gun
            transform.rotation = Quaternion.Euler(newAngle, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }
        
        private float AngleBetweenVectorAndPlane(Vector3 vector, Vector3 planeNormal) 
        {
            // Project the vector onto the plane
            Vector3 projectedVector = Vector3.ProjectOnPlane(vector, planeNormal);

            // Get the angle between the vector and its projection (i.e., the angle between the vector and the plane)
            float dotProduct = Vector3.Dot(vector, projectedVector);
            float vectorMagnitude = vector.magnitude;
            float projectedVectorMagnitude = projectedVector.magnitude;

            // Make sure to avoid division by zero
            if (vectorMagnitude * projectedVectorMagnitude <= 0)
                return 0;

            float angle = Mathf.Acos(dotProduct / (vectorMagnitude * projectedVectorMagnitude));

            // Convert the angle from radians to degrees
            angle *= Mathf.Rad2Deg;

            // We want the angle to be positive if the y-coordinate of the vector is positive, and negative otherwise
            angle *= Mathf.Sign(vector.y);

            return angle;
        }

        private void DebugRotation()
        {
            if (!debug)
                return;
            
            Debug.DrawRay(transform.position, _directionToTarget, Color.blue);
            Debug.Log("Vertical Target Angle: " + _targetAngle);
        }
    }
}
