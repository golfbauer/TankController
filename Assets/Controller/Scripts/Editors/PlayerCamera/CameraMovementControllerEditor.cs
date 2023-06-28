using System;
using Controller.Scripts.Editors.Utils;
using Controller.Scripts.Managers.PlayerCamera.CameraMovement;
using Controller.Scripts.Managers.PlayerCamera.CameraMovement.Controller;
using UnityEditor;

namespace Controller.Scripts.Editors.PlayerCamera
{
    [CustomEditor(typeof(CameraMovementController), true)]
    [CanEditMultipleObjects]
    public class CameraMovementControllerEditor : TankComponentEditor
    {
        private SerializedProperty _cameraSwitchKey;
        private SerializedProperty _cameraZOffset;
        private SerializedProperty _cameraSensitivity;
        private SerializedProperty _minPitch;
        private SerializedProperty _maxPitch;
        private SerializedProperty _fieldOfView;
        
        private CameraMovementController _controller;

        private void OnEnable()
        {
            _cameraSwitchKey = serializedObject.FindProperty("cameraSwitchKey");
            _cameraZOffset = serializedObject.FindProperty("cameraZOffset");
            _cameraSensitivity = serializedObject.FindProperty("cameraSensitivity");
            _minPitch = serializedObject.FindProperty("minPitch");
            _maxPitch = serializedObject.FindProperty("maxPitch");
            _fieldOfView = serializedObject.FindProperty("fieldOfView");
            
            transform = ((CameraMovementController) target).gameObject.transform;
            _controller = (CameraMovementController) target;
        }

        public override void SetUpGUI()
        {
            GUIUtils.HeaderGUI(CameraMessages.GeneralSettings);
            GUIUtils.PropFieldGUI(_cameraSwitchKey, CameraMessages.CameraSwitchKey);
            GUIUtils.PropFieldGUI(_cameraZOffset, CameraMessages.CameraZOffset);
            GUIUtils.PropFieldGUI(_cameraSensitivity, CameraMessages.CameraSensitivity);
            GUIUtils.PropFieldGUI(_minPitch, CameraMessages.MinPitch);
            GUIUtils.PropFieldGUI(_maxPitch, CameraMessages.MaxPitch);
            GUIUtils.SliderGUI(_fieldOfView, 1, 179, CameraMessages.FieldOfView);
            
            _controller.EditorGUI();
        }
    }
}