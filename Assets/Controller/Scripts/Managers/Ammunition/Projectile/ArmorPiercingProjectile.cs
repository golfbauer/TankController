using Controller.Scripts.Editors.Utils;
using Controller.Scripts.Managers.ImpactCollision;
using UnityEngine;

namespace Controller.Scripts.Managers.Ammunition.Projectile
{
    public class ArmorPiercingProjectile : BaseProjectile
    {
        protected override float GetDragCoefficient()
        {
            return 0.2f;
        }

        protected override float GetReferenceArea()
        {
            return Mathf.PI * Mathf.Pow((diameter / 100) / 2, 2);
        }

        protected override void HandleTankCollision(Collision collision,
            GameObject hitTank)
        {
            ArmorSection armorSection = GetArmorSection(hitTank, collision);

            Vector3 armorNormal = collision.contacts[0].normal;
            Vector3 projectileDirection =
                collision.relativeVelocity.normalized;
            float impactAngle =
                Vector3.Angle(armorNormal, projectileDirection);

            if (CalculateRicochet(impactAngle))
            {
                Debug.LogWarning("Ricochet");
                return;
            }

            float effectiveArmorThickness =
                CalculateEffectiveArmor(armorSection.thickness, impactAngle);
            float penetrationCapability =
                CalculatePenetration(mass, initVelocity, diameter);

            Debug.Log("Penetration capability: " + penetrationCapability);
            Debug.Log("Effective armor thickness: " +
                      effectiveArmorThickness);
            Debug.Log("Impat angle: " + impactAngle);

            if (penetrationCapability > effectiveArmorThickness)
            {
                Destroy(gameObject);
                Debug.LogWarning("Penetration");
                return;
            }

            Debug.LogWarning("No penetration");
            Destroy(gameObject);
        }

        protected virtual float CalculatePenetration(float m, float v,
            float d)
        {
            // Using Krupp formula: P = 0.0194 + sqrt(sqrt(0.5 * m * v^2)^3 / D^5)
            Debug.Log("");
            return 0.0194f *
                   Mathf.Pow(
                       Mathf.Pow(0.5f * m * Mathf.Pow(v, 2), 3) /
                       Mathf.Pow(d / 10, 5), 0.25f);
        }

        protected virtual float CalculateEffectiveArmor(float thickness,
            float angle)
        {
            return thickness / Mathf.Cos(Mathf.Deg2Rad * angle);
        }

        protected virtual bool CalculateRicochet(float angle)
        {
            const float ricochetThreshold = 70f;

            if (angle > ricochetThreshold)
            {
                return true;
            }

            return false;
        }

        protected override void HandleGenericCollision(Collision collision,
            GameObject hitObject)
        {
            Destroy(gameObject);
        }

        public override void EditorSetUp()
        {
        }
    }
}