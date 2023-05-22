﻿using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace Controller.Scripts.Editors.Utils
{
    public class TankComponentEditor : Editor
    {
        public Transform transform;
        public bool updateAll;
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update ();

            if (AllowAccess())
            {
                DenyAccessMessage();
                return;
            }

            SetUpGUI();

            if (GUI.changed || Event.current.commandName == "UndoRedoPerformed")
            {
                updateAll = true;
            }

            if (updateAll)
            {
                BulkUpdateComponents();
                serializedObject.ApplyModifiedProperties();
                RefreshParentSelection(transform.gameObject);
                EditorUtility.SetDirty(transform.gameObject);
            }
        }

        public virtual bool AllowAccess()
        {
            return PrefabStageUtility.GetCurrentPrefabStage() == null;
        }

        public virtual void DenyAccessMessage()
        {
            GUIUtils.DenyAccessGUI();
        }
        
        public virtual void SetUpGUI()
        {
            throw new NotImplementedException();
        }
        
        public virtual void BulkUpdateComponents()
        {
            return;
        }
        
        private void RefreshParentSelection(GameObject parent)
        {
            Selection.activeGameObject = null;
            Selection.activeGameObject = parent;
        }
        
        public virtual void UpdateMesh(Transform thisTransform, SerializedProperty mesh, SerializedProperty materials)
        {
            MeshRenderer meshRenderer = thisTransform.GetComponent<MeshRenderer>();
            if(!meshRenderer)
                meshRenderer = thisTransform.gameObject.AddComponent<MeshRenderer>();
            
            MeshFilter meshFilter = thisTransform.GetComponent<MeshFilter>();
            if(!meshFilter)
                meshFilter = thisTransform.gameObject.AddComponent<MeshFilter>();
            
            meshFilter.mesh = mesh.objectReferenceValue as Mesh;
            Material[] meshRendererMaterials = new Material[materials.arraySize];
            
            for (int i = 0; i < materials.arraySize; i++)
            {
                meshRendererMaterials[i] = materials.GetArrayElementAtIndex(i).objectReferenceValue as Material;
            }
            
            meshRenderer.sharedMaterials = meshRendererMaterials;
        }

        public virtual void UpdateMeshColliders(Transform thisTransform, SerializedProperty newMeshColliders, SerializedProperty colliderMaterial = null)
        {
            RemoveMeshColliders(thisTransform);
            
            for (int i = 0; i < newMeshColliders.arraySize; i++)
            {
                MeshCollider meshCollider = thisTransform.gameObject.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = newMeshColliders.GetArrayElementAtIndex(i).objectReferenceValue as Mesh;
                
                if (colliderMaterial != null)
                    meshCollider.material = colliderMaterial.objectReferenceValue as PhysicMaterial;
            }
        }
        
        public virtual void RemoveMeshColliders(Transform thisTransform)
        {
            MeshCollider[] oldMeshCollidersArray = thisTransform.GetComponents<MeshCollider>();
            
            if (oldMeshCollidersArray.Length > 0)
            {
                for (int i = 0; i < oldMeshCollidersArray.Length; i++)
                {
                    DestroyImmediate(oldMeshCollidersArray[i]);
                }
            }
        }
        
        public virtual void UpdateMeshCollider(Transform thisTransform, SerializedProperty newMeshCollider, SerializedProperty colliderMaterial = null)
        {
            RemoveMeshColliders(thisTransform);
            
            MeshCollider meshCollider = thisTransform.gameObject.AddComponent<MeshCollider>();
            
            if (colliderMaterial != null)
                meshCollider.material = colliderMaterial.objectReferenceValue as PhysicMaterial;
        }

        public virtual BoxCollider UpdateBoxCollider(Transform thisTransform)
        {
            BoxCollider boxCollider = thisTransform.GetComponent<BoxCollider>();

            if (!boxCollider)
                boxCollider = thisTransform.gameObject.AddComponent<BoxCollider>();
            
            return boxCollider;
        }

        public virtual BoxCollider UpdateBoxCollider(Transform thisTransform, SerializedProperty colliderCenter)
        {
            BoxCollider boxCollider = UpdateBoxCollider(thisTransform);
            boxCollider.center = colliderCenter.vector3Value;
            
            return boxCollider;
        }

        public virtual BoxCollider UpdateBoxCollider(Transform thisTransform, SerializedProperty colliderCenter, SerializedProperty colliderSize)
        {
            BoxCollider boxCollider = UpdateBoxCollider(thisTransform, colliderCenter);
            boxCollider.size = colliderSize.vector3Value;
            
            return boxCollider;
        }

        public virtual BoxCollider UpdateBoxCollider(
            Transform thisTransform, 
            SerializedProperty colliderCenter, 
            SerializedProperty colliderSize, 
            SerializedProperty colliderMaterial)
        {
            BoxCollider boxCollider = UpdateBoxCollider(thisTransform, colliderCenter, colliderSize);
            boxCollider.material = colliderMaterial.objectReferenceValue as PhysicMaterial;
            
            return boxCollider;
        }
        
        public virtual void RemoveBoxCollider(Transform thisTransform)
        {
            BoxCollider boxCollider = thisTransform.GetComponent<BoxCollider>();

            if (boxCollider)
                DestroyImmediate(boxCollider);
        }
    }
}