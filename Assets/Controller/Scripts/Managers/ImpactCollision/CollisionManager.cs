using System.Collections.Generic;
using UnityEngine;

namespace Controller.Scripts.Managers.ImpactCollision
{
    public class CollisionManager : MonoBehaviour
    {
        [SerializeField] 
        public List<ArmorSection> armorSections = new ();
        [SerializeField] 
        public ArmorSection defaultArmorSection;

        public List<Vector3> vertices = new ();
        public bool useColliderVertices = true;
        public Color colliderColor = Color.blue;

        public ArmorSection HandleImpact(Vector3 impactPoint, Transform transform)
        {
            foreach (ArmorSection armorSection in armorSections)
            {
                if (armorSection.IsImpactPointWithinArmorSection(impactPoint, transform.position))
                {
                    return armorSection;
                }
            }

            return defaultArmorSection;
        }
    }
}