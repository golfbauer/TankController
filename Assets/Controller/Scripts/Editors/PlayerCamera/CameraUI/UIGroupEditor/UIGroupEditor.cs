using System;
using Controller.Scripts.Editors.Utils;
using Controller.Scripts.Managers.PlayerCamera;
using Controller.Scripts.Managers.PlayerCamera.CameraUI.UIElements;
using Controller.Scripts.Managers.PlayerCamera.CameraUI.UIElements.AmmoElements;
using Controller.Scripts.Managers.PlayerCamera.CameraUI.UIElements.BasicElements;
using Controller.Scripts.Managers.PlayerCamera.CameraUI.UIGroups;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Controller.Scripts.Editors.PlayerCamera.CameraUI.UIGroupEditor
{
    [CustomEditor(typeof(UIGroup), true)]
    [CanEditMultipleObjects]
    public class UIGroupEditor : TankComponentEditor
    {
        private SerializedProperty _uiElements;

        protected virtual void OnEnable()
        {
            _uiElements = serializedObject.FindProperty("uiElements");
            transform = ((UIGroup)target).gameObject.transform;
        }
        
        public override void SetUpGUI()
        {
            GUIUtils.PropFieldGUI(_uiElements);
            ShowUIElementsGUI();
            GUIUtils.Space(2);
            CreateUIElementGUI();
            UpdateAllGUI();
        }
        
        private void ShowUIElementsGUI()
        {
            GUIUtils.HeaderGUI(CameraMessages.UIElements);
            for (int i = 0; i < _uiElements.arraySize; i++)
            {
                SerializedProperty elementProperty = _uiElements.GetArrayElementAtIndex(i);
                if (elementProperty.objectReferenceValue == null)
                {
                    _uiElements.DeleteArrayElementAtIndex(i);
                    i--;
                    continue;
                }

                UIElement element = _uiElements.GetArrayElementAtIndex(i).objectReferenceValue as UIElement;
                UIElementGUI(element, i);
            }
        }

        private void UIElementGUI(UIElement element, int index)
        {
            GameObject elementGameObject = element.gameObject;
            bool foldout = EditorPrefs.GetBool("UIElementFoldout" + index, false);
            foldout = EditorGUILayout.Foldout(foldout, elementGameObject.name);
            EditorPrefs.SetBool("UIElementFoldout" + index, foldout);

            if (foldout)
            {
                EditorGUI.indentLevel++;
                UIElementType elementType = (UIElementType)EditorGUILayout.EnumPopup(GeneralMessages.Type, element.type);
                EditorGUILayout.ObjectField(GeneralMessages.Transform, elementGameObject.transform, typeof(Transform), true);
                element.DisplayGUI();
                RemoveUIElementGUI(element);
                EditorGUI.indentLevel--;
                
                if (elementType != element.type)
                {
                    ReplaceUIElement(index, elementType, element);
                }
            }
        }
        
        private void CreateUIElementGUI()
        {
            if (GUILayout.Button(GeneralMessages.Add))
            {
                UIElement uiElement = CreateUIElement(UIElementType.Basic);
                _uiElements.arraySize++;
                _uiElements.GetArrayElementAtIndex(_uiElements.arraySize - 1).objectReferenceValue = uiElement;
            }
        }

        private UIElement CreateUIElement(UIElementType type)
        {
            GameObject uiGameObject = new GameObject("UIElement");
            RectTransform rectTransform = uiGameObject.AddComponent<RectTransform>();

            uiGameObject.transform.SetParent(transform, false);
            rectTransform.anchoredPosition = new Vector2(0, 0);
            rectTransform.sizeDelta = new Vector2(100, 100);

            UIElement uiElement = SelectUIElement(type, uiGameObject);

            return uiElement;
        }

        protected virtual UIElement SelectUIElement(UIElementType type, GameObject uiGameObject)
        {
            UIElement uiElement;

            switch (type)
            {
                case UIElementType.Basic:
                    uiElement = uiGameObject.AddComponent<BasicUIElement>();
                    break;
                case UIElementType.Image:
                    uiElement = uiGameObject.AddComponent<ImageUIElement>();
                    break;
                case UIElementType.Text:
                    uiElement = uiGameObject.AddComponent<TextUIElement>();
                    break;
                // Add more cases for additional UIElement types
                default:
                    throw new ArgumentOutOfRangeException();
            }

            uiElement.InitializeUIElement();
            return uiElement;
        }

        private void ReplaceUIElement(int index, UIElementType type, UIElement uiElement)
        {
            DestroyImmediate(uiElement.gameObject);
            
            UIElement newUIElement = CreateUIElement(type);
            _uiElements.GetArrayElementAtIndex(index).objectReferenceValue = newUIElement;
        }
        
        private void RemoveUIElementGUI(UIElement element)
        {
            if (GUILayout.Button(GeneralMessages.Remove))
            {
                GameObject elementGameObject = element.gameObject;
                DestroyImmediate(elementGameObject);
            }
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
            for(int i = 0; i < _uiElements.arraySize; i++)
            {
                SerializedProperty elementProperty = _uiElements.GetArrayElementAtIndex(i);
                if (elementProperty.objectReferenceValue == null)
                {
                    _uiElements.DeleteArrayElementAtIndex(i);
                    i--;
                }
            }
        }
    }
}