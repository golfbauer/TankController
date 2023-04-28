using System;
using Controller.Scripts.Editors.Wheels.CreateRearWheel;
using Controller.Scripts.Editors.Wheels.DriveWheel;
using Controller.Scripts.Editors.Wheels.SupportWheel;
using Controller.Scripts.Editors.Wheels.SuspensionWheel;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Controller.Scripts.Editors.Tank
{
    [CustomEditor(typeof(CreateTank))]
    [CanEditMultipleObjects]
    public class CreateTankEditor: Editor
    {
        
        private SerializedProperty _createComponent;
        private SerializedProperty _hullMass;
        
        private Transform _transform;
        
        private void OnEnable()
        {
            _createComponent = serializedObject.FindProperty("createComponent");
            _hullMass = serializedObject.FindProperty("hullMass");
            
            _transform = ((CreateTank) target).gameObject.transform;
        }

        private void SetUpGUI()
        {
            GUIUtils.HeaderGUI(TankUtilsMessages.AddComponents);
            GUIUtils.PropFieldGUI(_createComponent, TankUtilsMessages.Component);
            CreateButtonGUI();
            
            GUIUtils.HeaderGUI(TankUtilsMessages.Hull);
            GUIUtils.PropFieldGUI(_hullMass, TankUtilsMessages.Mass);
        }
        
        private void CreateButtonGUI()
        {
            if (GUILayout.Button("Create"))
            {
                AttachComponent();
            }
        }
        
        private void AttachComponent()
        {
            var createComponent = (CreateComponent) _createComponent.enumValueFlag;

            switch (createComponent)
            {
                case CreateComponent.SupportWheel:
                    AttachWheelComponent("Support Wheel", typeof(CreateSupportWheel));
                    break;
                
                case CreateComponent.SuspensionWheel:
                    AttachWheelComponent("Suspension Wheel", typeof(CreateSuspensionWheel));
                    break;
                
                case CreateComponent.DriveWheel:
                    AttachWheelComponent("Drive Wheel", typeof(CreateDriveWheel));
                    break;
                
                case CreateComponent.RearWheel:
                    AttachWheelComponent("Rear Wheel", typeof(CreateRearWheel));
                    break;
            }
        }

        private void AttachWheelComponent(string wheelName, Type wheelType)
        {
            GameObject wheel = new GameObject(wheelName);
            wheel.transform.parent = _transform;
            wheel.transform.localPosition = Vector3.zero;
            wheel.transform.localRotation = Quaternion.identity;
            wheel.transform.localScale = Vector3.one;
            wheel.AddComponent(wheelType);
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update ();
            
            if (PrefabStageUtility.GetCurrentPrefabStage() == null)
            {
                GUIUtils.DenyAccessGUI();
                return;
            }

            SetUpGUI();
            
            UpdateAll();

            serializedObject.ApplyModifiedProperties();
        }

        private void UpdateAll()
        {
            UpdateRigidbody();
        }

        

        private void UpdateRigidbody()
        {
            Rigidbody rigidbody = _transform.GetComponent<Rigidbody>();
            if (rigidbody == null)
                rigidbody = _transform.gameObject.AddComponent<Rigidbody>();
            
            rigidbody.mass = _hullMass.floatValue;
        }
    }
}