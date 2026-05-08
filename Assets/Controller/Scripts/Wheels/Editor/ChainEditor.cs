using System.Collections.Generic;
using Controller.Scripts.Utils;
using Controller.Scripts.Wheels.Chain;
using Controller.Scripts.Wheels;
using UnityEditor;
using UnityEngine;

namespace Controller.Scripts.Wheels.Chain
{
    [CustomEditor(typeof(Chain))]
    [CanEditMultipleObjects]
    public class ChainEditor : WheelEditor
    {
        private float _actualStraightDistance;
        private SerializedProperty _angularDrag;
        private float _backRadius;

        private SerializedProperty _boxColliderCenter;
        private SerializedProperty _boxColliderMaterial;
        private SerializedProperty _boxColliderSize;
        private SerializedProperty _chainBackCurveCount;

        private SerializedProperty _chainDistance;
        private SerializedProperty _chainFrontCurveCount;

        private SerializedProperty _chainMass;
        private SerializedProperty _chainSpacing;

        private SerializedProperty _chainStraightCount;

        private float _frontRadius;

        private SerializedProperty _hingeJointBreakForce;
        private Vector3 _leftBackCurveCenter;

        private List<GameObject> _leftChainLinks;

        private Vector3 _leftFrontCurveCenter;
        private SerializedProperty _leftMaterials;
        private SerializedProperty _leftMesh;

        private SerializedProperty _leftRotationProp;
        private Vector3 _leftStraightLowerEnd;
        private Vector3 _leftStraightLowerStart;
        private Vector3 _leftStraightUpperEnd;

        private Vector3 _leftStraightUpperStart;

        private Vector3 _rightBackCurveCenter;
        private List<GameObject> _rightChainLinks;
        private Vector3 _rightFrontCurveCenter;
        private SerializedProperty _rightMaterials;
        private SerializedProperty _rightMesh;

        private SerializedProperty _rightRotationProp;
        private Vector3 _rightStraightLowerEnd;
        private Vector3 _rightStraightLowerStart;
        private Vector3 _rightStraightUpperEnd;

        private Vector3 _rightStraightUpperStart;
        private SerializedProperty _showConnectionsProp;
        private SerializedProperty _showLabelsProp;
        private float _straightLowerRotationAngle;

        private float _straightUpperRotationAngle;

        private void OnEnable()
        {
            _showLabelsProp = serializedObject.FindProperty("showLabels");
            _showConnectionsProp = serializedObject.FindProperty("showConnections");

            _leftRotationProp = serializedObject.FindProperty("leftRotation");
            _leftMesh = serializedObject.FindProperty("leftMesh");
            _leftMaterials = serializedObject.FindProperty("leftMaterials");

            _rightRotationProp = serializedObject.FindProperty("rightRotation");
            _rightMesh = serializedObject.FindProperty("rightMesh");
            _rightMaterials = serializedObject.FindProperty("rightMaterials");

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

            _hingeJointBreakForce = serializedObject.FindProperty("hingeJointBreakForce");

            transform = ((Chain)target).transform;
            transform.localRotation = Quaternion.Euler(0, 0, 90);
            _leftChainLinks = new List<GameObject>();
            _rightChainLinks = new List<GameObject>();
        }

        protected override void OnSceneGUI()
        {
            if (!_showConnectionsProp.boolValue)
                return;
            var rotation90DegreesAroundZ = Quaternion.Euler(0, 0, 90);

            for (var i = 0; i < transform.childCount; i++)
            {
                var chainLink = transform.GetChild(i).gameObject;
                var rotatedLocalPosition =
                    rotation90DegreesAroundZ * chainLink.transform.localPosition;

                DrawUtils.DrawLine(
                    transform.position,
                    transform.position + rotatedLocalPosition,
                    Color.green
                );
            }
        }

