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

        // TODO(refactor 3.2): Replace Parallel.ForEach with a serial foreach. Three issues:
        //   1. `matchingArmorSection = armorSection` is an unsynchronized cross-thread write — race.
        //   2. armorSections is small; Parallel dispatch overhead exceeds the work.
        //   3. `impactPoint = localImpactPoint, tolerance = tolerance` look like named args
        //      but are assignment expressions — the second one needlessly rewrites the field on every call.
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
