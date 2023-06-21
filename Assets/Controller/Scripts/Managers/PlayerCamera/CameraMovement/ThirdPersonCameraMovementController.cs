using UnityEngine;

namespace Controller.Scripts.Managers.PlayerCamera.CameraMovement
{
    public class ThirdPersonCameraMovementController : CameraMovementController
    {
        public override void SetUpCameraController(GameObject mainCamera, CameraManager cameraManager)
        {
            MainCameraObject = mainCamera;
            MainCamera = MainCameraObject.GetComponent<Camera>();
            CameraManager = cameraManager;
        }

        public override void SetUpTransitionIn(CameraMovementController previousCameraMovementController)
        {
            yaw = previousCameraMovementController.yaw;
            pitch = previousCameraMovementController.pitch;
        }

        public override void TransitionIn()
        {
            MainCameraObject.transform.position = transform.position;
            MainCamera.fieldOfView = fieldOfView;
            ShowUI(true);
            CameraManager.FinishTransitionIn();
        }
        
        public override void SetUpTransitionOut(CameraMovementController nextCameraMovementController)
        {
            
        }
        
        public override void TransitionOut()
        {
            ShowUI(false);
            CameraManager.FinishTransitionOut();
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
            
            MainCameraObject.transform.Translate(0, 0, -cameraZOffset, Space.Self);
        }

        public override CameraType GetCameraType()
        {
            return CameraType.ThirdPerson;
        }
    }
}