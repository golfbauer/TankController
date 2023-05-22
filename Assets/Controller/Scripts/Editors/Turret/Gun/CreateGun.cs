using UnityEngine;

namespace Controller.Scripts.Editors.Turret.Gun
{
    public class CreateGun : MonoBehaviour
    {
        public bool useBoxCollider;
        public bool boxColliderChangeManually;
        public PhysicMaterial physicsMaterial;
        
        // Main Gun
        // Mesh
        public Mesh mainGunMesh;
        public Material[] mainGunMaterials;
        
        // Collider
        public MeshCollider[] mainGunColliderMeshes;
        
        // BoxCollider
        public Vector3 mainGunBoxColliderSize;
        public Vector3 mainGunBoxColliderCenter;
        
        // Mantlet
        public Mesh mantletMesh;
        public Material[] mantletMaterials;
        
        // Collider
        public MeshCollider[] mantletColliderMeshes;
        
        // BoxCollider
        public Vector3 mantletBoxColliderSize;
        public Vector3 mantletBoxColliderCenter;
    }
}