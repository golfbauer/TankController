using System;
using Controller.Scripts.Editors.Wheels.SuspensionWheel;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Controller.Scripts.Editors.Wheels.Chain
{
    [CustomEditor(typeof(CreateChain))]
    [CanEditMultipleObjects]
    public class CreateChainEditor : Editor
    {
        private SerializedProperty _showLabelsProp;
        
        private SerializedProperty _leftMesh;
        private SerializedProperty _leftMaterial;
        
        private SerializedProperty _rightMesh;
        private SerializedProperty _rightMaterial;
        
        private SerializedProperty _chainMass;
        
        private SerializedProperty _chainDistance;
        private SerializedProperty _chainSpacing;
        
        private SerializedProperty _chainStraightCount;
        private SerializedProperty _chainFrontCurveCount;
        private SerializedProperty _chainBackCurveCount;
        
        private Transform Transform;
        public bool UpdateAll;

        private void OnEnable()
        {
            _showLabelsProp = serializedObject.FindProperty("showLabels");
            
            _leftMesh = serializedObject.FindProperty("leftMesh");
            _leftMaterial = serializedObject.FindProperty("leftMaterial");
            
            _rightMesh = serializedObject.FindProperty("rightMesh");
            _rightMaterial = serializedObject.FindProperty("rightMaterial");
            
            _chainMass = serializedObject.FindProperty("chainMass");
            
            _chainDistance = serializedObject.FindProperty("chainDistance");
            _chainSpacing = serializedObject.FindProperty("chainSpacing");
            
            _chainStraightCount = serializedObject.FindProperty("chainStraightCount");
            _chainFrontCurveCount = serializedObject.FindProperty("chainFrontCurveCount");
            _chainBackCurveCount = serializedObject.FindProperty("chainBackCurveCount");
            
            Transform = ((CreateChain) target).transform;
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

            if (GUI.changed || Event.current.commandName == "UndoRedoPerformed")
            {
                UpdateAll = true;
            }

            if (UpdateAll)
            {
                BulkUpdateComponents();
                serializedObject.ApplyModifiedProperties();
            }
        }

        protected virtual void SetUpGUI()
        {
            GUIUtils.HeaderGUI("General");
            GUIUtils.PropFieldGUI(_showLabelsProp, "Show Labels");
            
            GUIUtils.HeaderGUI("Chain");
            GUIUtils.PropFieldGUI(_leftMesh, "Left Mesh");
            GUIUtils.PropFieldGUI(_leftMaterial, "Left Material");
            
            GUIUtils.PropFieldGUI(_rightMesh, "Right Mesh");
            GUIUtils.PropFieldGUI(_rightMaterial, "Right Material");
            
            GUIUtils.PropFieldGUI(_chainMass, "Chain Mass");
            
            GUIUtils.SliderGUI(_chainDistance, 0.1f, 5f, "Distance");
            GUIUtils.SliderGUI(_chainSpacing, 0.1f, 5f, "Spacing");
            
            GUIUtils.IntSliderGUI(_chainStraightCount, 0, 100, "Straight Count");
            GUIUtils.IntSliderGUI(_chainFrontCurveCount, 0, 100, "Front Curve Count");
            GUIUtils.IntSliderGUI(_chainBackCurveCount, 0, 100, "Back Curve Count");
        }
        
        private void BulkUpdateComponents()
        {
            BulkDestroyComponents();
            BulkCreateComponents();
            UpdateAll = false;
        }
        
        private void BulkDestroyComponents()
        {
            var childCount = Transform.childCount;
            for (var i = 0; i < childCount; i++)
                DestroyImmediate (Transform.GetChild(0).gameObject);
        }
        
        private void BulkCreateComponents()
        {
            CreateCurves();
            CreateStraight();
        }
        
        private void CreateCurves()
        {
            if (_chainFrontCurveCount.intValue == 0) return;
            
            float straightDistance = _chainStraightCount.intValue * _chainSpacing.floatValue / 2;
            
            // Front Left Curve
            CreateCurve(straightDistance, _chainDistance.floatValue / 2, _chainFrontCurveCount.intValue, 180, true);
                
            // Front Right Curve
            CreateCurve(straightDistance, -_chainDistance.floatValue / 2, _chainFrontCurveCount.intValue, 180f, false);
            
            // Back Left Curve
            CreateCurve(-straightDistance, _chainDistance.floatValue / 2, _chainBackCurveCount.intValue, 0f, true);
            
            // Back Right Curve
            CreateCurve(-straightDistance, -_chainDistance.floatValue / 2, _chainBackCurveCount.intValue,0f, false);
        }

        private void CreateCurve(float moveX, float moveZ, int curveCount, float startingAngle, bool isLeft)
        {
            float circumference = _chainSpacing.floatValue * curveCount * 2;
            float radius = circumference / (2 * Mathf.PI);
            
            float angle = 180f / (curveCount - 1);
            Vector3 startingVector = Vector3.up * radius;

            for (int i = 0; i < curveCount; i++)
            {
                float currentAngle = i * angle + startingAngle;
                float x = startingVector.x * Mathf.Cos(currentAngle * Mathf.Deg2Rad) - startingVector.y * Mathf.Sin( currentAngle * Mathf.Deg2Rad);
                float y = startingVector.x * Mathf.Sin(currentAngle * Mathf.Deg2Rad) + startingVector.y * Mathf.Cos(currentAngle * Mathf.Deg2Rad);

                Vector3 pivot = new Vector3(x + moveX, y, moveZ);

                CreateChainLink(pivot, i, isLeft);
            }
        }

        private void CreateStraight()
        {
            
        }

        private void CreateChainLink(Vector3 pivot, int i, bool isLeft = false)
        {
            string name = isLeft ? "L Link " : "R Link ";
            var chainLink = new GameObject(name + i)
            {
                transform =
                {
                    parent = Transform,
                    localPosition = pivot,
                },
            };
            WheelsUtils.ShowLabel(chainLink, _showLabelsProp);
        }

        private void OnSceneGUI()
        {
            for (int i = 0; i < Transform.childCount; i++)
            {
                GameObject chainLink = Transform.GetChild(i).gameObject;
                
                WheelsUtils.DrawLine(
                    Transform.position,
                    Transform.position + chainLink.transform.localPosition,
                    Color.green
                );
            }
        }
    }
}