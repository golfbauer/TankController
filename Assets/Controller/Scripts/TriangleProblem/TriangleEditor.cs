using UnityEditor;
using UnityEngine;

namespace Controller.Scripts.TriangleProblem
{
    [CustomEditor(typeof(Triangle))]
    public class TriangleEditor : Editor
    {
        SerializedProperty A;
        SerializedProperty B;
        SerializedProperty C;

        SerializedProperty P;

        private Vector3 _planeNormal;
        private Vector3 _center;

        private void OnEnable()
        {
            // Transform A, B, and C
            A = serializedObject.FindProperty("A");
            B = serializedObject.FindProperty("B");
            C = serializedObject.FindProperty("C");

            // Transform P
            P = serializedObject.FindProperty("P");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(A);
            EditorGUILayout.PropertyField(B);
            EditorGUILayout.PropertyField(C);

            EditorGUILayout.PropertyField(P);

            serializedObject.ApplyModifiedProperties();
        }

        private void OnSceneGUI()
        {
            Triangle triangle = target as Triangle;

            if (triangle == null)
            {
                return;
            }

            triangle.A.position = Handles.PositionHandle(triangle.A.position, triangle.A.rotation);
            triangle.B.position = Handles.PositionHandle(triangle.B.position, triangle.B.rotation);
            triangle.C.position = Handles.PositionHandle(triangle.C.position, triangle.C.rotation);

            triangle.P.position = Handles.PositionHandle(triangle.P.position, triangle.P.rotation);

            CalculateCenter();
            CalculateNormal();

            DrawTriangle();
            DrawAxis();

            MapToPlane1(
                triangle.A.position,
                triangle.B.position,
                triangle.C.position,
                triangle.P.position
            );
            // MapToPlane2(
            //     triangle.A.position,
            //     triangle.B.position,
            //     triangle.C.position,
            //     triangle.P.position
            // );
        }

        private void DrawTriangle()
        {
            Triangle triangle = target as Triangle;

            if (triangle == null)
            {
                return;
            }

            Handles.DrawLine(triangle.A.position, triangle.B.position);
            Handles.DrawLine(triangle.B.position, triangle.C.position);
            Handles.DrawLine(triangle.C.position, triangle.A.position);

            Handles.DrawLine(_center, _center + _planeNormal * 0.3f);
        }

        private void DrawAxis()
        {
            Handles.color = Color.red;
            Handles.DrawLine(Vector3.zero, Vector3.left * 3);
            Handles.DrawLine(Vector3.zero, Vector3.up * 3);
            Handles.DrawLine(Vector3.zero, Vector3.forward * 3);
            Handles.color = Color.white;
        }

        private void CalculateCenter()
        {
            Triangle triangle = target as Triangle;

            if (triangle == null)
            {
                return;
            }

            _center = (triangle.A.position + triangle.B.position + triangle.C.position) / 3;
        }

        private void CalculateNormal()
        {
            Triangle triangle = target as Triangle;

            if (triangle == null)
            {
                return;
            }

            _planeNormal = Vector3
                .Cross(
                    triangle.B.position - triangle.A.position,
                    triangle.C.position - triangle.A.position
                )
                .normalized;
        }

        private void MapToPlane1(Vector3 A, Vector3 B, Vector3 C, Vector3 P)
        {
            int dim1,
                dim2;

            // Choose the two dimensions to project onto
            if (
                Mathf.Abs(_planeNormal.x)
                <= Mathf.Min(Mathf.Abs(_planeNormal.y), Mathf.Abs(_planeNormal.z))
            )
            {
                dim1 = 1; // y
                dim2 = 2; // z
            }
            else if (
                Mathf.Abs(_planeNormal.y)
                <= Mathf.Min(Mathf.Abs(_planeNormal.x), Mathf.Abs(_planeNormal.z))
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
            var A2D = new Vector2(A[dim1], A[dim2]);
            var B2D = new Vector2(B[dim1], B[dim2]);
            var C2D = new Vector2(C[dim1], C[dim2]);
            var P2D = new Vector2(P[dim1], P[dim2]);

            // Draw the 2D triangle
            Handles.DrawLine(A2D, B2D);
            Handles.DrawLine(B2D, C2D);
            Handles.DrawLine(C2D, A2D);
            Handles.color = Color.red;
            Handles.DrawWireDisc(P2D, _planeNormal, 0.05f);
            Handles.color = Color.white;
        }

        private void MapToPlane2(Vector3 A, Vector3 B, Vector3 C, Vector3 P)
        {
            //a = sqrt((x2-x1)^2 + (y2-y1)^2 + (z2-z1)^2)
            //b = sqrt((x3-x2)^2 + (y3-y2)^2 + (z3-z2)^2)
            //c = sqrt((x1-x3)^2 + (y1-y3)^2 + (z1-z3)^2)
            var a = Mathf.Sqrt(
                Mathf.Pow(B.x - A.x, 2) + Mathf.Pow(B.y - A.y, 2) + Mathf.Pow(B.z - A.z, 2)
            );

            var b = Mathf.Sqrt(
                Mathf.Pow(C.x - B.x, 2) + Mathf.Pow(C.y - B.y, 2) + Mathf.Pow(C.z - B.z, 2)
            );

            var c = Mathf.Sqrt(
                Mathf.Pow(A.x - C.x, 2) + Mathf.Pow(A.y - C.y, 2) + Mathf.Pow(A.z - C.z, 2)
            );

            // A: [0,0]
            // B: [a,0]
            A = Vector3.zero;
            B = new Vector3(a, 0, 0);

            // C: [xc, yc]
            // yc = sqrt((a + b - c) (a - b + c) (-a + b + c) (a + b + c))/(2 a)
            // xc = sqrt(c^2 - yc^2)
            var yC = Mathf.Sqrt((a + b - c) * (a - b + c) * (-a + b + c) * (a + b + c)) / (2 * a);
            var xC = Mathf.Sqrt(Mathf.Pow(c, 2) - Mathf.Pow(yC, 2));
            C = new Vector3(xC, yC, 0);

            // P: [xp, yp]
            // yp = sqrt((a + b - p) (a - b + p) (-a + b + p) (a + b + p))/(2 a)
            // xp = sqrt(p^2 - yp^2)
            var p = Mathf.Sqrt(
                Mathf.Pow(P.x - A.x, 2) + Mathf.Pow(P.y - A.y, 2) + Mathf.Pow(P.z - A.z, 2)
            );
            var yP = Mathf.Sqrt((a + b - p) * (a - b + p) * (-a + b + p) * (a + b + p)) / (2 * a);
            var xP = Mathf.Sqrt(Mathf.Pow(p, 2) - Mathf.Pow(yP, 2));
            P = new Vector3(xP, yP, 0);

            // Draw the 2D triangle
            Handles.DrawLine(A, B);
            Handles.DrawLine(B, C);
            Handles.DrawLine(C, A);
            Handles.color = Color.red;
            Handles.DrawWireDisc(P, _planeNormal, 0.05f);
            Handles.color = Color.white;
        }
    }
}
