using System.Collections.Generic;
using Controller.Scripts.ImpactCollision.Services;
using UnityEngine;

namespace Controller.Scripts.ImpactCollision
{
    public class CollisionManager : MonoBehaviour
    {
        [SerializeField]
        public List<ArmorSection> armorSections = new();

        [SerializeField]
        public ArmorSection defaultArmorSection;

        public List<Vector3> vertices = new();
        public bool useColliderVertices = true;
        public Color colliderColor = Color.blue;

        public ArmorSection HandleImpact(Vector3 impactPoint, Transform transform)
        {
            foreach (var armorSection in armorSections)
                if (armorSection.IsImpactPointWithinArmorSection(impactPoint, transform.position))
                    return armorSection;

            return defaultArmorSection;
        }
    }
}
