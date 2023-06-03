using System.Collections.Generic;
using Controller.Scripts.Editors.Utils;
using Controller.Scripts.Managers.Tracks;
using UnityEditor;
using UnityEngine;

namespace Controller.Scripts.Editors.Wheels.Chain
{
    [CustomEditor(typeof(CreateChain))]
    [CanEditMultipleObjects]
    public class CreateChainEditor : CreateWheelEditor
    {
        private SerializedProperty _showLabelsProp;
        private SerializedProperty _showConnectionsProp;
        
        private SerializedProperty _rotationProp;
        private SerializedProperty _leftMesh;
        private SerializedProperty _leftMaterial;
        
        private SerializedProperty _rightRotationProp;
        private SerializedProperty _rightMesh;
        private SerializedProperty _rightMaterial;
        
        private SerializedProperty _boxColliderCenter;
        private SerializedProperty _boxColliderSize;
        private SerializedProperty _boxColliderMaterial;
        
        private SerializedProperty _chainMass;
        private SerializedProperty _angularDrag;
        
        private SerializedProperty _chainDistance;
        private SerializedProperty _chainSpacing;
        
        private SerializedProperty _chainStraightCount;
        private SerializedProperty _chainFrontCurveCount;
        private SerializedProperty _chainBackCurveCount;

        
        private Vector3 _leftFrontCurveCenter;
        private Vector3 _leftBackCurveCenter;
        
        private Vector3 _rightBackCurveCenter;
        private Vector3 _rightFrontCurveCenter;

        private Vector3 _leftStraightUpperStart;
        private Vector3 _leftStraightUpperEnd;
        private Vector3 _leftStraightLowerStart;
        private Vector3 _leftStraightLowerEnd;
        
        private Vector3 _rightStraightUpperStart;
        private Vector3 _rightStraightUpperEnd;
        private Vector3 _rightStraightLowerStart;
        private Vector3 _rightStraightLowerEnd;

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
            
            _rotationProp = serializedObject.FindProperty("Rotation");
            
            _leftMesh = serializedObject.FindProperty("leftMesh");
            _leftMaterial = serializedObject.FindProperty("leftMaterial");
            
            _rightMesh = serializedObject.FindProperty("rightMesh");
            _rightMaterial = serializedObject.FindProperty("rightMaterial");
            
            _boxColliderCenter = serializedObject.FindProperty("boxColliderCenter");
            _boxColliderSize = serializedObject.FindProperty("boxColliderSize");
            _boxColliderMaterial = serializedObject.FindProperty("boxColliderMaterial");
            
            _chainMass = serializedObject.FindProperty("chainMass");
            _angularDrag = serializedObject.FindProperty("angularDrag");
            
            _chainDistance = serializedObject.FindProperty("chainDistance");
            _chainSpacing = serializedObject.FindProperty("chainSpacing");
            
            _chainStraightCount = serializedObject.FindProperty("chainStraightCount");
            _chainFrontCurveCount = serializedObject.FindProperty("chainFrontCurveCount");
            _chainBackCurveCount = serializedObject.FindProperty("chainBackCurveCount");
            
            transform = ((CreateChain) target).transform;
            transform.localRotation = Quaternion.Euler(0, 0, 90);
            _leftChainLinks = new List<GameObject>();
            _rightChainLinks = new List<GameObject>();
        }

        public override void SetUpGUI()
        {
            GUIUtils.HeaderGUI("General");
            GUIUtils.PropFieldGUI(_showLabelsProp, "Show Labels");
            GUIUtils.PropFieldGUI(_showConnectionsProp, "Show Connections");
            
            GUIUtils.HeaderGUI("Chain");
            GUIUtils.PropFieldGUI(_rotationProp, "Rotation");
            
            GUIUtils.PropFieldGUI(_leftMesh, "Left Mesh");
            GUIUtils.PropFieldGUI(_leftMaterial, "Left Material");
            
            GUIUtils.PropFieldGUI(_rightMesh, "Right Mesh");
            GUIUtils.PropFieldGUI(_rightMaterial, "Right Material");
            
            GUIUtils.PropFieldGUI(_boxColliderCenter, "Collider Center");
            GUIUtils.PropFieldGUI(_boxColliderSize, "Collider Size");
            GUIUtils.PropFieldGUI(_boxColliderMaterial, "Collider Material");

            GUIUtils.PropFieldGUI(_chainMass, "Chain Mass");
            GUIUtils.PropFieldGUI(_angularDrag, "Angular Drag");
            
            GUIUtils.SliderGUI(_chainDistance, 0.1f, 5f, "Distance");
            GUIUtils.SliderGUI(_chainSpacing, 0.01f, 1f, "Spacing");
            
            GUIUtils.IntSliderGUI(_chainStraightCount, 1, 100, "Straight Count");
            GUIUtils.IntSliderGUI(_chainFrontCurveCount, 1, 100, "Front Curve Count");
            GUIUtils.IntSliderGUI(_chainBackCurveCount, 1, 100, "Back Curve Count");
            
            UpdateAllGUI();
        }
        
