using UnityEngine;

namespace Controller.Scripts.Editors.Wheels

{
    public class CreateWheel: MonoBehaviour
    {
        // Debug settings
        public bool showLabels = false;
        
        // Wheel
        // Settings
        public Vector3 wheelEulerRotation = Vector3.zero;
        
        // Collider settings
        public float wheelColliderRadius = 0.3f;
        public PhysicMaterial wheelColliderMaterial = null;

        // Mesh settings
        public Mesh wheelMesh = null;
        public Material wheelMaterial = null;
        
        // Relation
        public int wheelCount = 0;
        public float wheelDistance = 0.1f;
        public float wheelSpacing = 0.1f;
    }
}