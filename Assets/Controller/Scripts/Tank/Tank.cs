using System.Collections.Generic;
using Controller.Scripts.Tank;
using UnityEngine;

namespace Controller.Scripts.Tank
{
    public class Tank : MonoBehaviour
    {
        // Wheels
        public ComponentType componentType = ComponentType.Turret;

        // Hull
        // Rigidbody
        public float hullMass = 500f;
        public int physicsIterations = 16;
        public Vector3 hullCenterOfMass = Vector3.zero;

        // Mesh
        public Mesh hullMesh;
        public List<Material> hullMaterials;

        // Collider
        public bool useBoxCollider;
        public List<Mesh> hullMeshColliders;
        public Vector3 hullColliderCenter = Vector3.zero;
        public Vector3 hullColliderSize = Vector3.one;

        // Manager
        public bool useCameraManager = true;
        public bool useCollisionManager = true;
        public bool useMovementManager = true;
    }
}
