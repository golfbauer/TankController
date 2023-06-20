using System.Collections.Generic;
using UnityEngine;

namespace Controller.Scripts.Editors.Wheels.Chain
{
    public class CreateChain: MonoBehaviour
    {
        // General Settings
        public bool showLabels = false;
        public bool showConnections = false;
        
        // Left Chain Component
        public Vector3 leftRotation = Vector3.zero;
        public Mesh leftMesh;
        public List<Material> leftMaterials;
        
        // Right Chain Component
        public Vector3 rightRotation = Vector3.zero;
        public Mesh rightMesh;
        public List<Material> rightMaterials;
        
        // Chain Rigidbody
        public float chainMass = 30;
        public float angularDrag = 50;
        
        // Chain Relation
        public float chainDistance = 0.1f;
        public float chainSpacing = 0.1f;
        
        // Chain Straight
        public int chainStraightCount = 1;
        
        // Chain Front Curve
        public int chainFrontCurveCount = 3;
        
        // Chain Back Curve
        public int chainBackCurveCount = 3;
        
        // Box Collider
        public Vector3 boxColliderCenter;
        public Vector3 boxColliderSize;
        public PhysicMaterial boxColliderMaterial;

        private void Start()
        {
            Destroy(this);
        }
    }
}