using UnityEngine;

namespace Controller.Scripts.Managers.Turret
{
    public class HorizontalRotation : MonoBehaviour
    {
        [Tooltip("Will draw rays, to show the directions the camera and turret are pointing at")]
        public bool debug;
        public float maxRotationSpeed = 10f;
        [Tooltip(
            "The turret might shake a little, when it has found its target. " +
            "Pump up this value to prevent that. It will lock the last bit of rotation."
            )]
        public float minMovement = 0.3f;
        public float accelerationTime = 2f;
        public float decelerationTime = 2f;
        public float maxLeftRotationAngle = 180f;
        public float maxRightRotationAngle = 180f;
        [Tooltip("Layers that should be ignored by the turret when aiming (Please add the tanks layers here!)")]
        public LayerMask layersToIgnore;
        
        public Camera mainCamera;
        public int maxCollisionDistance = 1000;
        public Vector3 cameraAimDirection = Vector3.zero;
        
        private float _currentRotationSpeed;
        private int _raycastLayers;
        private Vector3 _targetPoint;
        private Vector3 _directionToTarget;
        private float _targetAngle;

        private void Start()
        { 
            int allLayers = LayerMask.GetMask("Default");
            _raycastLayers = allLayers & ~layersToIgnore;
        }

        private void Update()
        {
            DrawRay();
            RotateHorizontally();
            DebugRotation();
        }

        private void DrawRay()
        {
            var mainCameraTransform = mainCamera.transform;
            Vector3 cameraDirection = cameraAimDirection == Vector3.zero ? mainCameraTransform.forward : cameraAimDirection;
            Ray cameraRay = new Ray(mainCameraTransform.position, cameraDirection);
            if (Physics.Raycast(cameraRay, out var hit, maxCollisionDistance, _raycastLayers))
            {
                _targetPoint = hit.point;
            }
            else
            {
                _targetPoint = cameraRay.GetPoint(maxCollisionDistance);
            }
        }

        private void RotateHorizontally()
        {
            _directionToTarget = _targetPoint - transform.position;
            _directionToTarget.y = 0; // Ignore vertical difference.
            _targetAngle = Vector3.SignedAngle(transform.forward, _directionToTarget, Vector3.up);
            
            _targetAngle = Mathf.Clamp(_targetAngle, -maxRightRotationAngle, maxLeftRotationAngle);
            
            // Stop Turret from shaking, by forcing the last rotation, when the target angle is very small.
            if (Mathf.Abs(_targetAngle) < minMovement)
            {
                _currentRotationSpeed = 0;
                
                // This will also rotate x and z axis, by about 0.01f, but Quaternion are hard to understand (or just annoying to use).
                Vector3 targetForward = _directionToTarget.normalized;
                float yRotation = Mathf.Atan2(targetForward.x, targetForward.z) * Mathf.Rad2Deg;
                float deltaYRotation = yRotation - transform.eulerAngles.y;
                transform.Rotate(Vector3.up, deltaYRotation, Space.World);

                return;
            }

            // Determine current rotation speed
            float deceleration = maxRotationSpeed / decelerationTime;
            float decelerationAngle = 0.5f * _currentRotationSpeed * _currentRotationSpeed / deceleration;
            if (Mathf.Abs(_targetAngle) > decelerationAngle)
            {
                _currentRotationSpeed = Mathf.Lerp(_currentRotationSpeed, maxRotationSpeed, Time.deltaTime / accelerationTime);
            }
            else
            {
                _currentRotationSpeed = Mathf.Lerp(_currentRotationSpeed, 0, Time.deltaTime / decelerationTime);
            }

            // Rotate towards the target angle at the current speed.
            float currentYRotation = transform.localEulerAngles.y;
            float angleToRotate = Mathf.Sign(_targetAngle) * _currentRotationSpeed * Time.deltaTime;
            float newYRotation = currentYRotation + angleToRotate;

            Vector3 currentRotation = transform.localEulerAngles;
            currentRotation.y = newYRotation;
            transform.localEulerAngles = currentRotation;
        }

        private void DebugRotation()
        {
            if (!debug)
                return;
            
            Vector3 cameraDir = _targetPoint - mainCamera.transform.position;
            Debug.DrawRay(transform.position, _directionToTarget, Color.green);
            Debug.DrawRay(mainCamera.transform.position, cameraDir, Color.red);
            
            Debug.Log("Horizontal Angle to Target: " + _targetAngle);
        }
        
        public Vector3 GetTargetPoint()
        {
            return _targetPoint;
        }
    }
}

