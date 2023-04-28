using UnityEditor;
using UnityEngine;

namespace Controller.Scripts.Editors.Wheels.SupportWheel
{
    [CustomEditor(typeof(CreateSupportWheel))]
    [CanEditMultipleObjects]
    public class CreateSupportWheelEditor: CreateWheelEditor
    {
        private SerializedProperty _showLabels;

        private SerializedProperty _wheelEulerRotationProp;

        private SerializedProperty _wheelColliderRadiusProp;
        private SerializedProperty _wheelColliderMaterialProp;

        private SerializedProperty _wheelMeshProp;
        private SerializedProperty _wheelMaterialProp;

        private SerializedProperty _wheelDistanceProp;
        private SerializedProperty _wheelCountProp;
        private SerializedProperty _wheelSpacingProp;

        private void OnEnable()
        {
            _showLabels = serializedObject.FindProperty("showLabels");
            
            _wheelEulerRotationProp = serializedObject.FindProperty("wheelEulerRotation");
            
            _wheelColliderRadiusProp = serializedObject.FindProperty("wheelColliderRadius");
            _wheelColliderMaterialProp = serializedObject.FindProperty("wheelColliderMaterial");
            
            _wheelMeshProp = serializedObject.FindProperty("wheelMesh");
            _wheelMaterialProp = serializedObject.FindProperty("wheelMaterial");

            _wheelDistanceProp = serializedObject.FindProperty("wheelDistance");
            _wheelCountProp = serializedObject.FindProperty("wheelCount");
            _wheelSpacingProp = serializedObject.FindProperty("wheelSpacing");

            Transform = ((CreateSupportWheel) target).gameObject.transform;
            
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
            GUIUtils.SliderGUI(_wheelDistanceProp, 0.1f, 5f, WheelUtilsMessages.Distance);
            GUIUtils.PropFieldGUI(_wheelCountProp, WheelUtilsMessages.Count);
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
            for (int i = 0; i < _wheelCountProp.intValue; i++)
            {
                CreateWheel(true, i);
                CreateWheel(false, i);
            }
        }
        
        private void CreateWheel(bool isLeft, int i)
        {
            string wheelName = isLeft ? "L Wheel " : "R Wheel ";
            float wheelDistance = isLeft ? _wheelDistanceProp.floatValue : -_wheelDistanceProp.floatValue;
            Vector3 eulerRotation = _wheelEulerRotationProp.vector3Value;
            
            var wheel = new GameObject(wheelName + i)
            {
                transform =
                {
                    parent = Transform,
                    localPosition = new Vector3(i * _wheelSpacingProp.floatValue, 0, wheelDistance),
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
            AttachMesh(wheel, _wheelMeshProp, _wheelMaterialProp);
        }
    }
}