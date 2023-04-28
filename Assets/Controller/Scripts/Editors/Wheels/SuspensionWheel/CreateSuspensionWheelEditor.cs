using UnityEditor;
using UnityEngine;

namespace Controller.Scripts.Editors.Wheels.SuspensionWheel
{
    [CustomEditor(typeof(CreateSuspensionWheel))]
    [CanEditMultipleObjects]
    public class CreateSuspensionWheelEditor : CreateWheelEditor
    {
        // Debug Settings
        private SerializedProperty _showLabelsProp;
        
        
        // General Settings
        private SerializedProperty _componentCountProp;
        private SerializedProperty _componentSpacingProp;
        private SerializedProperty _componentDistanceProp;
        
        // Wheel Settings
        // Wheel Rotation
        private SerializedProperty _wheelEulerRotationProp;

        // Wheel Collider Settings
        private SerializedProperty _wheelColliderRadiusProp;
        private SerializedProperty _wheelColliderMaterialProp;

        // Wheel Mesh Settings
        private SerializedProperty _wheelMeshProp;
        private SerializedProperty _wheelMaterialProp;

        // Wheel Spacing Settings
        private SerializedProperty _wheelDistanceProp;
        private SerializedProperty _wheelSpacingProp;
        private SerializedProperty _wheelOffsetProp;
        
        // Wheel Rigidbody Settings
        private SerializedProperty _wheelMassProp;


        // Suspension Settings
        // Suspension Rotation
        private SerializedProperty _suspensionEulerRotationProp;

        // Suspension Mesh Settings
        private SerializedProperty _suspensionMeshProp;
        private SerializedProperty _suspensionMaterialProp;
        
        // Suspension Spacing Settings
        private SerializedProperty _suspensionDistanceProp;
        private SerializedProperty _suspensionSpacingProp;
        
        // Suspension Rigidbody Settings
        private SerializedProperty _suspensionMassProp;
        
        // Suspension HingeJoint Settings
        private SerializedProperty _anchorOffsetProp;
        private SerializedProperty _springForceProp;
        private SerializedProperty _springDamperProp;
        private SerializedProperty _springTargetPositionProp;
        private SerializedProperty _springMinLimitAngleProp;
        private SerializedProperty _springMaxLimitAngleProp;

        private void OnEnable()
        {
            _showLabelsProp = serializedObject.FindProperty("showLabels");
            _componentCountProp = serializedObject.FindProperty("componentCount");
            _componentSpacingProp = serializedObject.FindProperty("componentSpacing");
            _componentDistanceProp = serializedObject.FindProperty("componentDistance");

            _wheelEulerRotationProp = serializedObject.FindProperty("wheelEulerRotation");

            _wheelColliderRadiusProp = serializedObject.FindProperty("wheelColliderRadius");
            _wheelColliderMaterialProp = serializedObject.FindProperty("wheelColliderMaterial");

            _wheelMeshProp = serializedObject.FindProperty("wheelMesh");
            _wheelMaterialProp = serializedObject.FindProperty("wheelMaterial");

            _wheelOffsetProp = serializedObject.FindProperty("wheelOffset");
            _wheelDistanceProp = serializedObject.FindProperty("wheelDistance");
            _wheelSpacingProp = serializedObject.FindProperty("wheelSpacing");
            
            _wheelMassProp = serializedObject.FindProperty("wheelMass");
            
            
            _suspensionEulerRotationProp = serializedObject.FindProperty("suspensionEulerRotation");
            
            _suspensionMeshProp = serializedObject.FindProperty("suspensionMesh");
            _suspensionMaterialProp = serializedObject.FindProperty("suspensionMaterial");
            
            _suspensionDistanceProp = serializedObject.FindProperty("suspensionDistance");
            _suspensionSpacingProp = serializedObject.FindProperty("suspensionSpacing");
            
            _suspensionMassProp = serializedObject.FindProperty("suspensionMass");
            
            _anchorOffsetProp = serializedObject.FindProperty("AnchorOffset");
            _springForceProp = serializedObject.FindProperty("SpringForce");
            _springDamperProp = serializedObject.FindProperty("DamperForce");
            _springTargetPositionProp = serializedObject.FindProperty("SpringTargetPosition");
            _springMinLimitAngleProp = serializedObject.FindProperty("MinLimitAngle");
            _springMaxLimitAngleProp = serializedObject.FindProperty("MaxLimitAngle");

            Transform = ((CreateSuspensionWheel)target).gameObject.transform;

            AttachWheelManager();
        }
        