        public override void BulkUpdateComponents()
        {
            BulkDestroyComponents();
            BulkCreateComponents();
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
            _frontRadius = _chainSpacing.floatValue / (2 * Mathf.Tan(Mathf.PI / ((_chainFrontCurveCount.intValue - 1) * 2)));
            _backRadius = _chainSpacing.floatValue / (2 * Mathf.Tan(Mathf.PI / ((_chainBackCurveCount.intValue - 1) * 2)));
                
            float heightDifference = Mathf.Abs(_frontRadius - _backRadius);
            float straightDistance = _chainStraightCount.intValue * _chainSpacing.floatValue;

            _actualStraightDistance = Mathf.Sqrt(Mathf.Pow(straightDistance, 2) - Mathf.Pow(heightDifference, 2));
            
            float straightRotationAngle = Mathf.Asin(Mathf.Abs(_frontRadius - _backRadius) / (_chainStraightCount.intValue *
                _chainSpacing.floatValue));
            straightRotationAngle *= Mathf.Rad2Deg;
            
            _straightUpperRotationAngle = _chainFrontCurveCount.intValue > _chainBackCurveCount.intValue ? straightRotationAngle : -straightRotationAngle;
            _straightUpperRotationAngle += 180;
            _straightLowerRotationAngle = _chainFrontCurveCount.intValue > _chainBackCurveCount.intValue ? -straightRotationAngle : straightRotationAngle;

            
            float z = _actualStraightDistance / 2;
            _leftFrontCurveCenter = new Vector3( 0,-_chainDistance.floatValue / 2, z + _chainSpacing.floatValue / 2);
            _leftBackCurveCenter = new Vector3(0, -_chainDistance.floatValue / 2, -z - _chainSpacing.floatValue / 2);
            _rightFrontCurveCenter = new Vector3(0, _chainDistance.floatValue / 2, z + _chainSpacing.floatValue / 2);
            _rightBackCurveCenter = new Vector3(0, _chainDistance.floatValue / 2, -z - _chainSpacing.floatValue / 2);
            
            _leftStraightUpperStart = new Vector3(_frontRadius, _leftFrontCurveCenter.y, z);
            _leftStraightUpperEnd = new Vector3(_backRadius, _leftBackCurveCenter.y, -z);
            _leftStraightLowerStart = new Vector3( -_frontRadius, _leftFrontCurveCenter.y, z);
            _leftStraightLowerEnd = new Vector3( -_backRadius,_leftBackCurveCenter.y, -z);
            
            _rightStraightUpperStart = new Vector3( _frontRadius, _rightFrontCurveCenter.y, z);
            _rightStraightUpperEnd = new Vector3( _backRadius, _rightBackCurveCenter.y, -z);
            _rightStraightLowerStart = new Vector3( -_frontRadius, _rightFrontCurveCenter.y, z);
            _rightStraightLowerEnd = new Vector3( -_backRadius, _rightBackCurveCenter.y, -z);
        }

        private void CreateLeftChainLinks()
        {
            // Front Left Curve
            CreateCurve(false, true, true, _leftChainLinks, _leftFrontCurveCenter, Vector3.left, 0f, 180f);
            
            // Left Upper Straight
            CreateStraight(false, true,_leftStraightUpperStart, _leftStraightUpperEnd, _straightUpperRotationAngle, _leftChainLinks);
            
            // Back Left Curve
            CreateCurve( false,false, true, _leftChainLinks, _leftBackCurveCenter, Vector3.right , -180f, 0f);

            // Left Lower Straight
            CreateStraight(false, false, _leftStraightLowerEnd, _leftStraightLowerStart, _straightLowerRotationAngle, _leftChainLinks);
        }
        
        private void CreateRightChainLinks()
        {
            // Front Right Curve
            CreateCurve(true, true, true, _rightChainLinks, _rightFrontCurveCenter, Vector3.left, 0f, 180f);
            
            // Right Upper Straight
            CreateStraight(true, true,_rightStraightUpperStart, _rightStraightUpperEnd, _straightUpperRotationAngle, _rightChainLinks);
            
            // Back Right Curve
            CreateCurve(true, false, true, _rightChainLinks, _rightBackCurveCenter, Vector3.right,-180f, 0f);

            // Right Lower Straight
            CreateStraight(true, false, _rightStraightLowerEnd, _rightStraightLowerStart, _straightLowerRotationAngle, _rightChainLinks);
        }

