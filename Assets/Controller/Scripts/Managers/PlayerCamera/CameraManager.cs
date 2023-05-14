using System;
using System.Collections.Generic;
using Controller.Scripts.Managers.PlayerCamera.CameraMovement;
using Controller.Scripts.Managers.PlayerCamera.CameraUI;
using JetBrains.Annotations;
using UnityEngine;

namespace Controller.Scripts.Managers.PlayerCamera
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private List<CameraMovementController> cameraControllers = new();

        private GameObject _camera;
        private CameraMovementController _activeCameraMovementController;
        private CameraMovementController _switchToCameraMovementController;
        private bool _isSwitchingCameras;

        public bool TransitioningIn { get; set; }
        public bool TransitioningOut { get; set; }


        private void Awake()
        {
            SetUpCamera();
            SetUpCameraControllers();
            _activeCameraMovementController.ToggleUI(true);

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
                _camera.transform.position = cameraControllers[0].transform.position;
            }
        }
        
        private void SetUpCameraControllers()
        {
            ClearCameraControllers();
            foreach(CameraMovementController cameraPosition in cameraControllers)
            {
                cameraPosition.SetUpCameraController(_camera, this);
            }
            
            _activeCameraMovementController = cameraControllers[0];
            _switchToCameraMovementController = cameraControllers[0];
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
                _switchToCameraMovementController.TransitionIn();
                return;
            }

            if (_isSwitchingCameras)
            {
                _activeCameraMovementController = _switchToCameraMovementController;
                _isSwitchingCameras = false;
            }
            
            _activeCameraMovementController.ActiveCameraMovement();
            
            CheckCameraControllers();
        }

        private void CheckCameraControllers()
        {
            foreach(CameraMovementController cameraController in cameraControllers)
            {
                if (cameraController == _activeCameraMovementController)
                    continue;
                
                CheckCameraController(cameraController);
            }
        }

        private void CheckCameraController(CameraMovementController cameraMovementController)
        {
            if (cameraMovementController.CameraKeyIsPressed())
            {
                _isSwitchingCameras = true;
                _switchToCameraMovementController = cameraMovementController;
                _switchToCameraMovementController.SetTransitionInConditions(_activeCameraMovementController);
                _activeCameraMovementController.SetTransitionOutConditions(_switchToCameraMovementController);
                
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
            int index = cameraControllers.IndexOf(cameraPosition.GetComponent<CameraMovementController>());
            
            DestroyImmediate(cameraPosition.GetComponent<CameraMovementController>());
            cameraControllers.RemoveAt(index);
            
            cameraControllers.Insert(index, GetCameraController(cameraPosition, cameraType));
            ClearCameraControllers();
        }

        private CameraMovementController GetCameraController(GameObject cameraPosition, CameraType cameraType)
        {
            CameraMovementController cameraMovementController;
            switch (cameraType)
            {
                case (CameraType.ThirdPerson):
                    cameraMovementController = cameraPosition.AddComponent<CameraMovementController>();
                    break;
                case CameraType.Scoped:
                    cameraMovementController = cameraPosition.AddComponent<ScopedCameraMovementController>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cameraType), cameraType, null); 
            }
            
            return cameraMovementController;
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
        public List<CameraMovementController> GetCameraControllers()
        {
            return cameraControllers;
        }
    }
}