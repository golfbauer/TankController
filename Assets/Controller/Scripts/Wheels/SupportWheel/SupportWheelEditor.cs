using Controller.Scripts.Utils;
using Controller.Scripts.Wheels.Services;
using UnityEditor;
using UnityEngine;

namespace Controller.Scripts.Wheels.SupportWheel
{
    [CustomEditor(typeof(SupportWheel))]
    [CanEditMultipleObjects]
    public class SupportWheelEditor : WheelEditor
    {
        private SerializedProperty _leftWheelAxisProp;
        private SerializedProperty _leftWheelMaterialProp;

        private SerializedProperty _leftWheelMeshProp;

        private SerializedProperty _leftWheelRotationProp;
        private SerializedProperty _leftWheelTorqueProp;

        private SerializedProperty _resizeWheelProp;
        private SerializedProperty _rightWheelAxisProp;
        private SerializedProperty _rightWheelMaterialProp;

        private SerializedProperty _rightWheelMeshProp;

        private SerializedProperty _rightWheelRotationProp;
        private SerializedProperty _rightWheelTorqueProp;
        private SerializedProperty _showLabels;
        private SerializedProperty _wheelColliderMaterialProp;

        private SerializedProperty _wheelColliderRadiusProp;
        private SerializedProperty _wheelCountProp;

        private SerializedProperty _wheelDistanceProp;

        private SerializedProperty _wheelMassProp;
        private SerializedProperty _wheelResizeScaleProp;
        private SerializedProperty _wheelResizeSpeedProp;
        private SerializedProperty _wheelSpacingProp;

        private void OnEnable()
        {
            _showLabels = serializedObject.FindProperty("showLabels");

            _rightWheelRotationProp = serializedObject.FindProperty("rightWheelRotation");
            _rightWheelTorqueProp = serializedObject.FindProperty("rightWheelTorque");
            _leftWheelAxisProp = serializedObject.FindProperty("leftWheelHingeAxis");

            _leftWheelRotationProp = serializedObject.FindProperty("leftWheelRotation");
            _leftWheelTorqueProp = serializedObject.FindProperty("leftWheelTorque");
            _rightWheelAxisProp = serializedObject.FindProperty("rightWheelHingeAxis");

            _rightWheelMeshProp = serializedObject.FindProperty("rightWheelMesh");
            _rightWheelMaterialProp = serializedObject.FindProperty("rightWheelMaterials");

            _leftWheelMeshProp = serializedObject.FindProperty("leftWheelMesh");
            _leftWheelMaterialProp = serializedObject.FindProperty("leftWheelMaterials");

            _wheelColliderRadiusProp = serializedObject.FindProperty("wheelColliderRadius");
            _wheelColliderMaterialProp = serializedObject.FindProperty("wheelColliderMaterial");

            _wheelMassProp = serializedObject.FindProperty("wheelMass");

            _wheelDistanceProp = serializedObject.FindProperty("wheelDistance");
            _wheelCountProp = serializedObject.FindProperty("wheelCount");
            _wheelSpacingProp = serializedObject.FindProperty("wheelSpacing");

            _resizeWheelProp = serializedObject.FindProperty("resizeWheel");
            _wheelResizeScaleProp = serializedObject.FindProperty("wheelResizeScale");
            _wheelResizeSpeedProp = serializedObject.FindProperty("wheelResizeSpeed");

            transform = ((SupportWheel)target).gameObject.transform;

            AttachWheelManager(_wheelColliderRadiusProp.floatValue);
        }

