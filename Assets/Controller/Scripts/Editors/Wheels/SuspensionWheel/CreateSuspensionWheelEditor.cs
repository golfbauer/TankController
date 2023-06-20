using Controller.Scripts.Editors.Utils;
using Controller.Scripts.Managers.Wheels;
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
        // Right Wheel Rotation
        private SerializedProperty _rightWheelRotationProp;
        private SerializedProperty _rightTorqueDirectionProp;
        private SerializedProperty _rightWheelAxisProp;
        
        // Left Wheel Rotation
        private SerializedProperty _leftWheelRotationProp;
        private SerializedProperty _leftTorqueDirectionProp;
        private SerializedProperty _leftWheelAxisProp;
        
        // Right Wheel Mesh Settings
        private SerializedProperty _leftWheelMeshProp;
        private SerializedProperty _rightWheelMaterialProp;
        
        // Left Wheel Mesh Settings
        private SerializedProperty _rightWheelMeshProp;
        private SerializedProperty _leftWheelMaterialProp;

        // Wheel Collider Settings
        private SerializedProperty _wheelColliderRadiusProp;
        private SerializedProperty _wheelColliderMaterialProp;

        // Wheel Spacing Settings
        private SerializedProperty _wheelDistanceProp;
        private SerializedProperty _wheelSpacingProp;
        private SerializedProperty _wheelOffsetProp;
        
        // Wheel Rigidbody Settings
        private SerializedProperty _wheelMassProp;


        // Suspension Settings
        // Rotation
        private SerializedProperty _suspensionRotationProp;
        
        // Right Suspension Mesh Settings
        private SerializedProperty _rightSuspensionMeshProp;
        private SerializedProperty _rightSuspensionMaterialProp;
        
        // Left Suspension Mesh Settings
        private SerializedProperty _leftSuspensionMeshProp;
        private SerializedProperty _leftSuspensionMaterialProp;
        
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
        
        private SerializedProperty _resizeWheelProp;
        private SerializedProperty _wheelResizeScaleProp;
        private SerializedProperty _wheelResizeSpeedProp;

        private void OnEnable()
        {
            _showLabelsProp = serializedObject.FindProperty("showLabels");
            
            
            _componentCountProp = serializedObject.FindProperty("componentCount");
            _componentSpacingProp = serializedObject.FindProperty("componentSpacing");
            _componentDistanceProp = serializedObject.FindProperty("componentDistance");

            
            _leftWheelRotationProp = serializedObject.FindProperty("leftWheelRotation");
            _leftTorqueDirectionProp = serializedObject.FindProperty("leftWheelTorque");
            _leftWheelAxisProp = serializedObject.FindProperty("leftWheelHingeAxis");
            
            _rightWheelRotationProp = serializedObject.FindProperty("rightWheelRotation");
            _rightTorqueDirectionProp = serializedObject.FindProperty("rightWheelTorque");
            _rightWheelAxisProp = serializedObject.FindProperty("rightWheelHingeAxis");
            
            _leftWheelMeshProp = serializedObject.FindProperty("leftWheelMesh");
            _leftWheelMaterialProp = serializedObject.FindProperty("leftWheelMaterials");
            
            _rightWheelMeshProp = serializedObject.FindProperty("rightWheelMesh");
            _rightWheelMaterialProp = serializedObject.FindProperty("rightWheelMaterials");

            _wheelColliderRadiusProp = serializedObject.FindProperty("wheelColliderRadius");
            _wheelColliderMaterialProp = serializedObject.FindProperty("wheelColliderMaterial");

            _wheelOffsetProp = serializedObject.FindProperty("wheelOffset");
            _wheelDistanceProp = serializedObject.FindProperty("wheelDistance");
            _wheelSpacingProp = serializedObject.FindProperty("wheelSpacing");
            
            _wheelMassProp = serializedObject.FindProperty("wheelMass");
            
            
            _suspensionRotationProp = serializedObject.FindProperty("suspensionRotation");
            
            _leftSuspensionMeshProp = serializedObject.FindProperty("leftSuspensionMesh");
            _leftSuspensionMaterialProp = serializedObject.FindProperty("leftSuspensionMaterials");
            
            _rightSuspensionMeshProp = serializedObject.FindProperty("rightSuspensionMesh");
            _rightSuspensionMaterialProp = serializedObject.FindProperty("rightSuspensionMaterials");

            _suspensionDistanceProp = serializedObject.FindProperty("suspensionDistance");
            _suspensionSpacingProp = serializedObject.FindProperty("suspensionSpacing");
            
            _suspensionMassProp = serializedObject.FindProperty("suspensionMass");
            
            _anchorOffsetProp = serializedObject.FindProperty("anchorOffset");
            _springForceProp = serializedObject.FindProperty("springForce");
            _springDamperProp = serializedObject.FindProperty("damperForce");
            _springTargetPositionProp = serializedObject.FindProperty("springTargetPosition");
            _springMinLimitAngleProp = serializedObject.FindProperty("minLimitAngle");
            _springMaxLimitAngleProp = serializedObject.FindProperty("maxLimitAngle");
            
            _resizeWheelProp = serializedObject.FindProperty("resizeWheel");
            _wheelResizeScaleProp = serializedObject.FindProperty("wheelResizeScale");
            _wheelResizeSpeedProp = serializedObject.FindProperty("wheelResizeSpeed");

            transform = ((CreateSuspensionWheel)target).gameObject.transform;

            AttachWheelManager(_wheelColliderRadiusProp.floatValue);
        }
        
        public override void SetUpGUI()
        {
            GUIUtils.HeaderGUI(WheelMessages.GeneralSettings);
            GUIUtils.PropFieldGUI(_showLabelsProp);
            
            
            GUIUtils.HeaderGUI(WheelMessages.GeneralSettings);
            GUIUtils.PropFieldGUI(_componentCountProp, WheelMessages.Count);
            GUIUtils.SliderGUI(_componentSpacingProp, 0.1f, 5f, WheelMessages.Spacing);
            GUIUtils.SliderGUI(_componentDistanceProp, 0.1f, 5f, WheelMessages.Distance);
            
            
            GUIUtils.HeaderGUI(WheelMessages.RightWheelSettings);
            GUIUtils.PropFieldGUI(_rightWheelRotationProp, WheelMessages.EulerRotation);
            GUIUtils.PropFieldGUI(_rightTorqueDirectionProp, WheelMessages.TorqueDirection);
            GUIUtils.PropFieldGUI(_rightWheelAxisProp, WheelMessages.HingeAxis);
            GUIUtils.PropFieldGUI(_rightWheelMeshProp, WheelMessages.Mesh);
            GUIUtils.PropFieldGUI(_rightWheelMaterialProp, WheelMessages.Material);
            
            GUIUtils.HeaderGUI(WheelMessages.LeftWheelSettings);
            GUIUtils.PropFieldGUI(_leftWheelRotationProp, WheelMessages.EulerRotation);
            GUIUtils.PropFieldGUI(_leftTorqueDirectionProp, WheelMessages.TorqueDirection);
            GUIUtils.PropFieldGUI(_leftWheelAxisProp, WheelMessages.HingeAxis);
            GUIUtils.PropFieldGUI(_leftWheelMeshProp, WheelMessages.Mesh);
            GUIUtils.PropFieldGUI(_leftWheelMaterialProp, WheelMessages.Material);

            GUIUtils.HeaderGUI(WheelMessages.GeneralWheelSettings);
            GUIUtils.PropFieldGUI(_wheelMassProp, WheelMessages.Mass);
            
            GUIUtils.SliderGUI(_wheelOffsetProp, -1f, 1f, WheelMessages.Offset);
            GUIUtils.SliderGUI(_wheelDistanceProp, 0f, 5f, WheelMessages.Distance);
            GUIUtils.SliderGUI(_wheelSpacingProp, 0f, 5f, WheelMessages.Spacing);
            
            GUIUtils.PropFieldGUI(_wheelColliderMaterialProp, WheelMessages.ColliderMaterial);
            GUIUtils.SliderGUI(_wheelColliderRadiusProp, 0.1f, 5f, WheelMessages.ColliderRadius);
            

            GUIUtils.HeaderGUI(WheelMessages.LeftSuspensionSettings);
            GUIUtils.PropFieldGUI(_leftSuspensionMeshProp, WheelMessages.Mesh);
            GUIUtils.PropFieldGUI(_leftSuspensionMaterialProp, WheelMessages.Material);
            
            GUIUtils.HeaderGUI(WheelMessages.RightSuspensionSettings);
            GUIUtils.PropFieldGUI(_rightSuspensionMeshProp, WheelMessages.Mesh);
            GUIUtils.PropFieldGUI(_rightSuspensionMaterialProp, WheelMessages.Material);
            
            GUIUtils.HeaderGUI(WheelMessages.GeneralSuspensionSettings);
            GUIUtils.SliderGUI(_suspensionRotationProp, -180, 180, WheelMessages.EulerRotation);
            GUIUtils.SliderGUI(_suspensionDistanceProp, 0f, 5f, WheelMessages.Distance);
            GUIUtils.SliderGUI(_suspensionSpacingProp, 0f, 5f, WheelMessages.Spacing);
            GUIUtils.PropFieldGUI(_suspensionMassProp, WheelMessages.Mass);
            GUIUtils.PropFieldGUI(_anchorOffsetProp);
            GUIUtils.PropFieldGUI(_springForceProp);
            GUIUtils.PropFieldGUI(_springDamperProp);
            GUIUtils.SliderGUI(_springTargetPositionProp, -90, 90, WheelMessages.SpringTargetPosition);
            MaxAngelSliderGUI(_springMaxLimitAngleProp, -90, 90, WheelMessages.SpringMaxAngle);
            MinAngleSliderGUI(_springMinLimitAngleProp, -90, 90, WheelMessages.SpringMinAngle);
            
            GUIUtils.PropFieldGUI(_resizeWheelProp, WheelMessages.Resize);
            if (_resizeWheelProp.boolValue)
            {
                GUIUtils.SliderGUI(_wheelResizeScaleProp, 0.1f, 1f, WheelMessages.ResizeScale);
                GUIUtils.PropFieldGUI(_wheelResizeSpeedProp, WheelMessages.ResizeSpeed);
            }

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

        protected override void OnSceneGUI()
        {
            base.OnSceneGUI();
            
            for (int i = 0 ; i < transform.childCount ; i++)
            {
                GameObject suspension = transform.GetChild(i).gameObject;
                if (!suspension.name.Contains(WheelMessages.LeftSuspension) 
                    && !suspension.name.Contains(WheelMessages.RightSuspension))
                    continue;
                
                HingeJoint hingeJoint = suspension.GetComponent<HingeJoint>();
                var limits = hingeJoint.limits;
                
                Vector3 center = hingeJoint.connectedAnchor + hingeJoint.connectedBody.transform.position;
                Vector3 normal = hingeJoint.axis;
                
                // This is dependent on the axis direction -> if the axis is not pointing in X dir this will be broken
                float minRadAngle = -limits.min * Mathf.Deg2Rad;
                Vector3 from = new Vector3(0,Mathf.Sin(minRadAngle), Mathf.Cos(minRadAngle));
                float angle = limits.max - limits.min;
                
                float springAngle = hingeJoint.spring.targetPosition * Mathf.Deg2Rad;
                Vector3 targetPosition = new Vector3(0, Mathf.Sin(springAngle), Mathf.Cos(springAngle)).normalized;
                targetPosition.y *= -1;
                
                float radius = 0.1f;
                
                DrawUtils.DrawArc(
                    center,
                    normal,
                    from,
                    angle,
                    radius,
                    GeneralMessages.LightRed
                );
                
                DrawUtils.DrawLine(
                    center,
                    center + targetPosition * radius,
                    WheelMessages.Green
                );
                
                DrawUtils.DrawLine(
                    center,
                    center + normal * radius,
                    GeneralMessages.Orange,
                    3
                );
            }
        }

        public override void BulkUpdateComponents()
        {
            BulkDestroyComponents();
            BulkCreateComponents();
        }

        private void BulkCreateComponents()
        {
            for (int i = 0; i < _componentCountProp.intValue; i++)
            {
                GameObject leftSuspension = CreateSuspension(true, i);
                GameObject wheel = CreateWheel(true, i, leftSuspension);
                
                RotateObjectsAroundPivot(leftSuspension.transform, wheel.transform, Vector3.right, _suspensionRotationProp.floatValue);
                
                GameObject rightSuspension = CreateSuspension(false, i);
                wheel = CreateWheel(false, i, rightSuspension);
                
                RotateObjectsAroundPivot(rightSuspension.transform, wheel.transform, Vector3.right, _suspensionRotationProp.floatValue);
            }
        }
        
        private GameObject CreateWheel(bool isLeft, int i, GameObject suspension)
        {
            string wheelName = isLeft ? WheelMessages.LeftWheel : WheelMessages.RightWheel;
            float wheelDistance = _wheelDistanceProp.floatValue + _componentDistanceProp.floatValue;
            wheelDistance = isLeft ? -wheelDistance : wheelDistance;
            float wheelSpacing = i * (_wheelSpacingProp.floatValue + _componentSpacingProp.floatValue) + _wheelOffsetProp.floatValue;
            Vector3 eulerRotation = isLeft ? _leftWheelRotationProp.vector3Value : _rightWheelRotationProp.vector3Value;
            var wheel = new GameObject(wheelName + i)
            {
                transform =
                {
                    parent = transform,
                    localPosition = new Vector3(wheelDistance, 0, wheelSpacing),
                    localRotation = Quaternion.Euler(eulerRotation)
                },
            };
            
            DrawUtils.ShowLabel(wheel, _showLabelsProp);
            
            Vector3 torqueDirection = isLeft ? _leftTorqueDirectionProp.vector3Value : _rightTorqueDirectionProp.vector3Value;
            Vector3 hingeAxis = isLeft ? _leftWheelAxisProp.vector3Value : _rightWheelAxisProp.vector3Value;
            
            UpdateSphereCollider(wheel.transform, _wheelColliderRadiusProp, _wheelColliderMaterialProp);
            UpdateRigidbody(wheel.transform, _wheelMassProp);
            AttachWheelHingeJoint(wheel, suspension.transform, hingeAxis);
            AttachWheelScript(wheel, isLeft, torqueDirection);
            UpdateMesh(
                wheel.transform, 
                isLeft ? _leftWheelMeshProp : _rightWheelMeshProp, 
                isLeft ? _leftWheelMaterialProp : _rightWheelMaterialProp
                );
            AttachWheelResizeScript(wheel, _resizeWheelProp.boolValue, _wheelResizeScaleProp.floatValue, _wheelResizeSpeedProp.floatValue);
            
            SetLayers();

            return wheel;

        }

        private GameObject CreateSuspension(bool isLeft, int i)
        {
            string suspensionName = isLeft ? WheelMessages.LeftSuspension : WheelMessages.RightSuspension;
            float suspensionDistance = _suspensionDistanceProp.floatValue + _componentDistanceProp.floatValue;
            suspensionDistance = isLeft ? -suspensionDistance : suspensionDistance;
            float suspensionSpacing = _suspensionSpacingProp.floatValue + _componentSpacingProp.floatValue;
            
            var suspension = new GameObject(suspensionName + i)
            {
                transform =
                {
                    parent = transform,
                    localPosition = new Vector3(suspensionDistance, 0, i * suspensionSpacing),
                },
            };
            
            DrawUtils.ShowLabel(suspension, _showLabelsProp);
            
            UpdateRigidbody(suspension.transform, _suspensionMassProp);
            AttachSuspensionHingeJoint(suspension);
            UpdateMesh(
                suspension.transform, 
                isLeft ? _leftSuspensionMeshProp : _rightSuspensionMeshProp, 
                isLeft ? _leftSuspensionMaterialProp : _rightSuspensionMaterialProp
                );

            return suspension;
        }

        private void AttachSuspensionHingeJoint(GameObject suspension)
        {
            HingeJoint suspensionHingeJoint = suspension.AddComponent<HingeJoint>();
            suspensionHingeJoint.anchor = _anchorOffsetProp.vector3Value;
            suspensionHingeJoint.axis = Vector3.right;
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
            suspensionHingeJoint.connectedBody = transform.parent.GetComponent<Rigidbody>();
        }
        
        public static void RotateObjectsAroundPivot(Transform pivot, Transform objectToRotate, Vector3 rotationAxis, float rotationAngle)
        {
            Quaternion pivotRotation = Quaternion.AngleAxis(rotationAngle, rotationAxis);
            Vector3 pivotToWheel = objectToRotate.position - pivot.position;
    
            pivot.rotation *= pivotRotation;
            objectToRotate.rotation *= pivotRotation;

            Vector3 newPivotToWheel = pivotRotation * pivotToWheel;
            objectToRotate.position = pivot.position + newPivotToWheel;
        }
    }
}