        public override void SetUpGUI()
        {
            GUIUtils.HeaderGUI(WheelMessages.GeneralSettings);
            GUIUtils.PropFieldGUI(_showLabelsProp);
            GUIUtils.PropFieldGUI(_showConnectionsProp, WheelMessages.ShowConnections);

            GUIUtils.HeaderGUI(WheelMessages.LeftChainSettings);
            GUIUtils.PropFieldGUI(_leftRotationProp, WheelMessages.EulerRotation);
            GUIUtils.PropFieldGUI(_leftMesh, WheelMessages.Mesh);
            GUIUtils.PropFieldGUI(_leftMaterials, WheelMessages.Material);

            GUIUtils.HeaderGUI(WheelMessages.RightChainSettings);
            GUIUtils.PropFieldGUI(_rightRotationProp, WheelMessages.EulerRotation);
            GUIUtils.PropFieldGUI(_rightMesh, WheelMessages.Mesh);
            GUIUtils.PropFieldGUI(_rightMaterials, WheelMessages.Material);

            GUIUtils.HeaderGUI(WheelMessages.ChainSettings);
            GUIUtils.PropFieldGUI(_boxColliderCenter, WheelMessages.ColliderCenter);
            GUIUtils.PropFieldGUI(_boxColliderSize, WheelMessages.ColliderSize);
            GUIUtils.PropFieldGUI(_boxColliderMaterial, WheelMessages.ColliderMaterial);

            GUIUtils.PropFieldGUI(_chainMass, WheelMessages.Mass);
            GUIUtils.PropFieldGUI(_angularDrag, WheelMessages.AngularDrag);

            GUIUtils.PropFieldGUI(_hingeJointBreakForce, WheelMessages.BreakForce);

            GUIUtils.SliderGUI(_chainDistance, 0.1f, 5f, WheelMessages.Distance);
            GUIUtils.SliderGUI(_chainSpacing, 0.01f, 1f, WheelMessages.Spacing);

            GUIUtils.IntSliderGUI(_chainStraightCount, 1, 100, WheelMessages.StraightCount);
            GUIUtils.IntSliderGUI(_chainFrontCurveCount, 1, 100, WheelMessages.FrontCurveCount);
            GUIUtils.IntSliderGUI(_chainBackCurveCount, 1, 100, WheelMessages.BackCurveCount);

            UpdateAllGUI();
        }

        public override void ApplyUpdate()
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
            _frontRadius =
                _chainSpacing.floatValue
                / (2 * Mathf.Tan(Mathf.PI / ((_chainFrontCurveCount.intValue - 1) * 2)));
            _backRadius =
                _chainSpacing.floatValue
                / (2 * Mathf.Tan(Mathf.PI / ((_chainBackCurveCount.intValue - 1) * 2)));

            var heightDifference = Mathf.Abs(_frontRadius - _backRadius);
            var straightDistance = _chainStraightCount.intValue * _chainSpacing.floatValue;

            _actualStraightDistance = Mathf.Sqrt(
                Mathf.Pow(straightDistance, 2) - Mathf.Pow(heightDifference, 2)
            );

            var straightRotationAngle = Mathf.Asin(
                Mathf.Abs(_frontRadius - _backRadius)
                    / (_chainStraightCount.intValue * _chainSpacing.floatValue)
            );
            straightRotationAngle *= Mathf.Rad2Deg;

            _straightUpperRotationAngle =
                _chainFrontCurveCount.intValue > _chainBackCurveCount.intValue
                    ? straightRotationAngle
                    : -straightRotationAngle;
            _straightUpperRotationAngle += 180;
            _straightLowerRotationAngle =
                _chainFrontCurveCount.intValue > _chainBackCurveCount.intValue
                    ? -straightRotationAngle
                    : straightRotationAngle;

