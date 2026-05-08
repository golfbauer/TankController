using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Controller.Scripts.Utils
{
    public class TankComponentEditor : Editor
    {
        public Transform transform;
        public bool update;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (IsInPrefabStage())
            {
                ShowDenyAccessMessage();
                return;
            }

            SetUpGUI();

            update = ShouldUpdate();
            if (update)
            {
                ApplyUpdate();
                ApplyModification(parent: transform.gameObject);
                update = false;
            }
        }

        /// <summary>
        /// This function is called on each OnInspectorGUI call.
        /// It is used to display the GUI elements of the Editor
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void SetUpGUI()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called whenever the updateAll flag is set to True on the OnInspectorGUI function.
        /// </summary>
        public virtual void ApplyUpdate() { }

        /// <summary>
        /// Check if the current scene is in prefab stage.
        /// </summary>
        public virtual bool IsInPrefabStage()
        {
            return PrefabStageUtility.GetCurrentPrefabStage() is null;
        }

        /// <summary>
        /// Display the deny access message.
        /// </summary>
        public virtual void ShowDenyAccessMessage()
        {
            GUIUtils.DenyAccessGUI();
        }

        /// <summary>
        /// Checks whether Updates should be applied.
        /// Set update to True to manually trigger an update.
        /// Update will be automatically triggered if the GUI changed or an Undo/Redo was performed.
        /// </summary>
        /// <returns>Whether to update</returns>
        public bool ShouldUpdate()
        {
            return GUI.changed || Event.current.commandName == "UndoRedoPerformed" || update;
        }

        /// <summary>
        /// Can be added to the SetUpGUI function to update all Components at once.
        /// </summary>
        public virtual void UpdateAllGUI()
        {
            update = GUIUtils.UpdateAllGUI();
        }

        private void ApplyModification(GameObject parent)
        {
            serializedObject.ApplyModifiedProperties();
            Selection.activeGameObject = null;
            Selection.activeGameObject = parent;
            EditorUtility.SetDirty(transform.gameObject);
        }

        public virtual void UpdateMesh(
            Transform thisTransform,
            SerializedProperty mesh,
            SerializedProperty materials
        )
        {
            var meshRenderer = thisTransform.GetComponent<MeshRenderer>();
            if (!meshRenderer)
                meshRenderer = thisTransform.gameObject.AddComponent<MeshRenderer>();

            var meshFilter = thisTransform.GetComponent<MeshFilter>();
            if (!meshFilter)
                meshFilter = thisTransform.gameObject.AddComponent<MeshFilter>();

            meshFilter.mesh = mesh.objectReferenceValue as Mesh;
            var meshRendererMaterials = new Material[materials.arraySize];

            for (var i = 0; i < materials.arraySize; i++)
                meshRendererMaterials[i] =
                    materials.GetArrayElementAtIndex(i).objectReferenceValue as Material;

            meshRenderer.sharedMaterials = meshRendererMaterials;
        }

        public virtual void UpdateMeshColliders(
            Transform thisTransform,
            SerializedProperty newMeshColliders,
            SerializedProperty colliderMaterial = null
        )
        {
            RemoveMeshColliders(thisTransform);

            for (var i = 0; i < newMeshColliders.arraySize; i++)
            {
                var meshCollider = thisTransform.gameObject.AddComponent<MeshCollider>();
                meshCollider.sharedMesh =
                    newMeshColliders.GetArrayElementAtIndex(i).objectReferenceValue as Mesh;
                meshCollider.convex = true;

                if (colliderMaterial != null)
                    meshCollider.material = colliderMaterial.objectReferenceValue as PhysicsMaterial;
            }
        }

        public virtual void RemoveMeshColliders(Transform thisTransform)
        {
            var oldMeshCollidersArray = thisTransform.GetComponents<MeshCollider>();

            if (oldMeshCollidersArray.Length > 0)
                for (var i = 0; i < oldMeshCollidersArray.Length; i++)
                    DestroyImmediate(oldMeshCollidersArray[i]);
        }

        public virtual BoxCollider UpdateBoxCollider(Transform thisTransform)
        {
            var boxCollider = thisTransform.GetComponent<BoxCollider>();

            if (!boxCollider)
                boxCollider = thisTransform.gameObject.AddComponent<BoxCollider>();

            return boxCollider;
        }

        public virtual BoxCollider UpdateBoxCollider(
            Transform thisTransform,
            SerializedProperty colliderCenter
        )
        {
            var boxCollider = UpdateBoxCollider(thisTransform);
            boxCollider.center = colliderCenter.vector3Value;

            return boxCollider;
        }

        public virtual BoxCollider UpdateBoxCollider(
            Transform thisTransform,
            SerializedProperty colliderCenter,
            SerializedProperty colliderSize
        )
        {
            var boxCollider = UpdateBoxCollider(thisTransform, colliderCenter);
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
            var boxCollider = UpdateBoxCollider(thisTransform, colliderCenter, colliderSize);
            boxCollider.material = colliderMaterial.objectReferenceValue as PhysicsMaterial;

            return boxCollider;
        }

        public virtual void RemoveBoxCollider(Transform thisTransform)
        {
            var boxCollider = thisTransform.GetComponent<BoxCollider>();

            if (boxCollider)
                DestroyImmediate(boxCollider);
        }

        public virtual SphereCollider UpdateSphereCollider(Transform thisTransform)
        {
            var sphereCollider = thisTransform.GetComponent<SphereCollider>();

            if (!sphereCollider)
                sphereCollider = thisTransform.gameObject.AddComponent<SphereCollider>();

            return sphereCollider;
        }

        public virtual SphereCollider UpdateSphereCollider(
            Transform thisTransform,
            SerializedProperty radius
        )
        {
            var sphereCollider = UpdateSphereCollider(thisTransform);
            sphereCollider.radius = radius.floatValue;

            return sphereCollider;
        }

        public virtual SphereCollider UpdateSphereCollider(
            Transform thisTransform,
            SerializedProperty radius,
            SerializedProperty material
        )
        {
            var sphereCollider = UpdateSphereCollider(thisTransform, radius);
            sphereCollider.material = material.objectReferenceValue as PhysicsMaterial;

            return sphereCollider;
        }

        public virtual Rigidbody UpdateRigidbody(
            Transform thisTransform,
            bool isKinematic = false,
            bool useGravity = true
        )
        {
            var rigidbody = thisTransform.GetComponent<Rigidbody>();
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
            var rigidbody = UpdateRigidbody(thisTransform, isKinematic, useGravity);

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
            var rigidbody = UpdateRigidbody(thisTransform, hullMass, isKinematic, useGravity);

            rigidbody.angularDamping = angularDrag.floatValue;

            return rigidbody;
        }
    }
}
