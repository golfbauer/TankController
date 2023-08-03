using System;
using System.Collections.Generic;
using UnityEngine;

namespace Controller.Scripts.Managers.ImpactCollision
{
    [Serializable]
    public class ArmorSection
    {
        public List<Vector3> connectingPoints = new List<Vector3>();
        public float thickness;

        public ArmorMaterialType armorMaterialType =
            ArmorMaterialType.LowCarbonSteelPlate;

        public float tolerance;

        private Vector3 _planeNormal;

        public ArmorSection(List<Vector3> connectingPoints, float thickness,
            float tolerance = CollisionUtils.CollisionTolerance)
        {
            if (connectingPoints == null || connectingPoints.Count != 4)
            {
                throw new ArgumentException(
                    "connectingPoints must contain exactly 4 points.");
            }

            this.connectingPoints = connectingPoints;
            this.thickness = thickness;
            this.tolerance = tolerance;
        }

        public bool IsImpactPointWithinArmorSection(Vector3 impactPoint,
            Vector3 objectPosition)
        {
            Vector3 A = connectingPoints[0] + objectPosition;
            Vector3 B = connectingPoints[1] + objectPosition;
            Vector3 C = connectingPoints[2] + objectPosition;
            Vector3 D = connectingPoints[3] + objectPosition;

            _planeNormal = Vector3.Cross(B - A, C - A).normalized;

            // Find d for the plane equation => ax + by + cz + d = 0
            float planeD = -Vector3.Dot(_planeNormal, A);

            // Calculate the distance of the point from the plane
            float distance = Mathf.Abs(_planeNormal.x * impactPoint.x +
                                       _planeNormal.y * impactPoint.y +
                                       _planeNormal.z * impactPoint.z +
                                       planeD);

            // Check if the distance is within some tolerance
            if (distance > tolerance)
            {
                return false;
            }

            // Check if the point is inside the quadrilateral (by checking if it's inside any of the two triangles)
            if (
                IsPointInTriangle(impactPoint, A, B, D) ||
                IsPointInTriangle(impactPoint, B, C, D)
            )
            {
                return true;
            }

            return false;
        }

        private bool IsPointInTriangle(Vector3 P, Vector3 A, Vector3 B,
            Vector3 C)
        {
            int dim1, dim2;

            // Choose the two dimensions to project onto
            if (Mathf.Abs(_planeNormal.x) <=
                Mathf.Min(Mathf.Abs(_planeNormal.y),
                    Mathf.Abs(_planeNormal.z)))
            {
                dim1 = 1; // y
                dim2 = 2; // z
            }
            else if (Mathf.Abs(_planeNormal.y) <=
                     Mathf.Min(Mathf.Abs(_planeNormal.x),
                         Mathf.Abs(_planeNormal.z)))
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
            Vector2 P2D = new Vector2(P[dim1], P[dim2]);
            Vector2 A2D = new Vector2(A[dim1], A[dim2]);
            Vector2 B2D = new Vector2(B[dim1], B[dim2]);
            Vector2 C2D = new Vector2(C[dim1], C[dim2]);

            // Use the 2D method to check if the point is inside the triangle
            float d1 = Sign(P2D, A2D, B2D);
            float d2 = Sign(P2D, B2D, C2D);
            float d3 = Sign(P2D, C2D, A2D);

            bool hasNeg = (d1 < 0) || (d2 < 0) || (d3 < 0);
            bool hasPos = (d1 > 0) || (d2 > 0) || (d3 > 0);

            return !(hasNeg && hasPos);
        }

        private float Sign(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return (p1.x - p3.x) * (p2.y - p3.y) -
                   (p2.x - p3.x) * (p1.y - p3.y);
        }
    }
}