            var z = _actualStraightDistance / 2;
            _leftFrontCurveCenter = new Vector3(
                0,
                -_chainDistance.floatValue / 2,
                z + _chainSpacing.floatValue / 2
            );
            _leftBackCurveCenter = new Vector3(
                0,
                -_chainDistance.floatValue / 2,
                -z - _chainSpacing.floatValue / 2
            );
            _rightFrontCurveCenter = new Vector3(
                0,
                _chainDistance.floatValue / 2,
                z + _chainSpacing.floatValue / 2
            );
            _rightBackCurveCenter = new Vector3(
                0,
                _chainDistance.floatValue / 2,
                -z - _chainSpacing.floatValue / 2
            );

            _leftStraightUpperStart = new Vector3(_frontRadius, _leftFrontCurveCenter.y, z);
            _leftStraightUpperEnd = new Vector3(_backRadius, _leftBackCurveCenter.y, -z);
            _leftStraightLowerStart = new Vector3(-_frontRadius, _leftFrontCurveCenter.y, z);
            _leftStraightLowerEnd = new Vector3(-_backRadius, _leftBackCurveCenter.y, -z);

            _rightStraightUpperStart = new Vector3(_frontRadius, _rightFrontCurveCenter.y, z);
            _rightStraightUpperEnd = new Vector3(_backRadius, _rightBackCurveCenter.y, -z);
            _rightStraightLowerStart = new Vector3(-_frontRadius, _rightFrontCurveCenter.y, z);
            _rightStraightLowerEnd = new Vector3(-_backRadius, _rightBackCurveCenter.y, -z);
        }

        private void CreateLeftChainLinks()
        {
            // Front Left Curve
            CreateCurve(
                false,
                true,
                true,
                _leftChainLinks,
                _leftFrontCurveCenter,
                Vector3.left,
                0f,
                180f
            );

            // Left Upper Straight
            CreateStraight(
                false,
                true,
                _leftStraightUpperStart,
                _leftStraightUpperEnd,
                _straightUpperRotationAngle,
                _leftChainLinks
            );

            // Back Left Curve
            CreateCurve(
                false,
                false,
                true,
                _leftChainLinks,
                _leftBackCurveCenter,
                Vector3.right,
                -180f,
                0f
            );

            // Left Lower Straight
            CreateStraight(
                false,
                false,
                _leftStraightLowerEnd,
                _leftStraightLowerStart,
                _straightLowerRotationAngle,
                _leftChainLinks
            );
        }

        private void CreateRightChainLinks()
        {
            // Front Right Curve
            CreateCurve(
                true,
                true,
                true,
                _rightChainLinks,
                _rightFrontCurveCenter,
                Vector3.left,
                0f,
                180f
            );

            // Right Upper Straight
            CreateStraight(
                true,
                true,
                _rightStraightUpperStart,
                _rightStraightUpperEnd,
                _straightUpperRotationAngle,
                _rightChainLinks
            );

            // Back Right Curve
            CreateCurve(
                true,
                false,
                true,
                _rightChainLinks,
                _rightBackCurveCenter,
                Vector3.right,
                -180f,
                0f
            );

            // Right Lower Straight
            CreateStraight(
                true,
                false,
                _rightStraightLowerEnd,
                _rightStraightLowerStart,
                _straightLowerRotationAngle,
                _rightChainLinks
            );
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
            var radius = isFront ? _frontRadius : _backRadius;
            float curveCount = isFront
                ? _chainFrontCurveCount.intValue
                : _chainBackCurveCount.intValue;

            var startingVector = startVector.normalized * radius;

            for (var i = 0; i < curveCount; i++)
            {
                var positionAngle = Mathf.Lerp(0f, Mathf.PI, i / (curveCount - 1));
                if (!isClockwise)
                    positionAngle = Mathf.PI * 2 - positionAngle;

                var z =
                    startingVector.z * Mathf.Cos(positionAngle)
                    - startingVector.x * Mathf.Sin(positionAngle);
                var x =
                    startingVector.z * Mathf.Sin(positionAngle)
                    + startingVector.x * Mathf.Cos(positionAngle);

                var pivot = new Vector3(center.x + x, center.y, center.z + z);
                var rotationAngle = Mathf.Lerp(startAngle, endAngle, i / (curveCount - 1));
                var rotation = isLeft
                    ? _leftRotationProp.vector3Value
                    : _rightRotationProp.vector3Value;
                rotation.y += rotationAngle;

                var linkName = isLeft ? "L " : "R ";
                linkName += isFront ? "Front Curve " : "Rear Curve ";
                var chainLink = CreateChainLink(pivot, linkName + i, isLeft, rotation);
                chainLinks.Add(chainLink);
            }
        }

