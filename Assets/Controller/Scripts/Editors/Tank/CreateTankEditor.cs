using System;
using Controller.Scripts.Editors.Wheels.SupportWheel;
using UnityEditor;
using UnityEngine;

namespace Controller.Scripts.Editors.Tank
{
    [CustomEditor(typeof(CreateTank))]
    [CanEditMultipleObjects]
    public class CreateTankEditor: Editor
    {
        
        private SerializedProperty _createComponent;
        
        private Transform _transform;
        
        private void OnEnable()
        {
            _createComponent = serializedObject.FindProperty("createComponent");
            
            _transform = ((CreateTank) target).gameObject.transform;
        }
        
        private void CreateComponentGUI()
        {
            EditorGUILayout.PropertyField(_createComponent);
        }
        
        private void CreateButtonGUI()
        {
            if (GUILayout.Button("Create"))
            {
                CreateComponent();
            }
        }
        
        private void SetUpGUI()
        {
            CreateComponentGUI();
            CreateButtonGUI();
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update ();

            SetUpGUI();

            serializedObject.ApplyModifiedProperties();
        }

        private void CreateComponent()
        {
            var createComponent = (CreateComponent) _createComponent.enumValueFlag;

            if (createComponent == Tank.CreateComponent.CreateSupportWheel)
            {
                GameObject supportWheel = new GameObject("Support Wheel");
                supportWheel.transform.parent = _transform;
                supportWheel.transform.localPosition = Vector3.zero;
                supportWheel.transform.localRotation = Quaternion.identity;
                supportWheel.transform.localScale = Vector3.one;
                supportWheel.AddComponent<CreateSupportWheel>();
            }
        }

    }
}