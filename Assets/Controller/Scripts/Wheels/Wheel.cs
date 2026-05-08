using System.Collections.Generic;
using UnityEngine;

namespace Controller.Scripts.Wheels
{
    public class Wheel : MonoBehaviour
    {
        // Debug settings
        public bool showLabels;

        // Wheel Settings
        // Right
        public Vector3 rightWheelRotation = Vector3.zero;
        public Vector3 rightWheelTorque = Vector3.right;
        public Vector3 rightWheelHingeAxis = Vector3.right;

        // Left
        public Vector3 leftWheelRotation = Vector3.zero;
        public Vector3 leftWheelTorque = Vector3.right;
        public Vector3 leftWheelHingeAxis = Vector3.right;

        // Left Mesh
        public Mesh leftWheelMesh;
        public List<Material> leftWheelMaterials;

        // Right Mesh
        public Mesh rightWheelMesh;
        public List<Material> rightWheelMaterials;

        // Collider
        public float wheelColliderRadius = 0.3f;
        public PhysicsMaterial wheelColliderMaterial;

        // Relation
        public float wheelDistance = 0.1f;
        public float wheelSpacing = 0.1f;

        // Resize Script
        public bool resizeWheel;
        public float wheelResizeScale = 1;
        public float wheelResizeSpeed = 1;
    }
}