        protected override void SetUpGUI()
        {
            GUIUtils.HeaderGUI(WheelUtilsMessages.Settings);
            GUIUtils.PropFieldGUI(_showLabelsProp);

            GUIUtils.HeaderGUI(WheelUtilsMessages.GeneralSettings);
            GUIUtils.PropFieldGUI(_componentCountProp, WheelUtilsMessages.Count);
            GUIUtils.SliderGUI(_componentSpacingProp, 0.1f, 5f, WheelUtilsMessages.Spacing);
            GUIUtils.SliderGUI(_componentDistanceProp, 0.1f, 5f, WheelUtilsMessages.Distance);

            GUIUtils.HeaderGUI(WheelUtilsMessages.WheelSettings);
            GUIUtils.PropFieldGUI(_wheelEulerRotationProp, WheelUtilsMessages.EulerRotation);
            GUIUtils.SliderGUI(_wheelColliderRadiusProp, 0.1f, 5f, WheelUtilsMessages.ColliderRadius);
            GUIUtils.PropFieldGUI(_wheelColliderMaterialProp, WheelUtilsMessages.ColliderMaterial);
            GUIUtils.PropFieldGUI(_wheelMeshProp, WheelUtilsMessages.Mesh);
            GUIUtils.PropFieldGUI(_wheelMaterialProp, WheelUtilsMessages.Material);
            GUIUtils.PropFieldGUI(_wheelMassProp, WheelUtilsMessages.Mass);
            GUIUtils.SliderGUI(_wheelOffsetProp, -1f, 1f, WheelUtilsMessages.Offset);
            GUIUtils.SliderGUI(_wheelDistanceProp, 0f, 5f, WheelUtilsMessages.Distance);
            GUIUtils.SliderGUI(_wheelSpacingProp, 0f, 5f, WheelUtilsMessages.Spacing);

            GUIUtils.HeaderGUI(WheelUtilsMessages.SuspensionSettings);
            GUIUtils.PropFieldGUI(_suspensionEulerRotationProp, WheelUtilsMessages.EulerRotation);
            GUIUtils.PropFieldGUI(_suspensionMeshProp, WheelUtilsMessages.Mesh);
            GUIUtils.PropFieldGUI(_suspensionMaterialProp, WheelUtilsMessages.Material);
            GUIUtils.SliderGUI(_suspensionDistanceProp, 0f, 5f, WheelUtilsMessages.Distance);
            GUIUtils.SliderGUI(_suspensionSpacingProp, 0f, 5f, WheelUtilsMessages.Spacing);
            GUIUtils.PropFieldGUI(_anchorOffsetProp);
            GUIUtils.PropFieldGUI(_springForceProp);
            GUIUtils.PropFieldGUI(_springDamperProp);
            GUIUtils.PropFieldGUI(_suspensionMassProp, WheelUtilsMessages.Mass);
            GUIUtils.SliderGUI(_springTargetPositionProp, -90, 90, WheelUtilsMessages.SpringTargetPosition);
            MaxAngelSliderGUI(_springMaxLimitAngleProp, -90, 90, WheelUtilsMessages.SpringMaxAngle);
            MinAngleSliderGUI(_springMinLimitAngleProp, -90, 90, WheelUtilsMessages.SpringMinAngle);

            UpdateAllGUI();
        }
        
        private void MinAngleSliderGUI(SerializedProperty property, float leftVal, float rightVal, string label)
        {
            EditorGUI.BeginChangeCheck();
            var value = EditorGUILayout.Slider(label, property.floatValue, leftVal, rightVal);
            if (EditorGUI.EndChangeCheck())
                property.floatValue = value <= _springMaxLimitAngleProp.floatValue ? value : _springMaxLimitAngleProp.floatValue;
        }
        
        private void MaxAngelSliderGUI(SerializedProperty property, float leftVal, float rightVal, string label)
        {
            EditorGUI.BeginChangeCheck();
            var value = EditorGUILayout.Slider(label, property.floatValue, leftVal, rightVal);
            if (EditorGUI.EndChangeCheck())
                property.floatValue = value >= _springMinLimitAngleProp.floatValue ? value : _springMinLimitAngleProp.floatValue;
        }

        private void OnSceneGUI()
        {
            for (int i = 0 ; i < Transform.childCount ; i++)
            {
                GameObject suspension = Transform.GetChild(i).gameObject;
                if (!suspension.name.Contains(WheelUtilsMessages.LeftSuspension) 
                    && !suspension.name.Contains(WheelUtilsMessages.RightSuspension))
                    continue;
                
                HingeJoint hingeJoint = suspension.GetComponent<HingeJoint>();
                var limits = hingeJoint.limits;
                
                Vector3 center = hingeJoint.connectedAnchor + hingeJoint.connectedBody.transform.position;
                Vector3 normal = hingeJoint.axis;
                float maxAngle = limits.max  * Mathf.Deg2Rad;
                float springAngle = hingeJoint.spring.targetPosition * Mathf.Deg2Rad;
                Vector3 from = new Vector3(Mathf.Cos(maxAngle), Mathf.Sin(maxAngle),0);
                Vector3 targetPosition = new Vector3(Mathf.Cos(springAngle), Mathf.Sin(springAngle),0).normalized;
                float angle = limits.min - limits.max;
                float radius = 0.1f;
                
                WheelsUtils.DrawArc(
                    center,
                    normal,
                    from,
                    angle,
                    radius,
                    WheelUtilsMessages.LightRed
                );
                
                WheelsUtils.DrawLine(
                    center,
                    center + targetPosition * radius,
                    WheelUtilsMessages.Green
                );
                
                WheelsUtils.DrawLine(
                    center,
                    center + normal * radius,
                    WheelUtilsMessages.Orange,
                    3
                );
            }
        }

