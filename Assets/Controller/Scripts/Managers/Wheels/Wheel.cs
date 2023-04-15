using System;
using UnityEngine;

namespace Controller.Scripts.Managers.Wheels
{
    public class Wheel: MonoBehaviour
    {
        public WheelManager WheelManager { get; set; }
        public bool Left { get; set; }
        
        private void FixedUpdate()
        {
            Vector3 direction = Left ? Vector3.forward : Vector3.back;
            transform.Rotate(direction, WheelManager.RotationSpeed * Time.deltaTime);
        }
    }
}