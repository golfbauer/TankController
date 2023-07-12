using System;
using Controller.Scripts.Editors.Tank;
using Controller.Scripts.Editors.Utils;
using Controller.Scripts.Managers.PlayerCamera.CameraMovement;
using Controller.Scripts.Managers.PlayerCamera.CameraMovement.Controller;
using Controller.Scripts.Managers.PlayerCamera.CameraUI;
using UnityEditor;
using UnityEngine;
using CameraType = Controller.Scripts.Managers.PlayerCamera.CameraType;

namespace Controller.Scripts.Editors.PlayerCamera
{
    [CustomEditor(typeof(CameraManager), true)]
    [CanEditMultipleObjects]
    public class CameraManagerEditor : TankComponentEditor
    {
        private SerializedProperty _cameraSwitchKey;
        private SerializedProperty _cameraControllers;

        private void OnEnable()
        {
            _cameraSwitchKey = serializedObject.FindProperty("cameraSwitchKey");
            _cameraControllers = serializedObject.FindProperty("cameraControllers");
            
            transform = ((CameraManager) target).gameObject.transform;
        }

        public override void SetUpGUI()
        {
            GUIUtils.PropFieldGUI(_cameraSwitchKey, CameraMessages.CameraSwitchKey);
            GUIUtils.PropFieldGUI(_cameraControllers);
            
            GUIUtils.HeaderGUI(CameraMessages.CameraPositions);
            CameraPositionsGUI();
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginHorizontal();
            AddCameraPosition();
            GUIUtils.UpdateAllGUI();
            EditorGUILayout.EndHorizontal();
        }

        private void CameraPositionsGUI()
        {
        
            for(int i=0; i < _cameraControllers.arraySize; i++)
            {
                var cameraController = _cameraControllers.GetArrayElementAtIndex(i).objectReferenceValue as CameraMovementController;
                if (cameraController == null)
                    continue;
                CameraControllerGUI(cameraController, i);
            }
        }

        private void CameraControllerGUI(CameraMovementController movementController, int index)
        {
            GameObject cameraPosition = movementController.gameObject;
            bool foldout = EditorPrefs.GetBool("CameraPositionFoldout" + index, false);
            foldout = EditorGUILayout.Foldout(foldout, cameraPosition.name);
            EditorPrefs.SetBool("CameraPositionFoldout" + index, foldout);

            if (foldout)
            {
                EditorGUI.indentLevel++;
                CameraType currentType = movementController.GetCameraType();
                CameraType newType = (CameraType)EditorGUILayout.EnumPopup(CreateTankMessages.CameraType, currentType);
                if (newType != currentType)
                    ReplaceCameraPosition(cameraPosition, newType, index);
                EditorGUILayout.ObjectField(CreateTankMessages.Transform, cameraPosition.transform, typeof(Transform), true);
                RemoveCameraPosition(cameraPosition, index);
                EditorGUI.indentLevel--;
            }
        }
        
        private void AddCameraPosition()
        {
            if (GUILayout.Button(CameraMessages.AddCameraPosition))
            {
                GameObject cameraPosition = new GameObject("CameraPosition " + (_cameraControllers.arraySize + 1));
                cameraPosition.transform.SetParent(transform);
                cameraPosition.transform.localPosition = Vector3.zero;
                _cameraControllers.InsertArrayElementAtIndex(_cameraControllers.arraySize);
                _cameraControllers.GetArrayElementAtIndex(_cameraControllers.arraySize - 1).objectReferenceValue = 
                    GetCameraController(cameraPosition, CameraType.ThirdPerson);
                
                cameraPosition.AddComponent<CameraUIManager>();
            }
        }
        
        private void RemoveCameraPosition(GameObject cameraPosition, int i)
        {
            if (GUILayout.Button(GeneralMessages.Remove))
            {
                DestroyImmediate(cameraPosition);
                _cameraControllers.DeleteArrayElementAtIndex(i);
            }
        }
        
        private void ReplaceCameraPosition(GameObject cameraPosition, CameraType newType, int i)
        {
            DestroyImmediate(cameraPosition.GetComponent<CameraMovementController>());
            CameraMovementController newController = GetCameraController(cameraPosition, newType);
            _cameraControllers.GetArrayElementAtIndex(i).objectReferenceValue = newController;
        }
        
        private CameraMovementController GetCameraController(GameObject cameraPosition, CameraType cameraType)
        {
            CameraMovementController cameraMovementController;
            switch (cameraType)
            {
                case (CameraType.ThirdPerson):
                    cameraMovementController = cameraPosition.AddComponent<ThirdPersonCameraMovementController>();
                    break;
                case CameraType.Scoped:
                    cameraMovementController = cameraPosition.AddComponent<ScopedCameraMovementController>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cameraType), cameraType, null); 
            }
            
            return cameraMovementController;
        }
    }
}