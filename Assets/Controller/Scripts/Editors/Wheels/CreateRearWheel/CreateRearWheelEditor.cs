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

        private SerializedProperty _wheelEulerRotationProp;

        private SerializedProperty _wheelColliderRadiusProp;
        private SerializedProperty _wheelColliderMaterialProp;

        private SerializedProperty _wheelMeshProp;
        private SerializedProperty _wheelMaterialProp;

        private SerializedProperty _wheelMassProp;

        private SerializedProperty _wheelDistanceProp;
        private SerializedProperty _wheelSpacingProp;

        private void OnEnable()
        {
            _showLabels = serializedObject.FindProperty("showLabels");
            
            _wheelEulerRotationProp = serializedObject.FindProperty("wheelEulerRotation");
            
            _wheelColliderRadiusProp = serializedObject.FindProperty("wheelColliderRadius");
            _wheelColliderMaterialProp = serializedObject.FindProperty("wheelColliderMaterial");
            
            _wheelMeshProp = serializedObject.FindProperty("wheelMesh");
            _wheelMaterialProp = serializedObject.FindProperty("wheelMaterial");
            
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
            
            GUIUtils.HeaderGUI(WheelUtilsMessages.WheelSettings);
            GUIUtils.PropFieldGUI(_wheelEulerRotationProp, WheelUtilsMessages.EulerRotation);
            GUIUtils.SliderGUI(_wheelColliderRadiusProp, 0.1f, 5f, WheelUtilsMessages.ColliderRadius);
            GUIUtils.PropFieldGUI(_wheelColliderMaterialProp, WheelUtilsMessages.ColliderMaterial);
            GUIUtils.PropFieldGUI(_wheelMeshProp, WheelUtilsMessages.Mesh);
            GUIUtils.PropFieldGUI(_wheelMaterialProp, WheelUtilsMessages.Material);
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
            Vector3 eulerRotation = _wheelEulerRotationProp.vector3Value;

            var wheel = new GameObject(wheelName)
            {
                transform =
                {
                    parent = Transform,
                    localPosition = new Vector3(_wheelSpacingProp.floatValue, 0, wheelDistance),
                    localRotation = Quaternion.Euler(eulerRotation)
                },
            };

            WheelsUtils.ShowLabel(wheel, _showLabels);
            AttachComponents(wheel, isLeft);
        }

        private void AttachComponents(GameObject wheel, bool isLeft)
        {
            AttachCollider(wheel, _wheelColliderRadiusProp, _wheelColliderMaterialProp);
            AttachWheelScript(wheel, isLeft);
            AttachRigidbody(wheel, _wheelMassProp);
            AttachWheelHingeJoint(wheel);
            AttachMesh(wheel, _wheelMeshProp, _wheelMaterialProp);
        }


        private void AttachWheelHingeJoint(GameObject wheel)
        {
            HingeJoint wheelHingeJoint = wheel.AddComponent<HingeJoint>();
            wheelHingeJoint.anchor = Vector3.zero;
            wheelHingeJoint.axis = Vector3.up;
            wheelHingeJoint.useSpring = false;
            wheelHingeJoint.useMotor = false;
            wheelHingeJoint.useLimits = false;
            wheelHingeJoint.connectedBody = Transform.parent.GetComponent<Rigidbody>();
        }
    }
}