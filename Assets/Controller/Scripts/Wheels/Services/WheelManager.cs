using UnityEngine;

namespace Controller.Scripts.Wheels.Services
{
    public class WheelManager : MonoBehaviour
    {
        public WheelGroupManager WheelGroupManager { get; set; }
        public Rigidbody WheelRigidbody { get; set; }

        public bool isLeftWheel;
        public Vector3 torqueDirection;

        public void Start()
        {
            if (WheelGroupManager == null)
                WheelGroupManager = transform.parent.GetComponent<WheelGroupManager>();
            WheelRigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (WheelRigidbody == null)
                return;

            float torque = isLeftWheel
                ? WheelGroupManager.leftTorque
                : WheelGroupManager.rightTorque;
            float drag = isLeftWheel ? WheelGroupManager.leftDrag : WheelGroupManager.rightDrag;
            float maxSpeed = isLeftWheel
                ? WheelGroupManager.leftTargetSpeed
                : WheelGroupManager.rightTargetSpeed;

            WheelRigidbody.maxAngularVelocity = maxSpeed;
            WheelRigidbody.AddRelativeTorque(torqueDirection * torque);
            WheelRigidbody.angularDrag = drag;
        }
    }
}
