using System;
using UnityEngine;

namespace Controller.Scripts.Managers.PlayerCamera.CameraController
{
    public class ScopedCameraController : CameraController
    {
        [SerializeField] private float timeToZoom;

        private float _currentZoom;
        private float _targetZoom;
        private float _zoomSpeed;
        private bool _isZooming;

        private void Zoom()
        {
            _currentZoom = Mathf.Lerp(_currentZoom, _targetZoom, Time.deltaTime * _zoomSpeed);
            mainCamera.fieldOfView = _currentZoom;

            _isZooming = !(Math.Abs(_currentZoom - _targetZoom) < 0.1f);
        }

        public override void ActiveCameraMovement()
        {
            base.ActiveCameraMovement();
            mainCamera.fieldOfView = fieldOfView;
            
        }

        public override void TransitionIn()
        {
            Zoom();
            
            if(!_isZooming)
                cameraManager.FinishTransitionIn();
        }

        public override void TransitionOut()
        {
            cameraManager.FinishTransitionOut();
        }
        
        public override void SetTransitionInConditions(CameraController previousCameraController)
        {
            yaw = previousCameraController.Yaw;
            pitch = previousCameraController.Pitch;
            mainCameraObject.transform.position = transform.position;
            _targetZoom = fieldOfView;
            _currentZoom = mainCamera.fieldOfView;
            _zoomSpeed = Mathf.Abs(_targetZoom - mainCamera.fieldOfView) / timeToZoom;
        }

        public override void SetTransitionOutConditions(CameraController nextCameraController)
        {
        }

        public override CameraType GetCameraType()
        {
            return CameraType.Scoped;
        }
    }
}