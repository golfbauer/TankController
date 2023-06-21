using Controller.Scripts.Editors.Utils;
using Controller.Scripts.Managers.PlayerCamera;
using Controller.Scripts.Managers.PlayerCamera.CameraUI;
using Controller.Scripts.Managers.PlayerCamera.CameraUI.UIElements;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Controller.Scripts.Editors.PlayerCameraUI
{
    [CustomEditor(typeof(CameraUIController))]
    [CanEditMultipleObjects]
    public class CameraUIControllerEditor : Editor
    {
        private SerializedProperty _canvas;
        private SerializedProperty _isActive;
        private SerializedProperty _uiElements;
        private SerializedProperty _uiElementsData;

        private CameraUIController _controller;
        private bool UpdateAll;
        private Transform Transform;

        private void OnEnable()
        {
            _canvas = serializedObject.FindProperty("canvas");
            _isActive = serializedObject.FindProperty("isActive");
            _uiElements = serializedObject.FindProperty("uiElements");
            _uiElementsData = serializedObject.FindProperty("uiElementsData");
            
            Transform = ((CameraUIController) target).gameObject.transform;
            _controller = (CameraUIController) target;
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update ();

            if (PrefabStageUtility.GetCurrentPrefabStage() != null)
            {
                GUIUtils.DenyAccessGUI(GeneralMessages.PrefabModeWarning);
                return;
            }

            SetUpGUI();

            if (GUI.changed || Event.current.commandName == "UndoRedoPerformed")
            {
                UpdateAll = true;
            }

            if (UpdateAll)
            {
                _controller.isActive = _isActive.boolValue;
                _controller.UpdateActiveUIElements();
                
                serializedObject.ApplyModifiedProperties();
                RefreshParentSelection(Transform.gameObject);
                EditorUtility.SetDirty(Transform.gameObject);
            }
        }

        private void SetUpGUI()
        {
            GUIUtils.PropFieldGUI(_isActive, "Show UI Elements");
            GUIUtils.PropFieldGUI(_canvas, "Canvas");
            ShowUIElementsGUI();
            CreateUIElementGUI();
            UpdateAll = GUIUtils.UpdateAllGUI();
        }
        
        private void ShowUIElementsGUI()
        {
            GUIUtils.HeaderGUI("UI Elements");
            for(int i=0; i < _uiElements.arraySize; i++)
            {
                SerializedProperty elementProperty = _uiElements.GetArrayElementAtIndex(i);
                if (elementProperty.objectReferenceValue == null)
                {
                    _uiElements.DeleteArrayElementAtIndex(i);
                    _uiElementsData.DeleteArrayElementAtIndex(i);
                    i--;
                    continue;
                }
                
                UIElement element = _uiElements.GetArrayElementAtIndex(i).objectReferenceValue as UIElement;
                UIElementGUI(element, i);
            }
        }

        private void UIElementGUI(UIElement element, int index)
        {
            GameObject uiElement = element.gameObject;
            bool foldout = EditorPrefs.GetBool("UIElementFoldout" + index, false);
            foldout = EditorGUILayout.Foldout(foldout, uiElement.name);
            EditorPrefs.SetBool("UIElementFoldout" + index, foldout);

            if (foldout)
            {
                EditorGUI.indentLevel++;
                
                UIElementType currentType = element.GetUIElementType();
                UIElementType newType = (UIElementType)EditorGUILayout.EnumPopup("Type", currentType);
                
                EditorGUILayout.ObjectField("Transform", uiElement.transform, typeof(Transform), true);
                
                element.DisplayGUI();

                EditorGUI.indentLevel--;
                
                if (newType != currentType)
                {
                    UIElement newUIElement = _controller.ChangeTypeOfUIElement(index, newType);
                    AttachNewUIElement(newUIElement);
                }
            }
        }
        
        private void CreateUIElementGUI()
        {
            if(GUILayout.Button("Create UI Element"))
            {
                UIElementType uiElementType = (UIElementType)EditorGUILayout.EnumPopup("UI Element Type", UIElementType.Basic);
                UIElement uiElement = _controller.AddNewUIElement(uiElementType);

                AttachNewUIElement(uiElement);
            }
        }

        private void AttachNewUIElement(UIElement uiElement)
        {
            _uiElements.arraySize++;
            _uiElementsData.arraySize++;

            SerializedProperty newElement = _uiElements.GetArrayElementAtIndex(_uiElements.arraySize - 1);
            newElement.objectReferenceValue = uiElement;

            SerializedProperty newData = _uiElementsData.GetArrayElementAtIndex(_uiElementsData.arraySize - 1);
            newData.objectReferenceValue = uiElement.Data;
        }

        private void RefreshParentSelection(GameObject parent)
        {
            Selection.activeGameObject = null;
            Selection.activeGameObject = parent;
        }
        
    }
}