        private void CreateCurve(
            bool isLeft,
            bool isFront,
            bool isClockwise,
            ICollection<GameObject> chainLinks,
            Vector3 center, 
            Vector3 startVector,
            float startAngle, 
            float endAngle
        )
        {
            float radius = isFront ? _frontRadius : _backRadius;
            float curveCount = isFront ? _chainFrontCurveCount.intValue : _chainBackCurveCount.intValue;

            Vector3 startingVector = startVector.normalized * radius;

            for (int i = 0; i < curveCount; i++)
            {
                float positionAngle = Mathf.Lerp(0f, Mathf.PI, i / (curveCount - 1));
                if (!isClockwise) {
                    positionAngle = Mathf.PI * 2 - positionAngle;
                }

                float z = startingVector.z * Mathf.Cos(positionAngle) - startingVector.x * Mathf.Sin(positionAngle);
                float x = startingVector.z * Mathf.Sin(positionAngle) + startingVector.x * Mathf.Cos(positionAngle);

                Vector3 pivot = new Vector3(center.x + x, center.y, center.z + z);
                float rotationAngle = Mathf.Lerp(startAngle, endAngle, i / (curveCount - 1));
                Vector3 rotation = _rotationProp.vector3Value;
                rotation.y += rotationAngle;
    
                string linkName = isLeft ? "L " : "R ";
                linkName += isFront ? "Front Curve " : "Rear Curve ";
                GameObject chainLink = CreateChainLink(pivot, linkName + i, isLeft, rotation);
                chainLinks.Add(chainLink);
            }
        }

        private void CreateStraight(bool isLeft, bool isUpper, Vector3 start, Vector3 end, float rotationAngle, List<GameObject> chainLinks)
        {
            Vector3 direction = (end - start).normalized;

            for (float i = 0.5f; i < _chainStraightCount.intValue; i++)
            {
                Vector3 pivot = start + direction * (i * _chainSpacing.floatValue);
                string linkName = isLeft ? "L " : "R ";
                linkName += isUpper ? "Upper Straight " : "Lower Straight ";
                Vector3 rotation = new Vector3(0, rotationAngle, 0);
                GameObject chainLink = CreateChainLink(pivot, linkName + (i - 0.5f), isLeft, rotation);
                chainLinks.Add(chainLink);
            }
        }

        private GameObject CreateChainLink(Vector3 pivot, string objectName, bool isLeft, Vector3 rotation)
        {
            rotation.z = -90f;
            var chainLink = new GameObject(objectName)
            {
                transform =
                {
                    parent = transform,
                    localPosition = pivot,
                    localRotation = Quaternion.Euler(rotation),
                },
            };
            DrawUtils.ShowLabel(chainLink, _showLabelsProp);
            AttachComponents(chainLink, isLeft);
            return chainLink;
        }

        private void AttachComponents(GameObject chainLink, bool isLeft)
        {
            SerializedProperty mesh =  isLeft ? _leftMesh : _rightMesh;
            AttachMesh(chainLink, mesh, _leftMaterial);
            AttachRigidbody(chainLink, _chainMass, _angularDrag);
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

        private void AttachHingeJoints(List<GameObject> chainLinks)
        {
            foreach (GameObject chainLink in chainLinks)
            {
                int index = chainLinks.IndexOf(chainLink);
                if (index == chainLinks.Count - 1) continue;
                AttachHingeJoint(chainLink, chainLinks[index + 1]);
            }
            
            AttachHingeJoint(chainLinks[^1], chainLinks[0]);
        }

        private void AttachHingeJoint(GameObject link, GameObject prevLink)
        {
            float z = _chainSpacing.floatValue / 2;

            Vector3 anchor = new Vector3(0, 0, z);
        
            HingeJoint hingeJoint = link.AddComponent<HingeJoint>();
            hingeJoint.anchor = anchor;
            hingeJoint.axis = Vector3.right;
            hingeJoint.connectedBody = prevLink.GetComponent<Rigidbody>();
        }

        private void AttachScript(GameObject chainLink)
        {
            chainLink.AddComponent<ChainLinkBalancer>();
        }

        protected override void OnSceneGUI()
        {
            if (!_showConnectionsProp.boolValue) return;
            
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject chainLink = transform.GetChild(i).gameObject;
                
                DrawUtils.DrawLine(
                    transform.position,
                    transform.position + chainLink.transform.localPosition,
                    Color.green
                );
            }
        }
    }
}