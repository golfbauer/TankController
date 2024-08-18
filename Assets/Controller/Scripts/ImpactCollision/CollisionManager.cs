using System.Collections.Generic;
using System.Threading.Tasks;
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

        // Serialized Editor properties
        public List<Vector3> vertices = new();
        public bool useColliderVertices = true;
        public float colorThicknessModifier = 1.0f;
        public bool showArmorSettingSection;

        public ArmorSection HandleImpact(Vector3 impactPoint, Transform transform)
        {
            ArmorSection matchingArmorSection = null;
            Parallel.ForEach(
                armorSections,
                (armorSection, state) =>
                {
                    if (
                        armorSection.IsImpactPointWithinArmorSection(
                            impactPoint,
                            transform.position
                        )
                    )
                    {
                        matchingArmorSection = armorSection;
                        state.Stop();
                    }
                }
            );

            return matchingArmorSection ?? defaultArmorSection;
        }
    }
}
