using System;
using UnityEngine;

namespace Controller.Scripts.Managers.Wheels
{
    public class Wheel: MonoBehaviour
    {
        public WheelManager WheelManager { get; set; }
        
        public bool IsLeftWheel;
        
        public void Start()
        {
            if (WheelManager == null)
                WheelManager = transform.parent.GetComponent<WheelManager>();
        }
        
        private void FixedUpdate()
        {
            float rotationSpeed;
            Vector3 rotationDirection;
            
            if (IsLeftWheel)
            {
                rotationSpeed = WheelManager.LeftRotationSpeed;
                rotationDirection = WheelManager.LeftRotationDirection;
            }
            else
            {
                rotationSpeed = WheelManager.RightRotationSpeed;
                rotationDirection = WheelManager.RightRotationDirection;
            }
            
            transform.Rotate(rotationDirection, rotationSpeed * Time.fixedDeltaTime, Space.Self);
        }
    }
}