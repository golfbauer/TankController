using System;
using Controller.Scripts.Managers.ImpactCollision;
using Unity.VisualScripting;
using UnityEngine;

namespace Controller.Scripts.Managers.Ammunition.Projectile
{
    public abstract class BaseProjectile : MonoBehaviour
    {
        public float caliber;
        public float mass;
        public float initVelocity;
        
        public float maxTravelDistance = 1000f;
        public float maxLifetime = 10f;

        protected Rigidbody Rigidbody;
        protected Vector3 InitialPosition;

        protected void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
            
            if (Rigidbody == null)
            {
                Rigidbody = this.AddComponent<Rigidbody>();
            }
            
            Rigidbody.useGravity = true;
            Rigidbody.isKinematic = false;
            Rigidbody.mass = mass;
            Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        protected virtual void Start()
        {
            InitialPosition = transform.position;
            Rigidbody.velocity = transform.forward * initVelocity;
            Destroy(gameObject, maxLifetime);
        }

        protected virtual void FixedUpdate()
        {
            ApplyAirResistance();
            
            if (Vector3.Distance(InitialPosition, transform.position) > maxTravelDistance)
            {
                Destroy(gameObject);
            }
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            var hitObject = collision.gameObject;
            
            CollisionManager collisionManager = hitObject.GetComponent<CollisionManager>();
        
            if (collisionManager != null)
            {
                HandleTankCollision(collision, hitObject);
            }
            else
            {
                HandleGenericCollision(collision, hitObject);
            }
        }

        protected abstract void HandleTankCollision(Collision collision, GameObject hitTank);
        
        protected abstract void HandleGenericCollision(Collision collision, GameObject hitObject);
        
        protected virtual Vector3 GetHitPoint(Vector3 hitPoint)
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
        
        protected virtual ArmorSection GetArmorSection(GameObject hitTank, Collision collision)
        {
            CollisionManager collisionManager = hitTank.GetComponent<CollisionManager>();
            Vector3 actualHit = GetHitPoint(collision.contacts[0].point);
            return collisionManager.HandleImpact(actualHit, hitTank.transform);
        }

        protected virtual void ApplyAirResistance()
        {
            float C_d = GetDragCoefficient();
            float rho = 1.225f;
            float A = GetReferenceArea(); 
            float v = Rigidbody.velocity.magnitude;
    
            Vector3 dragForce = -0.5f * C_d * rho * A * v * v * Rigidbody.velocity.normalized;

            Rigidbody.AddForce(dragForce);
        }

        protected abstract float GetDragCoefficient();

        protected abstract float GetReferenceArea();
        
        public abstract void EditorSetUp();
    }
}