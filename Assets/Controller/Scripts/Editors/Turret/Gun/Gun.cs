using System.Collections.Generic;
using UnityEngine;

namespace Controller.Scripts.Editors.Turret.Gun
{
    public class Gun : MonoBehaviour
    {
        // Main Gun
        // Mesh
        public Mesh mainGunMesh;
        public List<Material> mainGunMaterials;
        
        // Collider
        public List<Mesh> mainGunColliderMeshes;
        
        // BoxCollider
        public bool mainGunUseBoxCollider;
        public bool mainGunBoxColliderChangeManually;
        public Vector3 mainGunBoxColliderSize;
        public Vector3 mainGunBoxColliderCenter;
        
        // Mantlet
        //Mesh
        public Mesh mantletMesh;
        public List<Material> mantletMaterials;
        
        // Collider
        public List<Mesh> mantletColliderMeshes;
        
        // BoxCollider
        public bool mantletUseBoxCollider;
        public bool mantletBoxColliderChangeManually;
        public Vector3 mantletBoxColliderSize;
        public Vector3 mantletBoxColliderCenter;
    }
}