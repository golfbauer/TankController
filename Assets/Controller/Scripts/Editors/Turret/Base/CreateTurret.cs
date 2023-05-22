using UnityEngine;

namespace Controller.Scripts.Editors.Turret.Base
{
    public class CreateTurret : MonoBehaviour
    {
        // Turret
        // Mesh
        public Mesh turretMesh;
        public Material[] turretMaterials;
        
        // Collider
        public MeshCollider[] colliderMeshes;

        public bool useBoxCollider;
        public PhysicMaterial physicsMaterial;

        public bool boxColliderChangeManually;
        public Vector3 boxColliderSize;
        public Vector3 boxColliderCenter;
    }
}