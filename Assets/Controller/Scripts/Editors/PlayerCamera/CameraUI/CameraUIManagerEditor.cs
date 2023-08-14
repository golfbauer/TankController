using System;
using Controller.Scripts.Editors.Utils;
using Controller.Scripts.Managers.PlayerCamera;
using Controller.Scripts.Managers.PlayerCamera.CameraUI;
using Controller.Scripts.Managers.PlayerCamera.CameraUI.UIGroups;
using Controller.Scripts.Managers.PlayerCamera.CameraUI.UIGroups.BasicUIGroup;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Controller.Scripts.Editors.PlayerCamera.CameraUI
{
    [CustomEditor(typeof(CameraUIManager))]
    public class CameraUIManagerEditor : TankComponentEditor
    {
        private SerializedProperty _canvas;
        private SerializedProperty _isActive;
        protected SerializedProperty uiGroups;

        public virtual void OnEnable()
        {
            _canvas = serializedObject.FindProperty("canvas");
            _isActive = serializedObject.FindProperty("isActive");
            uiGroups = serializedObject.FindProperty("uiGroups");

            transform = ((CameraUIManager)target).gameObject.transform;
        }

        public override void SetUpGUI()
        {
            GUIUtils.PropFieldGUI(_isActive, CameraMessages.ShowUI);
            GUIUtils.PropFieldGUI(_canvas, CameraMessages.Canvas);
            ShowUIGroupsGUI();
            GUIUtils.Space(2);
            CreateUIGroupGUI();
            UpdateAllGUI();
        }

        protected void ShowUIGroupsGUI()
        {
            GUIUtils.HeaderGUI(CameraMessages.UIGroups);
            for (int i = 0; i < uiGroups.arraySize; i++)
            {
                SerializedProperty elementProperty = uiGroups.GetArrayElementAtIndex(i);
                if (elementProperty.objectReferenceValue == null)
                {
                    uiGroups.DeleteArrayElementAtIndex(i);
                    i--;
                    continue;
                }

                UIGroup group = uiGroups.GetArrayElementAtIndex(i).objectReferenceValue as UIGroup;
                UIGroupGUI(group, i);
            }
        }

        private void UIGroupGUI(UIGroup group, int index)
        {
            GameObject groupGameObject = group.gameObject;
            bool foldout = EditorPrefs.GetBool("UIGroupFoldout" + index, false);
            foldout = EditorGUILayout.Foldout(foldout, groupGameObject.name);
            EditorPrefs.SetBool("UIGroupFoldout" + index, foldout);

            if (foldout)
            {
                EditorGUI.indentLevel++;

                UIGroupType newType = (UIGroupType)EditorGUILayout.EnumPopup(GeneralMessages.Type, group.type);
                EditorGUILayout.ObjectField(GeneralMessages.Transform, groupGameObject.transform, typeof(Transform), true);
                RemoveUIGroupGUI(index, group);

                EditorGUI.indentLevel--;

                if (newType != group.type)
                    ReplaceUIGroup(group, index, newType);
            }
        }

        protected void CreateUIGroupGUI()
        {
            if (GUILayout.Button(GeneralMessages.Add))
            {
                UIGroup uiGroup = CreateUIGroup(UIGroupType.Basic);

                uiGroups.arraySize++;
                uiGroups.GetArrayElementAtIndex(uiGroups.arraySize - 1).objectReferenceValue = uiGroup;
            }
        }

        protected virtual UIGroup CreateUIGroup(UIGroupType type)
        {
            if (_canvas.objectReferenceValue == null)
            {
                throw new NullReferenceException(CameraMessages.NoCanvasSelected);
            }

            GameObject uiGameObject = new GameObject(CameraMessages.UIGroup);
            RectTransform rectTransform = uiGameObject.AddComponent<RectTransform>();
            GameObject canvasGameObject = _canvas.objectReferenceValue as GameObject;

            uiGameObject.transform.SetParent(canvasGameObject.transform, false);
            rectTransform.anchoredPosition = new Vector2(0, 0);
            rectTransform.sizeDelta = new Vector2(100, 100);

            UIGroup uiGroup = SelectUIGroup(uiGameObject, type);
            return uiGroup;
        }

        protected UIGroup SelectUIGroup(GameObject uiGameObject, UIGroupType type)
        {
            UIGroup uiGroup;

            switch (type)
            {
                case UIGroupType.Basic:
                    uiGroup = uiGameObject.AddComponent<BasicUIGroup>();
                    break;
                case UIGroupType.Ammo:
                    uiGroup = uiGameObject.AddComponent<AmmoUIGroup>();
                    break;
                case UIGroupType.Group:
                    uiGroup = uiGameObject.AddComponent<GroupUIGroup>();
                    break;
                // Add more cases for additional UIGroup types
                default:
                    throw new ArgumentOutOfRangeException(CameraMessages.GroupTypeNotImplemented);
            }
            
            uiGroup.Initialize();
            return uiGroup;
        }

        private void RemoveUIGroupGUI(int index, UIGroup uiGroup)
        {
            if (GUILayout.Button(GeneralMessages.Remove))
            {
                RemoveUIGroup(index, uiGroup);
            }
        }

        private void RemoveUIGroup(int index, UIGroup uiGroup)
        {
            if (uiGroup != null)
            {
                DestroyImmediate(uiGroup.gameObject);
            }
            uiGroups.DeleteArrayElementAtIndex(index);
        }

        private void ReplaceUIGroup(UIGroup uiGroup, int index, UIGroupType type)
        {
            GameObject uiGameObject = uiGroup.gameObject;
            DestroyImmediate(uiGroup);
            
            UIGroup newUIGroup = SelectUIGroup(uiGameObject, type);
            uiGroups.GetArrayElementAtIndex(index).objectReferenceValue = newUIGroup;
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
            for (int i = 0; i < uiGroups.arraySize; i++)
            {
                SerializedProperty elementProperty = uiGroups.GetArrayElementAtIndex(i);
                if(elementProperty.objectReferenceValue == null)
                {
                    uiGroups.DeleteArrayElementAtIndex(i);
                    i--;
                }
                
                UIGroup group = uiGroups.GetArrayElementAtIndex(i).objectReferenceValue as UIGroup;
                group.ToggleUIElements(_isActive.boolValue);
            }
        }
    }
}