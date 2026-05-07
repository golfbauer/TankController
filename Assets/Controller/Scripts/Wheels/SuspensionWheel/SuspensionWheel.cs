using System.Collections.Generic;
using UnityEngine;

namespace Controller.Scripts.Wheels.SuspensionWheel
{
    public class SuspensionWheel : Wheel
    {
        // General
        public int componentCount;
        public float componentSpacing;
        public float componentDistance;

        //Wheel
        // Relation
        public float wheelOffset;

        // Rigidbody settings
        public float wheelMass = 50f;

        // Suspension
        //Rotation
        public float suspensionRotation;

        // Right Mesh settings
        public Mesh rightSuspensionMesh;
        public List<Material> rightSuspensionMaterials;

        // Left Mesh settings
        public Mesh leftSuspensionMesh;
        public List<Material> leftSuspensionMaterials;

        // Rigidbody settings
        public float suspensionMass = 30f;

        // HingeJoint settings
        public Vector3 anchorOffset = Vector3.zero;
        public float springForce;
        public float damperForce;
        public float springTargetPosition;

        public float minLimitAngle;
        public float maxLimitAngle;

        // Relation
        public float suspensionDistance;
        public float suspensionSpacing;
    }
}
