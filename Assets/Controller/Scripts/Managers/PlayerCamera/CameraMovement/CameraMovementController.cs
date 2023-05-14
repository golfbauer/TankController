using Controller.Scripts.Managers.PlayerCamera.CameraUI;
using UnityEngine;

namespace Controller.Scripts.Managers.PlayerCamera.CameraMovement
{
    public class CameraMovementController : MonoBehaviour
    {
        [SerializeField] protected KeyCode cameraSwitchKey;
        [SerializeField] protected float cameraZOffset;
        [SerializeField] protected float cameraSensitivity;
        [SerializeField] protected float minPitch;
        [SerializeField] protected float maxPitch;
        [SerializeField] protected float fieldOfView;

        public float Yaw => yaw;
        public float Pitch => pitch;
        
        protected GameObject mainCameraObject;
        protected Camera mainCamera;
        protected CameraManager cameraManager;
        protected CameraUIController cameraUIController;
        protected float yaw;
        protected float pitch;

        private void Awake()
        {
            if(cameraUIController == null)
            {
                cameraUIController = GetComponent<CameraUIController>();
            }
        }

        public bool CameraKeyIsPressed()
        {
            return Input.GetKeyDown(cameraSwitchKey);
        }
        
        public virtual void SetUpCameraController(GameObject mainCamera, CameraManager cameraManager)
        {
            mainCameraObject = mainCamera;
            this.mainCamera = mainCameraObject.GetComponent<Camera>();
            this.cameraManager = cameraManager;
        }
        
        public virtual void SetTransitionInConditions(CameraMovementController previousCameraMovementController)
        {
            yaw = previousCameraMovementController.Yaw;
            pitch = previousCameraMovementController.Pitch;
            mainCameraObject.transform.position = transform.position;
            mainCamera.fieldOfView = fieldOfView;
            ToggleUI(true);
        }
        
        public virtual void SetTransitionOutConditions(CameraMovementController nextCameraMovementController)
        {
            ToggleUI();
        }

        public virtual void TransitionIn()
        {
            cameraManager.FinishTransitionIn();
        }

        public virtual void TransitionOut()
        {
            cameraManager.FinishTransitionOut();
        }

        public virtual void ActiveCameraMovement()
        {
            float mouseX = Input.GetAxis("Mouse X") * cameraSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * cameraSensitivity;

            yaw += mouseX;
            pitch = Mathf.Clamp(pitch - mouseY, minPitch, maxPitch);

            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0.0f);

            mainCameraObject.transform.position = transform.position;
            mainCameraObject.transform.rotation = rotation;
            
            mainCameraObject.transform.Translate(0, 0, -cameraZOffset, Space.Self);
        }

        public virtual CameraType GetCameraType()
        {
            return CameraType.ThirdPerson;
        }

        public virtual void ToggleUI(bool activate = false)
        {
            if (cameraUIController == null)
                return;
            
            cameraUIController.ToggleAllUIElements(activate);
        }
    }
}