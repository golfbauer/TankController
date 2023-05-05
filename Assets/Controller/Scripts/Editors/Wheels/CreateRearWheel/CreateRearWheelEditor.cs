using Controller.Scripts.Editors.Wheels.DriveWheel;
using Controller.Scripts.Managers.Wheels;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Controller.Scripts.Editors.Wheels.CreateRearWheel
{
    [CustomEditor(typeof(CreateRearWheel))]
    [CanEditMultipleObjects]
    public class CreateRearWheelEditor : CreateDriveWheelEditor
    {
        private SerializedProperty _showLabels;

        private SerializedProperty _rightWheelRotationProp;
        private SerializedProperty _leftTorqueDirectionProp;
        
        private SerializedProperty _leftWheelRotationProp;
        private SerializedProperty _rightTorqueDirectionProp;

        private SerializedProperty _wheelColliderRadiusProp;
        private SerializedProperty _wheelColliderMaterialProp;

        private SerializedProperty _rightWheelMeshProp;
        private SerializedProperty _rightWheelMaterialProp;
        
        private SerializedProperty _leftWheelMeshProp;
        private SerializedProperty _leftWheelMaterialProp;

        private SerializedProperty _wheelMassProp;

        private SerializedProperty _wheelDistanceProp;
        private SerializedProperty _wheelSpacingProp;

        private void OnEnable()
        {
            _showLabels = serializedObject.FindProperty("showLabels");

            _rightWheelRotationProp = serializedObject.FindProperty("rightWheelRotation");
            _rightTorqueDirectionProp = serializedObject.FindProperty("rightTorqueDirection");
            
            _leftWheelRotationProp = serializedObject.FindProperty("leftWheelRotation");
            _leftTorqueDirectionProp = serializedObject.FindProperty("leftTorqueDirection");

            _wheelColliderRadiusProp = serializedObject.FindProperty("wheelColliderRadius");
            _wheelColliderMaterialProp = serializedObject.FindProperty("wheelColliderMaterial");

            _rightWheelMeshProp = serializedObject.FindProperty("rightWheelMesh");
            _rightWheelMaterialProp = serializedObject.FindProperty("rightWheelMaterial");
            
            _leftWheelMeshProp = serializedObject.FindProperty("leftWheelMesh");
            _leftWheelMaterialProp = serializedObject.FindProperty("leftWheelMaterial");

            _wheelMassProp = serializedObject.FindProperty("wheelMass");

            _wheelDistanceProp = serializedObject.FindProperty("wheelDistance");
            _wheelSpacingProp = serializedObject.FindProperty("wheelSpacing");

            Transform = ((CreateRearWheel) target).gameObject.transform;
            
            AttachWheelManager();
        }
        
        protected override void SetUpGUI()
        {
            GUIUtils.HeaderGUI(WheelUtilsMessages.GeneralSettings);
            GUIUtils.PropFieldGUI(_showLabels);
            
            GUIUtils.HeaderGUI(WheelUtilsMessages.RightWheelSettings);
            GUIUtils.PropFieldGUI(_rightWheelRotationProp, WheelUtilsMessages.EulerRotation);
            GUIUtils.PropFieldGUI(_rightTorqueDirectionProp, WheelUtilsMessages.TorqueDirection);
            GUIUtils.PropFieldGUI(_rightWheelMeshProp, WheelUtilsMessages.Mesh);
            GUIUtils.PropFieldGUI(_rightWheelMaterialProp, WheelUtilsMessages.Material);
            
            GUIUtils.HeaderGUI(WheelUtilsMessages.LeftWheelSettings);
            GUIUtils.PropFieldGUI(_leftWheelRotationProp, WheelUtilsMessages.EulerRotation);
            GUIUtils.PropFieldGUI(_leftTorqueDirectionProp, WheelUtilsMessages.TorqueDirection);
            GUIUtils.PropFieldGUI(_leftWheelMeshProp, WheelUtilsMessages.Mesh);
            GUIUtils.PropFieldGUI(_leftWheelMaterialProp, WheelUtilsMessages.Material);
            
            GUIUtils.HeaderGUI(WheelUtilsMessages.GeneralWheelSettings);
            GUIUtils.SliderGUI(_wheelColliderRadiusProp, 0.1f, 5f, WheelUtilsMessages.ColliderRadius);
            GUIUtils.PropFieldGUI(_wheelColliderMaterialProp, WheelUtilsMessages.ColliderMaterial);
            
            GUIUtils.PropFieldGUI(_wheelMassProp, WheelUtilsMessages.Mass);
            
            GUIUtils.SliderGUI(_wheelDistanceProp, 0.1f, 5f, WheelUtilsMessages.Distance);
            GUIUtils.SliderGUI(_wheelSpacingProp, 0.1f, 5f, WheelUtilsMessages.Spacing);

            UpdateAllGUI();
        }

        protected override void BulkUpdateComponents()
        {
            BulkDestroyComponents();
            BulkCreateWheels();

            UpdateAll = false;
        }

        private void BulkCreateWheels()
        {
            CreateWheel(true);
            CreateWheel(false);
        }

        private void CreateWheel(bool isLeft)
        {
            string wheelName = isLeft ? WheelUtilsMessages.LeftWheel : WheelUtilsMessages.RightWheel;
            float wheelDistance = isLeft ? _wheelDistanceProp.floatValue : -_wheelDistanceProp.floatValue;
            Vector3 eulerRotation = isLeft ? _leftWheelRotationProp.vector3Value : _rightWheelRotationProp.vector3Value;

            var wheel = new GameObject(wheelName)
            {
                transform =
                {
                    parent = Transform,
                    localPosition = new Vector3(wheelDistance, 0, _wheelSpacingProp.floatValue),
                    localRotation = Quaternion.Euler(eulerRotation)
                },
            };

            WheelsUtils.ShowLabel(wheel, _showLabels);
            AttachComponents(wheel, isLeft);
        }

        private void AttachComponents(GameObject wheel, bool isLeft)
        {
            AttachCollider(wheel, _wheelColliderRadiusProp, _wheelColliderMaterialProp);
            Vector3 torqueDirection = isLeft ? _leftTorqueDirectionProp.vector3Value : _rightTorqueDirectionProp.vector3Value;
            AttachWheelScript(wheel, isLeft, torqueDirection);
            AttachFixedRigidbody(wheel, _wheelMassProp);
            AttachWheelHingeJoint(wheel,Transform.parent, torqueDirection);
            AttachMesh(wheel, isLeft ? _leftWheelMeshProp : _rightWheelMeshProp, isLeft ? _leftWheelMaterialProp : _rightWheelMaterialProp);
        }
    }
}