        protected override void BulkUpdateComponents()
        {
            BulkDestroyComponents();
            BulkCreateComponents();
            
            UpdateAll = false;
        }

        private void BulkCreateComponents()
        {
            for (int i = 0; i < _componentCountProp.intValue; i++)
            {
                GameObject leftSuspension = CreateSuspension(true, i);
                CreateWheel(true, i, leftSuspension);
                
                GameObject rightSuspension = CreateSuspension(false, i);
                CreateWheel(false, i, rightSuspension);
            }
        }
        
        private void CreateWheel(bool isLeft, int i, GameObject suspension)
        {
            string wheelName = isLeft ? WheelUtilsMessages.LeftWheel : WheelUtilsMessages.RightWheel;
            float wheelDistance = _wheelDistanceProp.floatValue + _componentDistanceProp.floatValue;
            wheelDistance = isLeft ? wheelDistance : -wheelDistance;
            float wheelSpacing = i * (_wheelSpacingProp.floatValue + _componentSpacingProp.floatValue) + _wheelOffsetProp.floatValue;
            Vector3 eulerRotation = _wheelEulerRotationProp.vector3Value;
            var wheel = new GameObject(wheelName + i)
            {
                transform =
                {
                    parent = Transform,
                    localPosition = new Vector3(wheelSpacing, 0, wheelDistance),
                    localRotation = Quaternion.Euler(eulerRotation)
                },
            };
            
            WheelsUtils.ShowLabel(wheel, _showLabelsProp);
            
            AttachCollider(wheel, _wheelColliderRadiusProp, _wheelColliderMaterialProp);
            AttachRigidbody(wheel, _wheelMassProp);
            AttachWheelHingeJoint(wheel, suspension);
            AttachWheelScript(wheel, isLeft);
            AttachMesh(wheel, _wheelMeshProp, _wheelMaterialProp);
        }
        
        private GameObject CreateSuspension(bool isLeft, int i)
        {
            string suspensionName = isLeft ? WheelUtilsMessages.LeftSuspension : WheelUtilsMessages.RightSuspension;
            float suspensionDistance = _suspensionDistanceProp.floatValue + _componentDistanceProp.floatValue;
            suspensionDistance = isLeft ? suspensionDistance : -suspensionDistance;
            float suspensionSpacing = _suspensionSpacingProp.floatValue + _componentSpacingProp.floatValue;
            Vector3 eulerRotation = _suspensionEulerRotationProp.vector3Value;
            
            var suspension = new GameObject(suspensionName + i)
            {
                transform =
                {
                    parent = Transform,
                    localPosition = new Vector3(i * suspensionSpacing, 0, suspensionDistance),
                    localRotation = Quaternion.Euler(eulerRotation)
                },
            };
            
            WheelsUtils.ShowLabel(suspension, _showLabelsProp);
            
            AttachRigidbody(suspension, _suspensionMassProp);
            AttachSuspensionHingeJoint(suspension);
            AttachMesh(suspension, _suspensionMeshProp, _suspensionMaterialProp);

            return suspension;
        }
        
        private void AttachWheelHingeJoint(GameObject wheel, GameObject suspension)
        {
            HingeJoint wheelHingeJoint = wheel.AddComponent<HingeJoint>();
            wheelHingeJoint.anchor = Vector3.zero;
            wheelHingeJoint.axis = Vector3.forward;
            wheelHingeJoint.useSpring = false;
            wheelHingeJoint.useMotor = false;
            wheelHingeJoint.useLimits = false;
            wheelHingeJoint.connectedBody = suspension.GetComponent<Rigidbody>();
        }

        private void AttachSuspensionHingeJoint(GameObject suspension)
        {
            HingeJoint suspensionHingeJoint = suspension.AddComponent<HingeJoint>();
            suspensionHingeJoint.anchor = _anchorOffsetProp.vector3Value;
            suspensionHingeJoint.axis = Vector3.forward;
            suspensionHingeJoint.useSpring = true;
            suspensionHingeJoint.spring = new JointSpring
            {
                spring = _springForceProp.floatValue,
                damper = _springDamperProp.floatValue,
                targetPosition = _springTargetPositionProp.floatValue
            };
            suspensionHingeJoint.useLimits = true;
            suspensionHingeJoint.limits = new JointLimits
            {
                min = _springMinLimitAngleProp.floatValue,
                max = _springMaxLimitAngleProp.floatValue
            };
            suspensionHingeJoint.connectedBody = Transform.parent.GetComponent<Rigidbody>();
        }
    }
}