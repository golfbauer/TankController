using System.Collections.Generic;
using System.Threading.Tasks;
using Controller.Scripts.ImpactCollision;
using UnityEngine;

namespace Controller.Scripts.ImpactCollision
{
    public class CollisionManager : MonoBehaviour
    {
        [SerializeField]
        public List<ArmorSection> armorSections = new();

        [SerializeField]
        public ArmorSection defaultArmorSection;

        [SerializeField]
        public float tolerance = 0.01f;

        // Serialized Editor properties
        public List<Vector3> vertices = new();
        public bool useColliderVertices = true;
        public float colorThicknessModifier = 1.0f;
        public bool showArmorSettingSection;

        public ArmorSection HandleImpact(Vector3 impactPoint, Transform transform)
        {
            ArmorSection matchingArmorSection = null;
            Vector3 localImpactPoint = transform.InverseTransformPoint(impactPoint);
            Parallel.ForEach(
                armorSections,
                (armorSection, state) =>
                {
                    if (
                        armorSection.IsImpactPointWithinArmorSection(
                            impactPoint = localImpactPoint,
                            tolerance = tolerance
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
