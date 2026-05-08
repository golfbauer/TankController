using System;
using System.Collections.Generic;
using UnityEngine;

namespace Controller.Scripts.ImpactCollision
{
    [Serializable]
    public class ArmorSection
    {
        public List<Vector3> connectingPoints;
        public float thickness;
        public ArmorMaterialType armorMaterialType = ArmorMaterialType.LowCarbonSteelPlate;

        private Vector3 _a;
        private Vector3 _b;
        private Vector3 _c;
        private Vector3 _d;

        private Vector3 _leftPlaneNormal;
        private float _leftPlaneD;
        
        private Vector3 _rightPlaneNormal;
        private float _rightPlaneD;

        public ArmorSection(
            List<Vector3> connectingPoints,
            float thickness = CollisionUtils.DefaultThickness,
            float tolerance = CollisionUtils.CollisionTolerance
        )
        {
            if (connectingPoints == null || connectingPoints.Count != 4)
                throw new ArgumentException("New armor section must have 4 connecting points");
            this.connectingPoints = connectingPoints;
            this.thickness = thickness;
            _a = connectingPoints[0];
            _b = connectingPoints[1];
            _c = connectingPoints[2];
            _d = connectingPoints[3];
            
            CalculateLeftPlane();
            CalculateRightPlane();
        }

        private void CalculateLeftPlane()
        {
            // Calculate the normal of the left plane
            _leftPlaneNormal = Vector3.Cross(_b - _a, _c - _a).normalized;

            // Find d for the left plane equation => ax + by + cz + d = 0
            _leftPlaneD = -Vector3.Dot(_leftPlaneNormal, _a);
        }
        
        private void CalculateRightPlane()
        {
            // Calculate the normal of the right plane
            _rightPlaneNormal = Vector3.Cross(_c - _b, _d - _b).normalized;

            // Find d for the right plane
            _rightPlaneD = -Vector3.Dot(_rightPlaneNormal, _b);
        }

        public bool IsImpactPointWithinArmorSection(Vector3 impactPoint, float tolerance)
        {
            // Calculate the distance of the point from the left triangle
            float leftDistance = Mathf.Abs(
                _leftPlaneNormal.x * impactPoint.x
                    + _leftPlaneNormal.y * impactPoint.y
                    + _leftPlaneNormal.z * impactPoint.z
                    + _leftPlaneD
            );
            
            // Check if the distance is within a predefined tolerance &&
            // Check if the point is inside the quadrilateral (by checking if it's inside any of the two triangles)
            if (leftDistance <= tolerance && IsPointInTriangle(impactPoint, _b, _c, _d))
                return true;
            
            float rightDistance = Mathf.Abs(
                _rightPlaneNormal.x * impactPoint.x
                + _rightPlaneNormal.y * impactPoint.y
                + _rightPlaneNormal.z * impactPoint.z
                + _rightPlaneD
            );

            if (rightDistance <= tolerance && IsPointInTriangle(impactPoint, _a, _b, _d))
                return true;
           
            return false;
        }

        private bool IsPointInTriangle(Vector3 P, Vector3 A, Vector3 B, Vector3 C)
        {
            int dim1,
                dim2;

            // Choose the two dimensions to project onto
            if (
                Mathf.Abs(_leftPlaneNormal.x)
                >= Mathf.Max(Mathf.Abs(_leftPlaneNormal.y), Mathf.Abs(_leftPlaneNormal.z))
            )
            {
                dim1 = 1; // y
                dim2 = 2; // z
            }
            else if (
                Mathf.Abs(_leftPlaneNormal.y)
                >= Mathf.Max(Mathf.Abs(_leftPlaneNormal.x), Mathf.Abs(_leftPlaneNormal.z))
            )
            {
                dim1 = 0; // x
                dim2 = 2; // z
            }
            else
            {
                dim1 = 0; // x
                dim2 = 1; // y
            }

            // Project the 3D points onto the 2D plane by picking the two dimensions
            var P2D = new Vector2(P[dim1], P[dim2]);
            var A2D = new Vector2(A[dim1], A[dim2]);
            var B2D = new Vector2(B[dim1], B[dim2]);
            var C2D = new Vector2(C[dim1], C[dim2]);

            // Use the 2D method to check if the point is inside the triangle
            var d1 = Sign(P2D, A2D, B2D);
            var d2 = Sign(P2D, B2D, C2D);
            var d3 = Sign(P2D, C2D, A2D);

            var hasNeg = d1 < 0 || d2 < 0 || d3 < 0;
            var hasPos = d1 > 0 || d2 > 0 || d3 > 0;

            return !(hasNeg && hasPos);
        }

        private float Sign(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
        }
    }
}
