using UnityEngine;

namespace Controller.Scripts.Editors.Wheels.Chain
{
    public class CreateChain: MonoBehaviour
    {
        // General Settings
        public bool showLabels = false;
        public bool showConnections = false;
        
        // Left Chain Component
        public Vector3 leftRotation;
        public Mesh leftMesh;
        public Material leftMaterial;
        
        // Right Chain Component
        public Vector3 Rotation;
        public Mesh rightMesh;
        public Material rightMaterial;
        
        // Chain Rigidbody
        public float chainMass;
        
        // Chain Relation
        public float chainDistance;
        public float chainSpacing;
        
        // Chain Straight
        public int chainStraightCount = 1;
        
        // Chain Front Curve
        public int chainFrontCurveCount = 1;
        
        // Chain Back Curve
        public int chainBackCurveCount = 1;
        
        // Box Collider
        public Vector3 boxColliderCenter;
        public Vector3 boxColliderSize;
        public PhysicMaterial boxColliderMaterial;
    }
}