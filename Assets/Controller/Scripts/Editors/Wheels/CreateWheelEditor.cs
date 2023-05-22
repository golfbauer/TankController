using System;
using Controller.Scripts.Managers.Wheels;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Controller.Scripts.Editors.Wheels
{
    public class CreateWheelEditor: Editor
    {
        protected bool UpdateAll;
        protected Transform Transform;

        public override void OnInspectorGUI()
        {
            serializedObject.Update ();

            if (PrefabStageUtility.GetCurrentPrefabStage() == null)
            {
                GUIUtils.DenyAccessGUI();
                return;
            }

            SetUpGUI();

            if (GUI.changed || Event.current.commandName == "UndoRedoPerformed")
            {
                UpdateAll = true;
            }

            if (UpdateAll)
            {
                BulkUpdateComponents();
                serializedObject.ApplyModifiedProperties();
                RefreshParentSelection(Transform.gameObject);
            }
        }

        protected virtual void SetUpGUI()
        {
            throw new NotImplementedException();
        }
        
        protected virtual void BulkUpdateComponents()
        {
            throw new NotImplementedException();
        }
        
        protected void BulkDestroyComponents()
        {
            var childCount = Transform.childCount;
            for (var i = 0; i < childCount; i++)
                DestroyImmediate (Transform.GetChild(0).gameObject);
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
            
            wheelController.WheelManager = Transform.parent.GetComponent<WheelManager>();
            wheelController.isLeftWheel = isLeft;
            wheelController.torqueDirection = torqueDirection;
        }
        
        protected void AttachWheelManager()
        {
            WheelManager wheelManager = Transform.GetComponent<WheelManager>();
            if (wheelManager == null)
                Transform.gameObject.AddComponent<WheelManager>();
        }

        protected void UpdateAllGUI()
        {
            if(GUILayout.Button(WheelUtilsMessages.UpdateAll))
                UpdateAll = true;
        }
        
        private void RefreshParentSelection(GameObject parent)
        {
            Selection.activeGameObject = null;
            Selection.activeGameObject = parent;
        }

        protected virtual void OnSceneGUI()
        {
            for (int i = 0; i < Transform.childCount; i++)
            {
                Transform wheel = Transform.GetChild(i);
                
                if (!wheel.name.Contains("Wheel"))
                    continue;
                
                Vector3 hingeAxis = wheel.GetComponent<HingeJoint>().axis;
                Vector3 torqueDir = wheel.GetComponent<Wheel>().torqueDirection;
                Utils.DrawCircleWithDirection(wheel,hingeAxis, torqueDir, 0.1f);
            }
        }

        protected virtual void SetLayers()
        {
            for (int i = 0; i < Transform.childCount; i++)
            {
                Transform components = Transform.GetChild(i);
                if (components.name.Contains("Wheel"))
                    LayerUtils.SetLayer(components.gameObject, LayerUtils.WheelLayer);
            }
        }
    }
}