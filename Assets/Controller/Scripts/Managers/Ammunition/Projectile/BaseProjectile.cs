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

        protected virtual void Start()
        {
            Rigidbody = GetComponent<Rigidbody>();
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
        
            if (hitObject != null)
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