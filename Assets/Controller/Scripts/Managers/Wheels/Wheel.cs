using System;
using UnityEngine;

namespace Controller.Scripts.Managers.Wheels
{
    public class Wheel : MonoBehaviour
    {
        public WheelManager WheelManager { get; set; }
        public Rigidbody WheelRigidbody { get; set; }

        public bool isLeftWheel;
        public Vector3 torqueDirection;

        public void Start()
        {
            if (WheelManager == null)
                WheelManager = transform.parent.GetComponent<WheelManager>();
            WheelRigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (WheelRigidbody == null)
                return;

            float torque = isLeftWheel
                ? WheelManager.leftTorque
                : WheelManager.rightTorque;
            float drag = isLeftWheel
                ? WheelManager.leftDrag
                : WheelManager.rightDrag;
            float maxSpeed = isLeftWheel
                ? WheelManager.leftTargetSpeed
                : WheelManager.rightTargetSpeed;

            WheelRigidbody.maxAngularVelocity = maxSpeed;
            WheelRigidbody.AddRelativeTorque(torqueDirection * torque);
            WheelRigidbody.angularDrag = drag;
        }
    }
}