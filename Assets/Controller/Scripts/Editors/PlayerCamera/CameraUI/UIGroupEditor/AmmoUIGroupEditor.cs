using System;
using System.Collections.Generic;
using Controller.Scripts.Editors.Utils;
using Controller.Scripts.Managers.Ammunition;
using Controller.Scripts.Managers.PlayerCamera;
using Controller.Scripts.Managers.PlayerCamera.CameraUI.UIElements;
using Controller.Scripts.Managers.PlayerCamera.CameraUI.UIElements.AmmoElements;
using Controller.Scripts.Managers.PlayerCamera.CameraUI.UIElements.BasicElements;
using Controller.Scripts.Managers.PlayerCamera.CameraUI.UIGroups.BasicUIGroup;
using UnityEditor;
using UnityEngine;

namespace Controller.Scripts.Editors.PlayerCamera.CameraUI.UIGroupEditor
{
    [CustomEditor(typeof(AmmoUIGroup), true)]
    [CanEditMultipleObjects]
    public class AmmoUIGroupEditor : UIGroupEditor
    {
        private SerializedProperty _ammunitionType;
        private SerializedProperty _ammunitionManager;

        protected override void OnEnable()
        {
            base.OnEnable();
            _ammunitionType = serializedObject.FindProperty("ammunitionType");
            _ammunitionManager = serializedObject.FindProperty("ammunitionManager");
        }

        public override void SetUpGUI()
        {
            GUIUtils.PropFieldGUI(_ammunitionManager);
            if (_ammunitionManager.objectReferenceValue == null)
                return;
            
            List<AmmunitionType> ammunitionTypes =
                ((AmmunitionManager)_ammunitionManager.objectReferenceValue).ammunitionTypes;
            
            int index = _ammunitionType.objectReferenceValue == null
                ? 0
                : ammunitionTypes.IndexOf((AmmunitionType)_ammunitionType.objectReferenceValue);
            
            _ammunitionType.objectReferenceValue =
                ammunitionTypes[
                    EditorGUILayout.Popup("Ammunition Type", index,
                        ammunitionTypes.ConvertAll(type => type.ToString()).ToArray())];

            base.SetUpGUI();
        }
        
        protected override UIElement SelectUIElement(UIElementType type, GameObject uiGameObject)
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
                case UIElementType.AmmoImage:
                    uiElement = uiGameObject.AddComponent<AmmoImageUIElement>();
                    break;
                case UIElementType.AmmoText:
                    uiElement = uiGameObject.AddComponent<AmmoTextUIElement>();
                    break;
                // Add more cases for additional UIElement types
                default:
                    throw new ArgumentOutOfRangeException();
            }

            uiElement.InitializeUIElement();
            return uiElement;
        }
    }
}