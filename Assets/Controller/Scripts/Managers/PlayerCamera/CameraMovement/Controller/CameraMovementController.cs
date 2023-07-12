using Controller.Scripts.Managers.PlayerCamera.CameraUI;
using UnityEngine;

namespace Controller.Scripts.Managers.PlayerCamera.CameraMovement.Controller
{
    /// <summary>
    /// I recommend taking a look at the ThirdPersonCameraMovementController.cs and the ScopedCameraMovementController.cs
    /// </summary>
    public class CameraMovementController : MonoBehaviour
    {
        public KeyCode cameraSwitchKey = KeyCode.None;
        public float cameraZOffset = 1;
        public float cameraSensitivity = 1;
        public float minPitch = -30;
        public float maxPitch = 30;
        public float fieldOfView = 60;

        protected GameObject MainCameraObject;
        protected Camera MainCamera;
        protected CameraManager cameraManager;
        protected CameraUIManager CameraUIManager;
        
        public float yaw;
        public float pitch;

        private void Awake()
        {
            if(CameraUIManager == null)
            {
                CameraUIManager = GetComponent<CameraUIManager>();
            }
        }

        public virtual bool CameraKeyIsPressed()
        {
            return Input.GetKeyDown(cameraSwitchKey);
        }

        /// <summary>
        /// Set up the controller, this is called once in the CameraManager on Start.
        /// </summary>
        /// <param name="mainCamera"></param>
        /// <param name="cameraManager"></param>
        public virtual void SetUpCameraController(GameObject mainCamera, CameraManager cameraManager) {}

        /// <summary>
        /// Can be used to set up parameters for the transition in.
        /// This is called once in the CameraManager when Controller is switched.
        /// </summary>
        /// <param name="previousCameraMovementController"></param>
        public virtual void SetUpTransitionIn(CameraMovementController previousCameraMovementController) {}

        /// <summary>
        /// Called in the update loop of the CameraManager when transitioning in.
        /// This function needs to call the FinishTransitionIn function of the CameraManager, to exit the loop.
        /// </summary>
        public virtual void TransitionIn() {}

        /// <summary>
        /// Can be used to set up parameters for the transition out.
        /// This is called once in the CameraManager when Controller is switched.
        /// </summary>
        /// <param name="nextCameraMovementController"></param>
        public virtual void SetUpTransitionOut(CameraMovementController nextCameraMovementController) {}

        /// <summary>
        /// Called in the update loop of the CameraManager when transitioning out.
        /// This function needs to call the FinishTransitionOut function of the CameraManager, to exit the loop.
        /// </summary>
        public virtual void TransitionOut() {}

        /// <summary>
        /// Called in the update loop of the CameraManager when the controller is active.
        /// </summary>
        public virtual void ActiveCameraMovement()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets camera type for the CameraManager.
        /// </summary>
        /// <returns></returns>
        public virtual CameraType GetCameraType()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Disables or enables all UI elements for this controller.
        /// </summary>
        /// <param name="activate"></param>
        public virtual void ShowUI(bool activate)
        {
            if (CameraUIManager == null)
                return;
            
            CameraUIManager.ToggleUIElements(activate);
        }

        public virtual void EditorGUI()
        {
            
        }
    }
}