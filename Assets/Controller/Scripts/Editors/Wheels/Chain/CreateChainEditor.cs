using System;
using System.Collections.Generic;
using System.Linq;
using Controller.Scripts.Editors.Wheels.SuspensionWheel;
using Controller.Scripts.Managers.Tracks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Controller.Scripts.Editors.Wheels.Chain
{
    [CustomEditor(typeof(CreateChain))]
    [CanEditMultipleObjects]
    public class CreateChainEditor : CreateWheelEditor
    {
        private SerializedProperty _showLabelsProp;
        private SerializedProperty _showConnectionsProp;
        
        private SerializedProperty _leftMesh;
        private SerializedProperty _leftMaterial;
        
        private SerializedProperty _rightMesh;
        private SerializedProperty _rightMaterial;
        
        private SerializedProperty _boxColliderCenter;
        private SerializedProperty _boxColliderSize;
        private SerializedProperty _boxColliderMaterial;
        
        private SerializedProperty _chainMass;
        
        private SerializedProperty _chainDistance;
        private SerializedProperty _chainSpacing;
        
        private SerializedProperty _chainStraightCount;
        private SerializedProperty _chainFrontCurveCount;
        private SerializedProperty _chainBackCurveCount;

        private Vector3 _leftFrontCurveCenter;
        private Vector3 _leftBackCurveCenter;
        
        private Vector3 _rightBackCurveCenter;
        private Vector3 _rightFrontCurveCenter;

        private float _frontRadius;
        private float _backRadius;

        private float _straightUpperRotationAngle;
        private float _straightLowerRotationAngle;
        
        private float _actualStraightDistance;
        
        private List<GameObject> _leftChainLinks;
        private List<GameObject> _rightChainLinks;

        private void OnEnable()
        {
            _showLabelsProp = serializedObject.FindProperty("showLabels");
            _showConnectionsProp = serializedObject.FindProperty("showConnections");
            
            _leftMesh = serializedObject.FindProperty("leftMesh");
            _leftMaterial = serializedObject.FindProperty("leftMaterial");
            
            _rightMesh = serializedObject.FindProperty("rightMesh");
            _rightMaterial = serializedObject.FindProperty("rightMaterial");
            
            _boxColliderCenter = serializedObject.FindProperty("boxColliderCenter");
            _boxColliderSize = serializedObject.FindProperty("boxColliderSize");
            _boxColliderMaterial = serializedObject.FindProperty("boxColliderMaterial");
            
            _chainMass = serializedObject.FindProperty("chainMass");
            
            _chainDistance = serializedObject.FindProperty("chainDistance");
            _chainSpacing = serializedObject.FindProperty("chainSpacing");
            
            _chainStraightCount = serializedObject.FindProperty("chainStraightCount");
            _chainFrontCurveCount = serializedObject.FindProperty("chainFrontCurveCount");
            _chainBackCurveCount = serializedObject.FindProperty("chainBackCurveCount");
            
            Transform = ((CreateChain) target).transform;
            _leftChainLinks = new List<GameObject>();
            _rightChainLinks = new List<GameObject>();
        }

        protected override void SetUpGUI()
        {
            GUIUtils.HeaderGUI("General");
            GUIUtils.PropFieldGUI(_showLabelsProp, "Show Labels");
            GUIUtils.PropFieldGUI(_showConnectionsProp, "Show Connections");
            
            GUIUtils.HeaderGUI("Chain");
            GUIUtils.PropFieldGUI(_leftMesh, "Left Mesh");
            GUIUtils.PropFieldGUI(_leftMaterial, "Left Material");
            
            GUIUtils.PropFieldGUI(_boxColliderCenter, "Collider Center");
            GUIUtils.PropFieldGUI(_boxColliderSize, "Collider Size");
            GUIUtils.PropFieldGUI(_boxColliderMaterial, "Collider Material");
            
            GUIUtils.PropFieldGUI(_rightMesh, "Right Mesh");
            GUIUtils.PropFieldGUI(_rightMaterial, "Right Material");
            
            GUIUtils.PropFieldGUI(_chainMass, "Chain Mass");
            
            GUIUtils.SliderGUI(_chainDistance, 0.1f, 5f, "Distance");
            GUIUtils.SliderGUI(_chainSpacing, 0.01f, 1f, "Spacing");
            
            GUIUtils.IntSliderGUI(_chainStraightCount, 1, 100, "Straight Count");
            GUIUtils.IntSliderGUI(_chainFrontCurveCount, 1, 100, "Front Curve Count");
            GUIUtils.IntSliderGUI(_chainBackCurveCount, 1, 100, "Back Curve Count");
            
            UpdateAllGUI();
        }
        
        protected override void BulkUpdateComponents()
        {
            BulkDestroyComponents();
            BulkCreateComponents();
            UpdateAll = false;
        }

        private void BulkCreateComponents()
        {
            _leftChainLinks.Clear();
            _rightChainLinks.Clear();
            CalculateInitialValues();
            
            CreateLeftChainLinks();
            AttachHingeJoints(_leftChainLinks);
            
            CreateRightChainLinks();
            AttachHingeJoints(_rightChainLinks);
        }

        private void CalculateInitialValues()
        {
            float circumference = _chainSpacing.floatValue * _chainFrontCurveCount.intValue * 2;
            _frontRadius = circumference / (2 * Mathf.PI);
            
            circumference = _chainSpacing.floatValue * _chainBackCurveCount.intValue * 2;
            _backRadius = circumference / (2 * Mathf.PI);

            float heightDifference = Mathf.Abs(_frontRadius - _backRadius);
            float straightDistance = _chainStraightCount.intValue * _chainSpacing.floatValue;

            _actualStraightDistance = Mathf.Sqrt(Mathf.Pow(straightDistance, 2) - Mathf.Pow(heightDifference, 2));
            
            float straightRotationAngle = Mathf.Asin(Mathf.Abs(_frontRadius - _backRadius) / (_chainStraightCount.intValue *
                _chainSpacing.floatValue));
            straightRotationAngle *= Mathf.Rad2Deg;
            
            _straightUpperRotationAngle = _chainFrontCurveCount.intValue > _chainBackCurveCount.intValue ? straightRotationAngle : -straightRotationAngle;
            _straightLowerRotationAngle = _chainFrontCurveCount.intValue > _chainBackCurveCount.intValue ? -straightRotationAngle : straightRotationAngle;

            _leftFrontCurveCenter = new Vector3(_actualStraightDistance / 2, 0, _chainDistance.floatValue / 2);
            _leftBackCurveCenter = new Vector3(-_actualStraightDistance / 2, 0, _chainDistance.floatValue / 2);
            _rightFrontCurveCenter = new Vector3(_actualStraightDistance / 2, 0, -_chainDistance.floatValue / 2);
            _rightBackCurveCenter = new Vector3(-_actualStraightDistance / 2, 0, -_chainDistance.floatValue / 2);
        }

        private void CreateLeftChainLinks()
        {
            // Left Upper Straight
            Vector3 start = new Vector3(_leftFrontCurveCenter.x, _leftFrontCurveCenter.y + _frontRadius, _leftFrontCurveCenter.z);
            Vector3 end = new Vector3(_leftBackCurveCenter.x, _leftBackCurveCenter.y + _backRadius, _leftBackCurveCenter.z);
            CreateStraight(start, end, true, true, _straightUpperRotationAngle, _leftChainLinks);
            
            // Back Left Curve
            CreateCurve(_leftBackCurveCenter, _chainBackCurveCount.intValue, 0f, true, _leftChainLinks);
            
            // Left Lower Straight
            start.y = -_frontRadius;
            end.y = -_backRadius;
            CreateStraight(end, start , true, false, _straightLowerRotationAngle, _leftChainLinks);
            
            // Front Left Curve
            CreateCurve(_leftFrontCurveCenter, _chainFrontCurveCount.intValue, 180f, true, _leftChainLinks);
        }
        
        private void CreateRightChainLinks()
        {
            // Right Upper Straight
            Vector3 start = new Vector3(_rightFrontCurveCenter.x, _rightFrontCurveCenter.y + _frontRadius, _rightFrontCurveCenter.z);
            Vector3 end = new Vector3(_rightBackCurveCenter.x, _rightBackCurveCenter.y + _backRadius, _rightBackCurveCenter.z);
            CreateStraight(start, end, false, true, _straightUpperRotationAngle, _rightChainLinks);
            
            // Back Right Curve
            CreateCurve(_rightBackCurveCenter, _chainBackCurveCount.intValue,0f, false, _rightChainLinks);

            // Right Lower Straight
            start.y = -_frontRadius;
            end.y = -_backRadius;
            CreateStraight(end, start, false, false, _straightLowerRotationAngle, _rightChainLinks);
            
            // Front Right Curve
            CreateCurve(_rightFrontCurveCenter, _chainFrontCurveCount.intValue, 180f, false, _rightChainLinks);
        }

        private void CreateCurve(Vector3 center, int curveCount, float startingAngle, bool isLeft, ICollection<GameObject> chainLinks)
        {
            float circumference = _chainSpacing.floatValue * curveCount * 2;
            float radius = circumference / (2 * Mathf.PI);
            
            float angle = 180f / curveCount;
            Vector3 startingVector = Vector3.up * radius;

            for (int i = 0; i < curveCount; i++)
            {
                float currentAngle = i * angle + startingAngle;
                float x = startingVector.x * Mathf.Cos(currentAngle * Mathf.Deg2Rad) - startingVector.y * Mathf.Sin( currentAngle * Mathf.Deg2Rad);
                float y = startingVector.x * Mathf.Sin(currentAngle * Mathf.Deg2Rad) + startingVector.y * Mathf.Cos(currentAngle * Mathf.Deg2Rad);

                Vector3 pivot = new Vector3(x + center.x, y + center.y, center.z);
                
                string linkName = isLeft ? "L Curve " : "R Curve ";
                GameObject chainLink = CreateChainLink(pivot, linkName + i, isLeft, currentAngle);
                chainLinks.Add(chainLink);
            }
        }

        private void CreateStraight(Vector3 start, Vector3 end, bool isLeft, bool isUpper, float rotationAngle, List<GameObject> chainLinks)
        {
            Vector3 direction = (end - start).normalized;

            for (int i = 0; i < _chainStraightCount.intValue; i++)
            {
                Vector3 pivot = start + direction * (i * _chainSpacing.floatValue);
                string linkName = isUpper ? "Upper " : "Lower ";
                linkName += isLeft ? "L Straight " + i : "R Straight " + i;
                GameObject chainLink = CreateChainLink(pivot, linkName, isLeft, rotationAngle);
                chainLinks.Add(chainLink);
            }
        }

        private GameObject CreateChainLink(Vector3 pivot, string objectName, bool isLeft, float rotation = 0f)
        {
            var chainLink = new GameObject(objectName)
            {
                transform =
                {
                    parent = Transform,
                    localPosition = pivot,
                    localRotation = Quaternion.Euler(0, 0, rotation)
                },
            };
            WheelsUtils.ShowLabel(chainLink, _showLabelsProp);
            AttachComponents(chainLink, isLeft);
            return chainLink;
        }

        private void AttachComponents(GameObject chainLink, bool isLeft)
        {
            SerializedProperty mesh =  isLeft ? _leftMesh : _rightMesh;
            AttachMesh(chainLink, mesh, _leftMaterial);
            AttachRigidbody(chainLink, _chainMass);
            AttachBoxCollider(chainLink);
            AttachScript(chainLink);
        }

        private void AttachBoxCollider(GameObject chainLink)
        {
            BoxCollider boxCollider = chainLink.AddComponent<BoxCollider>();
            boxCollider.size = _boxColliderSize.vector3Value;
            boxCollider.center = _boxColliderCenter.vector3Value;
            boxCollider.material = _boxColliderMaterial.objectReferenceValue as PhysicMaterial;
        }

        protected override void AttachRigidbody(GameObject gameObject, SerializedProperty massProp)
        {
            Rigidbody wheelRigidbody = gameObject.AddComponent<Rigidbody>();
            wheelRigidbody.mass = massProp.floatValue;
        }

        private void AttachHingeJoints(List<GameObject> chainLinks)
        {
            foreach (GameObject chainLink in chainLinks)
            {
                int index = chainLinks.IndexOf(chainLink);
                if (index == 0) continue;
                AttachHingeJoint(chainLink, chainLinks[index - 1]);
            }
            
            AttachHingeJoint(chainLinks[0], chainLinks[^1]);
        }

        private void AttachHingeJoint(GameObject link, GameObject prevLink)
        {
            float x = _chainSpacing.floatValue / 2;
            if (link.name.Contains("Lower"))
            {
                x = -x;
            }
        
            Vector3 anchor = new Vector3(x, 0, 0);
        
            HingeJoint hingeJoint = link.AddComponent<HingeJoint>();
            hingeJoint.anchor = anchor;
            hingeJoint.axis = Vector3.forward;
            hingeJoint.connectedBody = prevLink.GetComponent<Rigidbody>();
        }

        private void AttachScript(GameObject chainLink)
        {
            chainLink.AddComponent<ChainLinkBalancer>();
        }

        private void OnSceneGUI()
        {
            if (!_showConnectionsProp.boolValue) return;
            
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