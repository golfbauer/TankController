using System;
using Controller.Scripts.Editors.Utils;
using Controller.Scripts.Managers.Movement;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace Controller.Scripts.Editors.Movement
{
    [CustomEditor(typeof(MovementManager))]
    [CanEditMultipleObjects]
    public class MovementManagerEditor : TankComponentEditor
    {
        private SerializedProperty _inputType;
        
        private SerializedProperty _steeringMode;
        
        private SerializedProperty _allowPivotSteering;
        private SerializedProperty _maxPivotSpeed;
        private SerializedProperty _pivotTime;
        private SerializedProperty _pivotCurve;
        
        private SerializedProperty _torque;
        
        private SerializedProperty _turningDrag;
        private SerializedProperty _minTurningDrag;
        private SerializedProperty _breakDrag;
        private SerializedProperty _breakDecelerationRate;
        private SerializedProperty _rollingDrag;
        
        private SerializedProperty _maxForwardSpeed;
        private SerializedProperty _maxReverseSpeed;
        
        private SerializedProperty _accelerationTime;
        private SerializedProperty _accelerationCurve;
        
        private SerializedProperty _decelerationTime;
        private SerializedProperty _decelerationCurve;

        private void OnEnable()
        {
            _inputType = serializedObject.FindProperty("inputType");
            
            _steeringMode = serializedObject.FindProperty("steeringMode");
            
            _allowPivotSteering = serializedObject.FindProperty("allowPivotSteering");
            _maxPivotSpeed = serializedObject.FindProperty("maxPivotSpeed");
            _pivotTime = serializedObject.FindProperty("pivotTime");
            _pivotCurve = serializedObject.FindProperty("pivotCurve");
            
            _torque = serializedObject.FindProperty("torque");
            
            _turningDrag = serializedObject.FindProperty("turningDrag");
            _minTurningDrag = serializedObject.FindProperty("minTurningDrag");
            _breakDrag = serializedObject.FindProperty("breakDrag");
            _breakDecelerationRate = serializedObject.FindProperty("breakDecelerationRate");
            _rollingDrag = serializedObject.FindProperty("rollingDrag");
            
            _maxForwardSpeed = serializedObject.FindProperty("maxForwardSpeed");
            _maxReverseSpeed = serializedObject.FindProperty("maxReverseSpeed");

            _accelerationTime = serializedObject.FindProperty("accelerationTime");
            _accelerationCurve = serializedObject.FindProperty("accelerationCurve");
            
            _decelerationTime = serializedObject.FindProperty("decelerationTime");
            _decelerationCurve = serializedObject.FindProperty("decelerationCurve");
            
            transform = ((MovementManager) target).gameObject.transform;
        }
        
        public override void SetUpGUI()
        {
            EditorGUILayout.PropertyField(_inputType);
            EditorGUILayout.PropertyField(_steeringMode);
            if (_steeringMode.enumValueIndex == (int)TankSteeringMode.TrackAccelerationAndDeceleration)
            {
                EditorGUILayout.PropertyField(_allowPivotSteering);
                if (_allowPivotSteering.boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(_maxPivotSpeed);
                    EditorGUILayout.PropertyField(_pivotTime);
                    serializedObject.ApplyModifiedProperties();
                    ShowCurve(_pivotCurve, "Pivot Curve");
                    EditorGUI.indentLevel--;
                }
            }
            
            EditorGUILayout.Space();
            GUIUtils.HeaderGUI("Break");
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_breakDrag);
            EditorGUILayout.PropertyField(_breakDecelerationRate);
            EditorGUI.indentLevel--;
            
            EditorGUILayout.Space();
            GUIUtils.HeaderGUI("Movement");
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_torque);
            EditorGUILayout.PropertyField(_rollingDrag);
            EditorGUILayout.PropertyField(_maxForwardSpeed);
            EditorGUILayout.PropertyField(_maxReverseSpeed);
            serializedObject.ApplyModifiedProperties();
            
            EditorGUILayout.PropertyField(_accelerationTime);
            serializedObject.ApplyModifiedProperties();
            ShowCurve(_accelerationCurve, "Acceleration Curve");
            
            EditorGUILayout.PropertyField(_decelerationTime);
            serializedObject.ApplyModifiedProperties();
            ShowCurve(_decelerationCurve, "Deceleration Curve");
            EditorGUI.indentLevel--;
            
            EditorGUILayout.Space();
            GUIUtils.HeaderGUI("Turning");
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_turningDrag);
            EditorGUILayout.PropertyField(_minTurningDrag);
            EditorGUI.indentLevel--;
        }

        private void ShowCurve(SerializedProperty curveAnimation, string label = null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label);
            serializedObject.Update();
            AnimationCurve curve = curveAnimation.animationCurveValue;
            curve = EditorGUILayout.CurveField(curve);
            curveAnimation.animationCurveValue = curve;
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndHorizontal();
        }

        public override bool AllowAccess()
        {
            return false;
        }
    }
}