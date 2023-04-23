using UnityEditor;
using UnityEngine;

namespace Controller.Scripts.Editors.Wheels.SupportWheel
{
    [CustomEditor(typeof(CreateSupportWheel))]
    [CanEditMultipleObjects]
    public class CreateSupportWheelEditor: CreateWheelEditor
    {
        protected override void InitComponents(GameObject wheel)
        {
            InitCollider(wheel);
            InitWheelScript(wheel);
            InitRigidbody(wheel);
            
            if(WheelMeshProp.objectReferenceValue != null && WheelMaterialProp.objectReferenceValue != null)
                InitMesh(wheel);
        }
    }
}