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
        
        protected void AttachRigidbody(GameObject gameObject, SerializedProperty massProp)
        {
            Rigidbody wheelRigidbody = gameObject.AddComponent<Rigidbody>();
            wheelRigidbody.mass = massProp.floatValue;
        }
        
        protected void AttachWheelScript(GameObject wheel, bool isLeft)
        {
            Wheel wheelController = wheel.AddComponent<Wheel>();
            
            wheelController.WheelManager = Transform.parent.GetComponent<WheelManager>();
            wheelController.IsLeftWheel = isLeft;
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
    }
}