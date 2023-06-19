using System.Collections.Generic;
using UnityEngine;

namespace Controller.Scripts.Managers.ImpactCollision
{
    public class CollisionManager : MonoBehaviour
    {
        [SerializeField] 
        public List<ArmorSection> armorSections = new List<ArmorSection>();

        [SerializeField] public ArmorSection defaultArmorSection;

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