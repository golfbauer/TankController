using UnityEngine;

namespace Controller.Scripts.Editors.Wheels

{
    public class CreateWheel: MonoBehaviour
    {
        // Debug settings
        public bool showLabels = false;
        
        // Wheel
        // Right Settings
        public Vector3 rightWheelRotation = Vector3.zero;
        public Vector3 rightTorqueDirection = Vector3.right;
        
        // Left Settings
        public Vector3 leftWheelRotation = Vector3.zero;
        public Vector3 leftTorqueDirection = Vector3.right;

        // Left Mesh settings
        public Mesh leftWheelMesh = null;
        public Material leftWheelMaterial = null;
        
        // Right Mesh settings
        public Mesh rightWheelMesh = null;
        public Material rightWheelMaterial = null;
        
        // Collider settings
        public float wheelColliderRadius = 0.3f;
        public PhysicMaterial wheelColliderMaterial = null;
        
        // Relation
        public int wheelCount = 0;
        public float wheelDistance = 0.1f;
        public float wheelSpacing = 0.1f;
    }
}