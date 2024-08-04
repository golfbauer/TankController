using Controller.Scripts.ImpactCollision;
using Controller.Scripts.ImpactCollision.Services;
using Unity.VisualScripting;
using UnityEngine;

namespace Controller.Scripts.Ammunition.Projectile
{
    public abstract class Projectile : MonoBehaviour
    {
        public float Diameter;
        public float Mass;
        public float InitVelocity;

        public float MaxLifetime = 10f;

        protected Rigidbody Rigidbody;

        protected virtual void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();

            if (Rigidbody == null)
            {
                Rigidbody = this.AddComponent<Rigidbody>();
            }

            Rigidbody.useGravity = true;
            Rigidbody.isKinematic = false;
            Rigidbody.drag = 0;
            Rigidbody.angularDrag = 0;
            Rigidbody.mass = Mass;
            Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        protected virtual void Start()
        {
            Rigidbody.velocity = transform.forward * InitVelocity;
            Destroy(gameObject, MaxLifetime);
        }

        protected virtual void FixedUpdate()
        {
            ApplyAirResistance(airVelocity: Vector3.zero);
        }

        protected virtual void ApplyAirResistance(
            Vector3 airVelocity,
            float cd = 0.04f,
            float pf = 1.225f
        )
        {
            Vector3 velocity = Rigidbody.velocity - airVelocity;
            Vector3 direction = velocity.normalized;
            float A = Mathf.PI * Mathf.Pow(Diameter / 2, 2); // Area
            float v = velocity.magnitude; // Velocity

            Vector3 dragForce = -0.5f * pf * A * cd * v * v * direction;

            Rigidbody.AddForce(dragForce);
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            var hitObject = collision.gameObject;

            CollisionManager collisionManager = hitObject.GetComponent<CollisionManager>();

            if (collisionManager != null)
            {
                Vector3 actualHit = GetHitPoint(collision.contacts[0].point);
                ArmorSection armorSection = collisionManager.HandleImpact(
                    actualHit,
                    hitObject.transform
                );
                HandleTankCollision(collision, hitObject, armorSection);
                return;
            }

            HandleGenericCollision(collision, hitObject);
        }

        private Vector3 GetHitPoint(Vector3 hitPoint)
        {
            Vector3 rayDirection = transform.forward;
            Vector3 rayStart = hitPoint - (rayDirection * 0.1f);
            float rayLength = 1f;

            if (Physics.Raycast(rayStart, rayDirection, out RaycastHit hitInfo, rayLength))
            {
                return hitInfo.point;
            }

            return hitPoint;
        }

        protected abstract void HandleTankCollision(
            Collision collision,
            GameObject hitTank,
            ArmorSection armorSection
        );

        protected abstract void HandleGenericCollision(Collision collision, GameObject hitObject);
    }
}
