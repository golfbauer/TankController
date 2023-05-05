using System;
using UnityEngine;

namespace Controller.Scripts.Test
{
    public class Wheels: MonoBehaviour
    {
        [SerializeField] private float maxAngularVelocity;
        [SerializeField] private float addTorque;
        [SerializeField] private float drag;
        [SerializeField] private Vector3 torqueDirection;
        
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            _rigidbody.maxAngularVelocity = maxAngularVelocity;
            _rigidbody.AddRelativeTorque(torqueDirection * addTorque);
            _rigidbody.angularDrag = drag;
        }
    }
}