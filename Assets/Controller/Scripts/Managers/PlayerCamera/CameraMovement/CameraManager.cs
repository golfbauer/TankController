using System;
using System.Collections.Generic;
using Controller.Scripts.Managers.PlayerCamera.CameraMovement.Controller;
using UnityEngine;

namespace Controller.Scripts.Managers.PlayerCamera.CameraMovement
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField]
        public List<CameraMovementController> cameraControllers = new();

        [SerializeField] public KeyCode cameraSwitchKey = KeyCode.C;

        private GameObject _camera;
        private CameraMovementController _activeCameraMovementController;
        private CameraMovementController _nextCameraMovementController;

        public bool TransitioningIn { get; set; }
        public bool TransitioningOut { get; set; }


        private void Awake()
        {
            if (cameraControllers.Count == 0)
            {
                Destroy(this);
                return;
            }

            SetUpCamera();
            SetUpCameraControllers();

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void SetUpCamera()
        {
            try
            {
                _camera = gameObject.transform.Find("Main Camera").gameObject;
            }
            catch (NullReferenceException)
            {
                _camera = new GameObject("Main Camera");
                _camera.AddComponent<Camera>();
                _camera.transform.SetParent(transform);
                if (cameraControllers.Count > 0)
                    _camera.transform.position =
                        cameraControllers[0].transform.position;
                else
                    _camera.transform.position = transform.position;
            }
        }

        private void SetUpCameraControllers()
        {
            ClearCameraControllers();
            foreach (CameraMovementController cameraPosition in
                     cameraControllers)
            {
                cameraPosition.SetUpCameraController(_camera, this);
            }

            _activeCameraMovementController = cameraControllers[0];
            _nextCameraMovementController = cameraControllers[0];
            _activeCameraMovementController.ShowUI(true);
        }

        private void ClearCameraControllers()
        {
            for (int i = 0; i < cameraControllers.Count; i++)
            {
                if (cameraControllers[i] == null)
                {
                    cameraControllers.RemoveAt(i);
                    i--;
                }
            }
        }

        private void Update()
        {
            if (TransitioningOut)
            {
                _activeCameraMovementController.TransitionOut();
                return;
            }

            if (TransitioningIn)
            {
                _nextCameraMovementController.TransitionIn();
                return;
            }

            _activeCameraMovementController = _nextCameraMovementController;
            _activeCameraMovementController.ActiveCameraMovement();
            CheckCameraControllers();
        }

        private void CheckCameraControllers()
        {
            if (Input.GetKeyDown(cameraSwitchKey))
            {
                int index =
                    cameraControllers.IndexOf(
                        _activeCameraMovementController);
                if (index == cameraControllers.Count - 1)
                    index = -1;

                SwitchToToNextCamera(cameraControllers[index + 1]);
                return;
            }

            foreach (CameraMovementController cameraController in
                     cameraControllers)
            {
                if (cameraController == _activeCameraMovementController)
                    continue;

                if (cameraController.CameraKeyIsPressed())
                {
                    SwitchToToNextCamera(cameraController);
                }
            }
        }

        private void SwitchToToNextCamera(
            CameraMovementController cameraMovementController)
        {
            _nextCameraMovementController = cameraMovementController;
            _activeCameraMovementController.SetUpTransitionOut(
                _nextCameraMovementController);
            _nextCameraMovementController.SetUpTransitionIn(
                _activeCameraMovementController);
            TransitioningOut = true;
        }
        
        public void FinishTransitionIn()
        {
            TransitioningOut = false;
            TransitioningIn = false;
        }

        public void FinishTransitionOut()
        {
            TransitioningOut = false;
            TransitioningIn = true;
        }
    }
}