using UnityEngine;
namespace Controller.Scripts.Editors.Wheels.SuspensionWheel
{
    public class CreateSuspensionWheel: CreateWheel
    {
        // General
        public int componentCount = 0;
        public float componentSpacing = 0f;
        public float componentDistance = 0f;
        
        //Wheel
        // Relation
        public float wheelOffset = 0f;

        // Rigidbody settings
        public float wheelMass = 1f;

        // Suspension
        // Left Settings
        public Vector3 leftSuspensionRotation = Vector3.zero;
        
        // Right Settings
        public Vector3 rightSuspensionRotation = Vector3.right;
        
        // Right Mesh settings
        public Mesh rightSuspensionMesh = null;
        public Material rightSuspensionMaterial = null;
        
        // Left Mesh settings
        public Mesh leftSuspensionMesh = null;
        public Material leftSuspensionMaterial = null;
        
        // Rigidbody settings
        public float suspensionMass = 1f;
        
        // HingeJoint settings
        public Vector3 AnchorOffset = Vector3.zero;
        public float SpringForce = 0f;
        public float DamperForce = 0f;
        public float SpringTargetPosition = 0f;
        
        public float MinLimitAngle = 0f;
        public float MaxLimitAngle = 0f;
        
        // Relation
        public float suspensionDistance = 0f;
        public float suspensionSpacing = 0f;
    }
}