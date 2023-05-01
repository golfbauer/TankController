using UnityEngine;

namespace Controller.Scripts.Editors.Wheels.Chain
{
    public class CreateChain: MonoBehaviour
    {
        // General Settings
        public bool showLabels = false;
        public bool showConnections = false;
        
        // Left Chain Component
        public Mesh leftMesh;
        public Material leftMaterial;
        
        // Right Chain Component
        public Mesh rightMesh;
        public Material rightMaterial;
        
        // Chain Rigidbody
        public float chainMass;
        
        // Chain Relation
        public float chainDistance;
        public float chainSpacing;
        
        // Chain Straight
        public int chainStraightCount;
        
        // Chain Front Curve
        public int chainFrontCurveCount;
        
        // Chain Back Curve
        public int chainBackCurveCount;
        
        // Box Collider
        public Vector3 boxColliderCenter;
        public Vector3 boxColliderSize;
        public PhysicMaterial boxColliderMaterial;
    }
}