using System.Collections.Generic;
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
        public float wheelMass = 50f;

        // Suspension
        //Rotation
        public float suspensionRotation = 0f;
        
        // Right Mesh settings
        public Mesh rightSuspensionMesh = null;
        public List<Material> rightSuspensionMaterials = null;
        
        // Left Mesh settings
        public Mesh leftSuspensionMesh = null;
        public List<Material> leftSuspensionMaterials = null;
        
        // Rigidbody settings
        public float suspensionMass = 30f;
        
        // HingeJoint settings
        public Vector3 anchorOffset = Vector3.zero;
        public float springForce = 0f;
        public float damperForce = 0f;
        public float springTargetPosition = 0f;
        
        public float minLimitAngle = 0f;
        public float maxLimitAngle = 0f;
        
        // Relation
        public float suspensionDistance = 0f;
        public float suspensionSpacing = 0f;
    }
}