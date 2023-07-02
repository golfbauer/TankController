using System.Collections.Generic;
using UnityEngine;

namespace Controller.Scripts.Editors.Tank
{
    public class Tank: MonoBehaviour
    {
        // Wheels
        public ComponentType componentType = ComponentType.Turret;

        // Hull
        // Rigidbody
        public float hullMass = 500f;
        public int physicsIterations = 16;
        public Vector3 hullCenterOfMass = Vector3.zero;

        // Mesh
        public Mesh hullMesh = null;
        public List<Material> hullMaterials = null;
        
        // Collider
        public bool useBoxCollider = false;
        public List<Mesh> hullMeshColliders = null;
        public Vector3 hullColliderCenter = Vector3.zero;
        public Vector3 hullColliderSize = Vector3.one;
        
        // Manager
        public bool useCameraManager = true;
        public bool useCollisionManager = true;
        public bool useMovementManager = true;


        private void Awake()
        {
            Rigidbody tankRigidbody = gameObject.GetComponent<Rigidbody>();
            
            if (tankRigidbody == null)
            {
                tankRigidbody = gameObject.AddComponent<Rigidbody>();
            }
            
            tankRigidbody.solverIterations = physicsIterations;
            tankRigidbody.centerOfMass = hullCenterOfMass;
        }
    }
}