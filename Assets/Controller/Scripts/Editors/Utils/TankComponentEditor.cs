using System;
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
                updateAll = false;
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

        public virtual void CheckTarget()
        {
            if (target == null)
            {
                Debug.LogError(GeneralMessages.TargetNotAssigned);
            }
        }
        
        public virtual void SetUpGUI()
        {
            throw new NotImplementedException();
        }

        public virtual void UpdateAllGUI()
        {
            EditorGUILayout.Space();
            if(GUILayout.Button(GeneralMessages.UpdateAll))
                updateAll = true;
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

        public virtual void UpdateMeshColliders(
            Transform thisTransform, 
            SerializedProperty newMeshColliders, 
            SerializedProperty colliderMaterial = null
            )
        {
            RemoveMeshColliders(thisTransform);
            
            for (int i = 0; i < newMeshColliders.arraySize; i++)
            {
                MeshCollider meshCollider = thisTransform.gameObject.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = newMeshColliders.GetArrayElementAtIndex(i).objectReferenceValue as Mesh;
                meshCollider.convex = true;
                
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

        public virtual BoxCollider UpdateBoxCollider(Transform thisTransform, 
            SerializedProperty colliderCenter, 
            SerializedProperty colliderSize
            )
        {
            BoxCollider boxCollider = UpdateBoxCollider(thisTransform, colliderCenter);
            boxCollider.size = colliderSize.vector3Value;
            
            return boxCollider;
        }

        public virtual BoxCollider UpdateBoxCollider(
            Transform thisTransform, 
            SerializedProperty colliderCenter, 
            SerializedProperty colliderSize, 
            SerializedProperty colliderMaterial
            )
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
        
        public virtual SphereCollider UpdateSphereCollider(Transform thisTransform)
        {
            SphereCollider sphereCollider = thisTransform.GetComponent<SphereCollider>();

            if (!sphereCollider)
                sphereCollider = thisTransform.gameObject.AddComponent<SphereCollider>();
            
            return sphereCollider;
        }
        
        public virtual SphereCollider UpdateSphereCollider(Transform thisTransform, SerializedProperty radius)
        {
            SphereCollider sphereCollider = UpdateSphereCollider(thisTransform);
            sphereCollider.radius = radius.floatValue;
            
            return sphereCollider;
        }

        public virtual SphereCollider UpdateSphereCollider(
            Transform thisTransform, 
            SerializedProperty radius,
            SerializedProperty material
            )
        {
            SphereCollider sphereCollider = UpdateSphereCollider(thisTransform, radius);
            sphereCollider.material = material.objectReferenceValue as PhysicMaterial;
            
            return sphereCollider;
        }
        
        public virtual Rigidbody UpdateRigidbody(
            Transform thisTransform,
            bool isKinematic = false,
            bool useGravity = true
            )
        {
            Rigidbody rigidbody = thisTransform.GetComponent<Rigidbody>();
            if (rigidbody == null)
                rigidbody = thisTransform.gameObject.AddComponent<Rigidbody>();
            
            rigidbody.isKinematic = isKinematic;
            rigidbody.useGravity = useGravity;
            
            return rigidbody;
        }

        public virtual Rigidbody UpdateRigidbody(
            Transform thisTransform, 
            SerializedProperty hullMass,
            bool isKinematic = false,
            bool useGravity = true
        )
        {
            Rigidbody rigidbody = UpdateRigidbody(thisTransform, isKinematic, useGravity);
            
            rigidbody.mass = hullMass.floatValue;

            return rigidbody;
        }
        
        public virtual Rigidbody UpdateRigidbody(
            Transform thisTransform, 
            SerializedProperty hullMass,
            SerializedProperty angularDrag,
            bool isKinematic = false,
            bool useGravity = true
            )
        {
            Rigidbody rigidbody = UpdateRigidbody(thisTransform, hullMass, isKinematic, useGravity);
            
            rigidbody.angularDrag = angularDrag.floatValue;

            return rigidbody;
        }
    }
}