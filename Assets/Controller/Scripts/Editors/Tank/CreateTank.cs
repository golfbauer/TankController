using System;
using System.Collections.Generic;
using Controller.Scripts.Editors.Wheels;
using UnityEngine;
using CameraType = Controller.Scripts.Managers.PlayerCamera.CameraType;

namespace Controller.Scripts.Editors.Tank
{
    public class CreateTank: MonoBehaviour
    {
        // Wheels
        public WheelType wheelType;
        
        // Cameras
        public CameraType cameraType;
        
        
        // Hull
        // Rigidbody
        public float hullMass = 1f;
        public int physicsIterations = 8;
        public Vector3 hullCenterOfMass = Vector3.zero;

        // Mesh
        public Mesh hullMesh = null;
        public List<Material> hullMaterial = null;
        
        // Collider
        public Vector3 hullColliderCenter = Vector3.zero;
        public Vector3 hullColliderSize = Vector3.one;



        private void Awake()
        {
            Rigidbody tankRigidbody = gameObject.GetComponent<Rigidbody>();
            
            if (tankRigidbody == null)
            {
                tankRigidbody = gameObject.AddComponent<Rigidbody>();
            }
            
            tankRigidbody.solverIterations = physicsIterations;
            tankRigidbody.centerOfMass = hullCenterOfMass;
        }
    }
}