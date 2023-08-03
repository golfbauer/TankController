using System;
using Controller.Scripts.Editors.Utils;
using Controller.Scripts.Managers.PlayerCamera;
using Controller.Scripts.Managers.PlayerCamera.CameraUI;
using Controller.Scripts.Managers.PlayerCamera.CameraUI.ElementData;
using Controller.Scripts.Managers.PlayerCamera.CameraUI.Elements;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Controller.Scripts.Editors.PlayerCamera.CameraUI
{
    [CustomEditor(typeof(CameraUIManager))]
    [CanEditMultipleObjects]
    public class CameraUIManagerEditor : TankComponentEditor
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

            transform = ((CameraUIManager)target).gameObject.transform;
            _manager = (CameraUIManager)target;
        }

        public override void SetUpGUI()
        {
            GUIUtils.PropFieldGUI(_isActive, CameraMessages.ShowUIElements);
            GUIUtils.PropFieldGUI(_canvas, CameraMessages.Canvas);
            GUIUtils.PropFieldGUI(_uiElements);
            GUIUtils.PropFieldGUI(_uiElementsData);
            ShowUIElementsGUI();
            CreateUIElementGUI();
            UpdateAllGUI();
        }

        private void ShowUIElementsGUI()
        {
            GUIUtils.HeaderGUI(CameraMessages.UIElements);
            for (int i = 0; i < _uiElements.arraySize; i++)
            {
                SerializedProperty elementProperty =
                    _uiElements.GetArrayElementAtIndex(i);
                if (elementProperty.objectReferenceValue == null)
                {
                    _uiElements.DeleteArrayElementAtIndex(i);
                    _uiElementsData.DeleteArrayElementAtIndex(i);
                    i--;
                    continue;
                }

                UIElement element =
                    _uiElements.GetArrayElementAtIndex(i).objectReferenceValue
                        as UIElement;
                UIElementGUI(element, i);
            }
        }

        private void UIElementGUI(UIElement element, int index)
        {
            GameObject uiElement = element.gameObject;
            bool foldout =
                EditorPrefs.GetBool("UIElementFoldout" + index, false);
            foldout = EditorGUILayout.Foldout(foldout, uiElement.name);
            EditorPrefs.SetBool("UIElementFoldout" + index, foldout);

            if (foldout)
            {
                EditorGUI.indentLevel++;

                UIElementType currentType = element.GetUIElementType();
                UIElementType newType =
                    (UIElementType)EditorGUILayout.EnumPopup("Type",
                        currentType);

                EditorGUILayout.ObjectField("Transform", uiElement.transform,
                    typeof(Transform), true);

                element.DisplayGUI();

                RemoveUIElementGUI(index);

                EditorGUI.indentLevel--;

                if (newType != currentType)
                {
                    ChangeTypeOfUIElement(index, newType);
                }
            }
        }

        private void CreateUIElementGUI()
        {
            if (GUILayout.Button(GeneralMessages.Add))
            {
                UIElement uiElement = AddNewUIElement(UIElementType.Basic);

                AttachNewUIElement(uiElement);
            }
        }

        public UIElement AddNewUIElement(UIElementType elementType)
        {
            if (_canvas.objectReferenceValue == null)
            {
                throw new Exception(
                    "No canvas assigned to CameraUIController!");
            }

            GameObject uiGameObject = new GameObject("UIElement");
            RectTransform rectTransform =
                uiGameObject.AddComponent<RectTransform>();
            GameObject canvasGameObject =
                _canvas.objectReferenceValue as GameObject;

            uiGameObject.transform.SetParent(canvasGameObject.transform,
                false);
            rectTransform.anchoredPosition = new Vector2(0, 0);
            rectTransform.sizeDelta = new Vector2(100, 100);

            UIElementData elementData = CreateUIElementData(elementType);
            UIElement uiElement = CreateUIElement(elementData, uiGameObject);

            if (_isActive.boolValue)
                uiElement.Activate();
            else
                uiElement.Deactivate();

            return uiElement;
        }

        private UIElementData CreateUIElementData(UIElementType elementType)
        {
            UIElementData uiElementData;
            switch (elementType)
            {
                case UIElementType.Basic:
                    uiElementData = CreateInstance<UIElementData>();
                    break;
                case UIElementType.StaticSprite:
                    uiElementData = CreateInstance<UISpriteElementData>();
                    break;
                // Add more cases for additional UIElement types
                default:
                    throw new ArgumentOutOfRangeException();
            }

            uiElementData.Type = elementType;
            return uiElementData;
        }

        private UIElement CreateUIElement(UIElementData elementData,
            GameObject uiGameObject)
        {
            UIElement uiElement;

            switch (elementData.Type)
            {
                case UIElementType.Basic:
                    uiElement = uiGameObject.AddComponent<BasicUIElement>();
                    break;
                case UIElementType.StaticSprite:
                    uiElement =
                        uiGameObject.AddComponent<StaticSpriteUIElement>();
                    break;
                // Add more cases for additional UIElement types
                default:
                    throw new ArgumentOutOfRangeException();
            }

            uiElement.Data = elementData;
            uiElement.InitializeUIElement();

            return uiElement;
        }

        public void ChangeTypeOfUIElement(int index,
            UIElementType uiElementType)
        {
            if (index < _uiElements.arraySize)
            {
                UIElement uiElement =
                    _uiElements.GetArrayElementAtIndex(index)
                        .objectReferenceValue as UIElement;
                GameObject uiGameObject = uiElement.gameObject;
                DestroyImmediate(uiElement);

                UIElementData elementData =
                    CreateUIElementData(uiElementType);
                _uiElements.GetArrayElementAtIndex(index)
                        .objectReferenceValue =
                    CreateUIElement(elementData, uiGameObject);
                _uiElementsData.GetArrayElementAtIndex(index)
                    .objectReferenceValue = elementData;
            }
        }

        private void RemoveUIElementGUI(int index)
        {
            if (GUILayout.Button(GeneralMessages.Remove))
            {
                // Get the element to be removed
                UIElement element =
                    _uiElements.GetArrayElementAtIndex(index)
                        .objectReferenceValue as UIElement;

                // Remove the UIElement component and associated ScriptableObject data
                if (element != null)
                {
                    // Destroy UIElementData
                    if (element.Data != null)
                    {
                        DestroyImmediate(element.Data);
                    }

                    // Destroy UIElement GameObject
                    DestroyImmediate(element.gameObject);
                }

                // Remove the element from serialized lists
                _uiElements.DeleteArrayElementAtIndex(index);
                _uiElementsData.DeleteArrayElementAtIndex(index);
            }
        }

        private void AttachNewUIElement(UIElement uiElement)
        {
            _uiElements.arraySize++;
            _uiElementsData.arraySize++;

            _uiElements.GetArrayElementAtIndex(_uiElements.arraySize - 1)
                .objectReferenceValue = uiElement;
            _uiElementsData
                .GetArrayElementAtIndex(_uiElementsData.arraySize - 1)
                .objectReferenceValue = uiElement.Data;
        }

        public override bool DenyAccess()
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