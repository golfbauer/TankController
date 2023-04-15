using System;
using UnityEngine;

namespace Controller.Scripts.Test
{
    public class Wheels: MonoBehaviour
    {
        [SerializeField] private float rotationSpeed;
        private void FixedUpdate()
        {
            transform.Rotate(Vector3.back, rotationSpeed, Space.Self);
        }
    }
}