        private void CreateStraight(
            bool isLeft,
            bool isUpper,
            Vector3 start,
            Vector3 end,
            float rotationAngle,
            List<GameObject> chainLinks
        )
        {
            var direction = (end - start).normalized;

            for (var i = 0.5f; i < _chainStraightCount.intValue; i++)
            {
                var pivot = start + direction * (i * _chainSpacing.floatValue);
                var linkName = isLeft ? "L " : "R ";
                linkName += isUpper ? "Upper Straight " : "Lower Straight ";
                var rotation = isLeft
                    ? _leftRotationProp.vector3Value
                    : _rightRotationProp.vector3Value;
                rotation.y = rotationAngle;
                var chainLink = CreateChainLink(pivot, linkName + (i - 0.5f), isLeft, rotation);
                chainLinks.Add(chainLink);
            }
        }

        private GameObject CreateChainLink(
            Vector3 pivot,
            string objectName,
            bool isLeft,
            Vector3 rotation
        )
        {
            rotation.z = -90f;
            var chainLink = new GameObject(objectName)
            {
                transform =
                {
                    parent = transform,
                    localPosition = pivot,
                    localRotation = Quaternion.Euler(rotation)
                }
            };
            DrawUtils.ShowLabel(chainLink, _showLabelsProp);
            AttachComponents(chainLink, isLeft);
            return chainLink;
        }

        private void AttachComponents(GameObject chainLink, bool isLeft)
        {
            var mesh = isLeft ? _leftMesh : _rightMesh;
            UpdateMesh(chainLink.transform, mesh, _leftMaterials);
            UpdateRigidbody(chainLink.transform, _chainMass, _angularDrag);
            AttachBoxCollider(chainLink);
            AttachScript(chainLink);
        }

        private void AttachBoxCollider(GameObject chainLink)
        {
            var boxCollider = chainLink.AddComponent<BoxCollider>();
            boxCollider.size = _boxColliderSize.vector3Value;
            boxCollider.center = _boxColliderCenter.vector3Value;
            boxCollider.material = _boxColliderMaterial.objectReferenceValue as PhysicsMaterial;
        }

        private void AttachHingeJoints(List<GameObject> chainLinks)
        {
            foreach (var chainLink in chainLinks)
            {
                var index = chainLinks.IndexOf(chainLink);
                if (index == chainLinks.Count - 1)
                    continue;
                AttachHingeJoint(chainLink, chainLinks[index + 1]);
            }

            AttachHingeJoint(chainLinks[^1], chainLinks[0]);
        }

        private void AttachHingeJoint(GameObject link, GameObject prevLink)
        {
            var z = _chainSpacing.floatValue / 2;

            var anchor = new Vector3(0, 0, z);

            var hingeJoint = link.AddComponent<HingeJoint>();
            hingeJoint.anchor = anchor;
            hingeJoint.axis = Vector3.right;
            hingeJoint.connectedBody = prevLink.GetComponent<Rigidbody>();
            hingeJoint.breakForce = _hingeJointBreakForce.floatValue;
        }

        private void AttachScript(GameObject chainLink)
        {
            chainLink.AddComponent<ChainLinkBalancer>();
        }
    }
}
