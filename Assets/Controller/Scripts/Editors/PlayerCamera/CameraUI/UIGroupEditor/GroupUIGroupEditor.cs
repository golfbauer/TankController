using System;
using Controller.Scripts.Editors.Utils;
using Controller.Scripts.Managers.PlayerCamera;
using Controller.Scripts.Managers.PlayerCamera.CameraUI.UIGroups;
using Controller.Scripts.Managers.PlayerCamera.CameraUI.UIGroups.BasicUIGroup;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Controller.Scripts.Editors.PlayerCamera.CameraUI.UIGroupEditor
{
    [CustomEditor(typeof(GroupUIGroup), true)]
    [CanEditMultipleObjects]
    public class GroupUIGroupEditor : CameraUIManagerEditor
    {
        public override void OnEnable()
        {
            uiGroups = serializedObject.FindProperty("groups");
            transform = ((GroupUIGroup)target).gameObject.transform;
        }
        
        public override void SetUpGUI()
        {
            GUIUtils.PropFieldGUI(uiGroups);
            ShowUIGroupsGUI();
            GUIUtils.Space(2);
            CreateUIGroupGUI();
            UpdateAllGUI();
        }
        
        protected override UIGroup CreateUIGroup(UIGroupType type)
        {
            GameObject uiGameObject = new GameObject(CameraMessages.UIGroup);
            RectTransform rectTransform = uiGameObject.AddComponent<RectTransform>();

            uiGameObject.transform.SetParent(transform, false);
            rectTransform.anchoredPosition = new Vector2(0, 0);
            rectTransform.sizeDelta = new Vector2(100, 100);

            UIGroup uiGroup = SelectUIGroup(uiGameObject, type);
            return uiGroup;
        }
        
        public override void BulkUpdateComponents()
        {
            for (int i = 0; i < uiGroups.arraySize; i++)
            {
                SerializedProperty elementProperty = uiGroups.GetArrayElementAtIndex(i);
                if(elementProperty.objectReferenceValue == null)
                {
                    uiGroups.DeleteArrayElementAtIndex(i);
                    i--;
                }
            }
        }
    }
}