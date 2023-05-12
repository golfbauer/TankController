using UnityEngine;

namespace Controller.Scripts.Editors.Wheels

{
    public class CreateWheel: MonoBehaviour
    {
        // Debug settings
        public bool showLabels = false;
        
        // Wheel Settings
        // Right
        public Vector3 rightWheelRotation = Vector3.zero;
        public Vector3 rightWheelTorque = Vector3.forward;
        public Vector3 rightWheelHingeAxis = Vector3.right;
        
        // Left
        public Vector3 leftWheelRotation = Vector3.zero;
        public Vector3 leftWheelTorque = Vector3.forward;
        public Vector3 leftWheelHingeAxis = Vector3.right;

        // Left Mesh 
        public Mesh leftWheelMesh = null;
        public Material leftWheelMaterial = null;
        
        // Right Mesh 
        public Mesh rightWheelMesh = null;
        public Material rightWheelMaterial = null;
        
        // Collider 
        public float wheelColliderRadius = 0.3f;
        public PhysicMaterial wheelColliderMaterial = null;
        
        // Relation
        public float wheelDistance = 0.1f;
        public float wheelSpacing = 0.1f;
    }
}