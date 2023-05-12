using System;
using System.Collections.Generic;
using Controller.Scripts.Managers.PlayerCamera.CameraController;
using Controller.Scripts.Managers.PlayerCamera.UIController;
using JetBrains.Annotations;
using UnityEngine;

namespace Controller.Scripts.Managers.PlayerCamera
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private List<CameraController.CameraController> cameraControllers = new();

        private GameObject _camera;
        private CameraController.CameraController _activeCameraController;
        private CameraController.CameraController _switchToCameraController;
        private bool _isSwitchingCameras;

        public bool TransitioningIn { get; set; }
        public bool TransitioningOut { get; set; }


        private void Awake()
        {
            SetUpCamera();
            SetUpCameraControllers();

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        private void SetUpCamera()
        {
            _camera = new GameObject("Main Camera");
            _camera.AddComponent<Camera>();
            _camera.transform.SetParent(transform);

            _camera.transform.position = cameraControllers[0].transform.position;
        }
        
        private void SetUpCameraControllers()
        {
            ClearCameraControllers();
            foreach(CameraController.CameraController cameraPosition in cameraControllers)
            {
                cameraPosition.SetUpCameraController(_camera, this);
            }
            
            _activeCameraController = cameraControllers[0];
            _switchToCameraController = cameraControllers[0];
        }



        private void Update()
        {
            if (TransitioningOut)
            {
                _activeCameraController.TransitionOut();
                return;
            }
            
            if (TransitioningIn)
            {
                _switchToCameraController.TransitionIn();
                return;
            }

            if (_isSwitchingCameras)
            {
                _activeCameraController = _switchToCameraController;
                _isSwitchingCameras = false;
            }
            
            _activeCameraController.ActiveCameraMovement();
            
            CheckCameraControllers();
        }

        private void CheckCameraControllers()
        {
            foreach(CameraController.CameraController cameraController in cameraControllers)
            {
                if (cameraController == _activeCameraController)
                    continue;
                
                CheckCameraController(cameraController);
            }
        }

        private void CheckCameraController(CameraController.CameraController cameraController)
        {
            if (cameraController.CameraKeyIsPressed())
            {
                _isSwitchingCameras = true;
                _switchToCameraController = cameraController;
                _switchToCameraController.SetTransitionInConditions(_activeCameraController);
                _activeCameraController.SetTransitionOutConditions(_switchToCameraController);
                
                TransitioningOut = true;
            }
        }
        
        public void FinishTransitionIn()
        {
            TransitioningIn = false;
        }
        
        public void FinishTransitionOut()
        {
            TransitioningOut = false;
            TransitioningIn = true;
        }

        public void AddNewCameraPosition(Transform parent, CameraType cameraType)
        {
            ClearCameraControllers();
            GameObject cameraPosition = new GameObject("Camera Position " + (cameraControllers.Count + 1));
            cameraPosition.transform.SetParent(parent);
            AttachCameraController(cameraPosition, cameraType);
        }

        private void AttachCameraController(GameObject cameraPosition, CameraType cameraType)
        {
            cameraControllers.Add(GetCameraController(cameraPosition, cameraType));
            cameraPosition.AddComponent<CameraUIController>();
        }

        public void ReplaceCameraController(GameObject cameraPosition, CameraType cameraType)
        {
            int index = cameraControllers.IndexOf(cameraPosition.GetComponent<CameraController.CameraController>());
            
            DestroyImmediate(cameraPosition.GetComponent<CameraController.CameraController>());
            cameraControllers.RemoveAt(index);
            
            cameraControllers.Insert(index, GetCameraController(cameraPosition, cameraType));
            ClearCameraControllers();
        }

        private CameraController.CameraController GetCameraController(GameObject cameraPosition, CameraType cameraType)
        {
            CameraController.CameraController cameraController;
            switch (cameraType)
            {
                case (CameraType.ThirdPerson):
                    cameraController = cameraPosition.AddComponent<CameraController.CameraController>();
                    break;
                case CameraType.Scoped:
                    cameraController = cameraPosition.AddComponent<ScopedCameraController>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cameraType), cameraType, null); 
            }
            
            return cameraController;
        }
        
        private void ClearCameraControllers()
        {
            for (int i = 0; i < cameraControllers.Count; i++)
            {
                if (cameraControllers[i] == null)
                {
                    cameraControllers.RemoveAt(i);
                    i--;
                    continue;
                }
                
                cameraControllers[i].transform.name = "Camera Position " + (i + 1);
            }
        }

        [ItemCanBeNull]
        public List<CameraController.CameraController> GetCameraControllers()
        {
            return cameraControllers;
        }
    }
}