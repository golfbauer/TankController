using System.Collections.Generic;
using UnityEngine;

namespace Controller.Scripts
{
    public class TankController : MonoBehaviour
    {
        [SerializeField] private GameObject tank;
    
        [Header("Physics Settings")]
        [SerializeField] private float mass;
        [SerializeField] private float drag;
        [SerializeField] private float angularDrag;

        private Rigidbody _tankRigidbody;
        private TrackController _trackController;

        private void Start()
        {
            _tankRigidbody = tank.GetComponent<Rigidbody>();
            if (_tankRigidbody != null) return;
            
            _tankRigidbody = tank.AddComponent<Rigidbody>();
            
            SetUpRigidBody();
        }
    
        private void SetUpRigidBody()
        {
            _tankRigidbody.mass = mass;
            _tankRigidbody.drag = drag;
            _tankRigidbody.angularDrag = angularDrag;
        }
    }
}
