using System;
using Controller.Scripts.Editors.Utils;
using Controller.Scripts.Managers.Wheels;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Controller.Scripts.Editors.Wheels
{
    public class CreateWheelEditor: TankComponentEditor
    {
        protected void BulkDestroyComponents()
        {
            var childCount = transform.childCount;
            for (var i = 0; i < childCount; i++)
                DestroyImmediate (transform.GetChild(0).gameObject);
        }
        
        protected void AttachCollider(GameObject gameObject, SerializedProperty radiusProp, SerializedProperty materialProp)
        {
            var collider = gameObject.AddComponent<SphereCollider>();
            collider.radius = radiusProp.floatValue;
            collider.center = Vector3.zero;
            collider.material = materialProp.objectReferenceValue as PhysicMaterial;
        }
        
        protected void AttachMesh(GameObject gameObject, SerializedProperty meshProp, SerializedProperty materialProp)
        {
            if(meshProp.objectReferenceValue == null || materialProp.objectReferenceValue == null)
                return;
            
            MeshFilter mesh = gameObject.AddComponent<MeshFilter>();
            mesh.mesh = meshProp.objectReferenceValue as Mesh;
            MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
            renderer.material = materialProp.objectReferenceValue as Material;
        }

        protected virtual void AttachRigidbody(GameObject gameObject, SerializedProperty massProp,
            SerializedProperty angularDragProp = null)
        {
            Rigidbody wheelRigidbody = gameObject.AddComponent<Rigidbody>();
            wheelRigidbody.mass = massProp.floatValue;
            
            if (angularDragProp != null)
                wheelRigidbody.angularDrag = angularDragProp.floatValue;
        }
        
        protected void AttachFixedRigidbody(GameObject gameObject, SerializedProperty massProp)
        {
            Rigidbody wheelRigidbody = gameObject.AddComponent<Rigidbody>();
            wheelRigidbody.mass = massProp.floatValue;
            wheelRigidbody.constraints = RigidbodyConstraints.FreezePosition;
        }
        
        protected void AttachWheelHingeJoint(GameObject wheel, Transform connectedTo, Vector3 axis)
        {
            HingeJoint wheelHingeJoint = wheel.AddComponent<HingeJoint>();
            wheelHingeJoint.anchor = Vector3.zero;
            wheelHingeJoint.axis = axis;
            wheelHingeJoint.useSpring = false;
            wheelHingeJoint.useMotor = false;
            wheelHingeJoint.useLimits = false;
            wheelHingeJoint.connectedBody = connectedTo.GetComponent<Rigidbody>();
        }

        protected void AttachWheelScript(GameObject wheel, bool isLeft, Vector3 torqueDirection)
        {
            Wheel wheelController = wheel.AddComponent<Wheel>();
            
            wheelController.WheelManager = transform.parent.GetComponent<WheelManager>();
            wheelController.isLeftWheel = isLeft;
            wheelController.torqueDirection = torqueDirection;
        }
        
        protected void AttachWheelManager(float wheelRadius)
        {
            WheelManager wheelManager = transform.GetComponent<WheelManager>();
            if (wheelManager == null)
                wheelManager = transform.gameObject.AddComponent<WheelManager>();
            
            wheelManager.wheelRadius = wheelRadius;
        }

        protected void UpdateAllGUI()
        {
            if(GUILayout.Button(WheelUtilsMessages.UpdateAll))
                updateAll = true;
        }

        protected virtual void OnSceneGUI()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform wheel = transform.GetChild(i);
                
                if (!wheel.name.Contains("Wheel"))
                    continue;
                
                Vector3 hingeAxis = wheel.GetComponent<HingeJoint>().axis;
                Vector3 torqueDir = wheel.GetComponent<Wheel>().torqueDirection;
                DrawUtils.DrawCircleWithDirection(wheel,hingeAxis, torqueDir, 0.1f);
            }
        }

        protected virtual void SetLayers()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform components = transform.GetChild(i);
                if (components.name.Contains("Wheel"))
                    LayerUtils.SetLayer(components.gameObject, LayerUtils.WheelLayer);
            }
        }
    }
}