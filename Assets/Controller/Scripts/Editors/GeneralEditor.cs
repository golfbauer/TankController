using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Controller.Scripts.Editors
{
    public class GeneralEditor: Editor
    {
        
        protected bool UpdateAll;
        protected Transform Transform;
        
        protected void OnEnable()
        {
            throw new NotImplementedException();
        }

        protected void SetUpGUI()
        {
            UpdateAllGUI();
        }
        
        protected void UpdateAllGUI()
        {
            if(GUILayout.Button(GeneralMessages.UpdateAll))
                UpdateAll = true;
        }
        
        protected void UpdateComponents()
        {
            throw new NotImplementedException();
        }
        
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
                UpdateComponents();
                serializedObject.ApplyModifiedProperties();
                RefreshParentSelection(Transform.gameObject);
                EditorUtility.SetDirty(Transform.gameObject);
            }
        }
        
        protected void RefreshParentSelection(GameObject parent)
        {
            Selection.activeGameObject = null;
            Selection.activeGameObject = parent;
        }
    }
}