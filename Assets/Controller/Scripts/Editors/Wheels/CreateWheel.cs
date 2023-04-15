using UnityEngine;

namespace Controller.Scripts.Editors.Wheels

{
    public class CreateWheel: MonoBehaviour
    {
        public float wheelMass = 30f;
        public float wheelColliderRadius = 0.3f;
        public PhysicMaterial wheelColliderMaterial = null;
        public Mesh wheelMesh = null;
        public Material wheelMaterial = null;
        
        public float wheelDistance = 2f;
        public int wheelCount = 3;
        public float wheelSpacing = 4f;
    }
}