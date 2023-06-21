using System.Collections.Generic;
using UnityEngine;

namespace Controller.Scripts.Editors.Turret.Base
{
    public class Turret : MonoBehaviour
    {
        // Turret
        // Mesh
        public Mesh turretMesh;
        public List<Material> turretMaterials;
        
        // Collider
        public List<Mesh> colliderMeshes;

        public bool useBoxCollider;
        public PhysicMaterial physicsMaterial;

        public bool boxColliderChangeManually;
        public Vector3 boxColliderSize;
        public Vector3 boxColliderCenter;
    }
}