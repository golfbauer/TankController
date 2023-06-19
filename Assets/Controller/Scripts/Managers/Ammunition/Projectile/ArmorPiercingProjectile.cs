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
            return Mathf.PI * Mathf.Pow((caliber / 100) / 2, 2);
        }

        protected override void HandleTankCollision(Collision collision, GameObject hitTank)
        {
            ArmorSection armorSection = GetArmorSection(hitTank, collision);
            
            Vector3 armorNormal = collision.contacts[0].normal;
            if (!armorSection.useColliderAngle)
            {
                armorNormal = armorSection.RotatePlaneNormal();
            }
            
            Vector3 projectileDirection = collision.relativeVelocity.normalized;
            float angle = Vector3.Angle(armorNormal, projectileDirection);
            
            if (CalculateRicochet(angle))
            {
                
                return;
            }

            float effectiveArmorThickness = CalculateEffectiveArmor(armorSection.thickness, angle);

            float penetrationCapability = CalculatePenetrationDeMarre(caliber, mass, (float) armorSection.armorMaterialType);
            
            Debug.Log("Penetration capability: " + penetrationCapability);
            Debug.Log("Effective armor thickness: " + effectiveArmorThickness);
            
            if (penetrationCapability > effectiveArmorThickness)
            {
                // The armor is penetrated
                Destroy(this);
                Debug.Log("Penetration succeeded");
                return;
            }
            
            // The armor is not penetrated
            Destroy(this);
        }
        
        protected virtual float CalculatePenetrationDeMarre(float diameter, float weight, float armorMaterialCoefficient)
        {
            const float n = 0.75f; // Exponent for diameter
            const float m = 0.5f;  // Exponent for weight

            // Using DeMarre formula: P = K * D^n * W^m
            return armorMaterialCoefficient * Mathf.Pow(diameter, n) * Mathf.Pow(weight, m);
        }
        
        protected virtual float CalculatePenetrationKrupp(float impactFactor, float velocity, float weight, float armorMaterialConstant, float diameter) {
            // Using Krupp formula: P = (iF * V² * W * K) / D²
            return (impactFactor * velocity * velocity * weight * armorMaterialConstant) / (diameter * diameter);
        }

        protected virtual float CalculateEffectiveArmor(float thickness, float angle)
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

        protected override void HandleGenericCollision(Collision collision, GameObject hitObject)
        {
            Destroy(this);
        }

        public override void EditorSetUp()
        {
        }
    }
}