        public override void SetUpGUI()
        {
            GUIUtils.HeaderGUI(WheelMessages.GeneralSettings);
            GUIUtils.PropFieldGUI(_showLabels);

            GUIUtils.HeaderGUI(WheelMessages.RightWheelSettings);
            GUIUtils.PropFieldGUI(_rightWheelRotationProp, WheelMessages.EulerRotation);
            GUIUtils.PropFieldGUI(_rightWheelTorqueProp, WheelMessages.TorqueDirection);
            GUIUtils.PropFieldGUI(_rightWheelAxisProp, WheelMessages.HingeAxis);
            GUIUtils.PropFieldGUI(_rightWheelMeshProp, WheelMessages.Mesh);
            GUIUtils.PropFieldGUI(_rightWheelMaterialProp, WheelMessages.Material);

            GUIUtils.HeaderGUI(WheelMessages.LeftWheelSettings);
            GUIUtils.PropFieldGUI(_leftWheelRotationProp, WheelMessages.EulerRotation);
            GUIUtils.PropFieldGUI(_leftWheelTorqueProp, WheelMessages.TorqueDirection);
            GUIUtils.PropFieldGUI(_leftWheelAxisProp, WheelMessages.HingeAxis);
            GUIUtils.PropFieldGUI(_leftWheelMeshProp, WheelMessages.Mesh);
            GUIUtils.PropFieldGUI(_leftWheelMaterialProp, WheelMessages.Material);

            GUIUtils.HeaderGUI(WheelMessages.GeneralWheelSettings);
            GUIUtils.PropFieldGUI(_wheelMassProp, WheelMessages.Mass);
            GUIUtils.PropFieldGUI(_wheelCountProp, WheelMessages.Count);
            GUIUtils.SliderGUI(_wheelDistanceProp, 0.1f, 5f, WheelMessages.Distance);
            GUIUtils.SliderGUI(_wheelSpacingProp, 0.1f, 5f, WheelMessages.Spacing);

            GUIUtils.PropFieldGUI(_wheelColliderMaterialProp, WheelMessages.ColliderMaterial);
            GUIUtils.SliderGUI(_wheelColliderRadiusProp, 0.1f, 5f, WheelMessages.ColliderRadius);

            GUIUtils.PropFieldGUI(_resizeWheelProp, WheelMessages.Resize);
            if (_resizeWheelProp.boolValue)
            {
                GUIUtils.SliderGUI(_wheelResizeScaleProp, 0.1f, 1f, WheelMessages.ResizeScale);
                GUIUtils.PropFieldGUI(_wheelResizeSpeedProp, WheelMessages.ResizeSpeed);
            }

            UpdateAllGUI();
        }

        public override void ApplyUpdate()
        {
            BulkDestroyComponents();
            BulkCreateWheels();
        }

        private void BulkCreateWheels()
        {
            for (var i = 0; i < _wheelCountProp.intValue; i++)
            {
                CreateWheel(true, i);
                CreateWheel(false, i);
            }
        }

        private void CreateWheel(bool isLeft, int i)
        {
            var wheelName = isLeft ? WheelMessages.LeftWheel : WheelMessages.RightWheel;
            var wheelDistance = isLeft
                ? -_wheelDistanceProp.floatValue
                : _wheelDistanceProp.floatValue;
            var eulerRotation = isLeft
                ? _leftWheelRotationProp.vector3Value
                : _rightWheelRotationProp.vector3Value;

            var wheel = new GameObject(wheelName + i)
            {
                transform =
                {
                    parent = transform,
                    localPosition = new Vector3(wheelDistance, 0, i * _wheelSpacingProp.floatValue),
                    localRotation = Quaternion.Euler(eulerRotation)
                }
            };

            DrawUtils.ShowLabel(wheel, _showLabels);
            AttachComponents(wheel, isLeft);
        }

        private void AttachComponents(GameObject wheel, bool isLeft)
        {
            var torque = isLeft
                ? _leftWheelTorqueProp.vector3Value
                : _rightWheelTorqueProp.vector3Value;
            var hingeAxis = isLeft
                ? _leftWheelAxisProp.vector3Value
                : _rightWheelAxisProp.vector3Value;

            UpdateSphereCollider(
                wheel.transform,
                _wheelColliderRadiusProp,
                _wheelColliderMaterialProp
            );
            AttachWheelScript(wheel, isLeft, torque);
            UpdateMesh(
                wheel.transform,
                isLeft ? _leftWheelMeshProp : _rightWheelMeshProp,
                isLeft ? _leftWheelMaterialProp : _rightWheelMaterialProp
            );
            UpdateRigidbody(wheel.transform, _wheelMassProp);
            AttachWheelHingeJoint(wheel, transform.parent, hingeAxis);
            AttachWheelResizeScript(
                wheel,
                _resizeWheelProp.boolValue,
                _wheelResizeScaleProp.floatValue,
                _wheelResizeSpeedProp.floatValue
            );

            SetLayers();
        }
    }
}
