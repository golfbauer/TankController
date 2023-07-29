using System;
using UnityEditor;
using UnityEngine;

namespace Controller.Scripts.Managers.PlayerCamera.CameraMovement.Controller
{
    public class ScopedCameraMovementController : CameraMovementController
    {
        [SerializeField] private float timeToZoom;

        private float _currentZoom;
        private float _targetZoom;
        private float _zoomSpeed;
        private bool _isZooming;

        public override void SetUpCameraController(GameObject mainCamera, CameraManager cameraManager)
        {
            MainCameraObject = mainCamera;
            MainCamera = MainCameraObject.GetComponent<Camera>();
            this.cameraManager = cameraManager;
        }
        
        private void Zoom()
        {
            _currentZoom = Mathf.Lerp(_currentZoom, _targetZoom, Time.deltaTime * _zoomSpeed);
            MainCamera.fieldOfView = _currentZoom;

            _isZooming = !(Math.Abs(_currentZoom - _targetZoom) < 0.1f);
        }

        public override void SetUpTransitionIn(CameraMovementController previousCameraMovementController)
        {
            yaw = previousCameraMovementController.yaw;
            pitch = previousCameraMovementController.pitch;
            
            _targetZoom = fieldOfView;
            _currentZoom = MainCamera.fieldOfView;
            _zoomSpeed = Mathf.Abs(_targetZoom - _currentZoom) / (timeToZoom + 0.1f);
            ShowUI(true);
        }

        public override void TransitionIn()
        {
            MainCameraObject.transform.position = transform.position;
            Zoom();
            ActiveCameraMovement();

            if (!_isZooming)
            {
                cameraManager.FinishTransitionIn();
                MainCamera.fieldOfView = fieldOfView;
            }
        }

        public override void SetUpTransitionOut(CameraMovementController nextCameraMovementController)
        {
        }

        public override void TransitionOut()
        {
            ShowUI(false);
            cameraManager.FinishTransitionOut();
        }
        
        public override void ActiveCameraMovement()
        {
            float mouseX = Input.GetAxis("Mouse X") * cameraSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * cameraSensitivity;

            yaw += mouseX;
            pitch = Mathf.Clamp(pitch - mouseY, minPitch, maxPitch);

            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0.0f);

            MainCameraObject.transform.position = transform.position;
            MainCameraObject.transform.rotation = rotation;
            
            MainCameraObject.transform.Translate(0, 0, cameraZOffset, Space.Self);
        }
        
        public override CameraType GetCameraType()
        {
            return CameraType.Scoped;
        }

        public override void EditorGUI()
        {
            timeToZoom = EditorGUILayout.FloatField("Time to zoom", timeToZoom);
        }
    }
}