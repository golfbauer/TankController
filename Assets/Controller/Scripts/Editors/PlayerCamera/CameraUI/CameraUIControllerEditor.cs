using Controller.Scripts.Editors.Utils;
using Controller.Scripts.Managers.PlayerCamera;
using Controller.Scripts.Managers.PlayerCamera.CameraUI;
using Controller.Scripts.Managers.PlayerCamera.CameraUI.Elements;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Controller.Scripts.Editors.PlayerCamera.CameraUI
{
    [CustomEditor(typeof(CameraUIManager))]
    [CanEditMultipleObjects]
    public class CameraUIControllerEditor : TankComponentEditor
    {
        private SerializedProperty _canvas;
        private SerializedProperty _isActive;
        private SerializedProperty _uiElements;
        private SerializedProperty _uiElementsData;

        private CameraUIManager _manager;

        private void OnEnable()
        {
            _canvas = serializedObject.FindProperty("canvas");
            _isActive = serializedObject.FindProperty("isActive");
            _uiElements = serializedObject.FindProperty("uiElements");
            _uiElementsData = serializedObject.FindProperty("uiElementsData");
            
            transform = ((CameraUIManager) target).gameObject.transform;
            _manager = (CameraUIManager) target;
        }

        public override void SetUpGUI()
        {
            GUIUtils.PropFieldGUI(_isActive, CameraMessages.ShowUIElements);
            GUIUtils.PropFieldGUI(_canvas, CameraMessages.Canvas);
            ShowUIElementsGUI();
            CreateUIElementGUI();
            
            UpdateAllGUI();
        }

        private void ShowUIElementsGUI()
        {
            GUIUtils.HeaderGUI(CameraMessages.UIElements);
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
                    _manager.ChangeTypeOfUIElement(index, newType);
                }
            }
        }
        
        private void CreateUIElementGUI()
        {
            if(GUILayout.Button("Create UI Element"))
            {
                UIElementType uiElementType = (UIElementType)EditorGUILayout.EnumPopup("UI Element Type", UIElementType.Basic);
                UIElement uiElement = _manager.AddNewUIElement(uiElementType);

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
        
        public override bool AllowAccess()
        {
            return PrefabStageUtility.GetCurrentPrefabStage() != null;
        }

        public override void DenyAccessMessage()
        {
            GUIUtils.DenyAccessGUI(GeneralMessages.PrefabModeWarning);
        }
        
        public override void BulkUpdateComponents()
        {
            _manager.isActive = _isActive.boolValue;
            _manager.UpdateUIElements();
        }
    }
}