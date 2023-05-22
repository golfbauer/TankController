using System;
using System.IO;
using Controller.Scripts.Editors.Utils;
using Controller.Scripts.Managers.Projectile;
using Controller.Scripts.Managers.Projectile.AmmunitionType;
using UnityEditor;
using UnityEngine;

namespace Controller.Scripts.Editors.Turret.CreateAmmunition
{
    [CustomEditor(typeof(Ammunition))]
    [CanEditMultipleObjects]
    public class CreateAmmunitionEditor : TankComponentEditor
    {
        private SerializedProperty _ammunitionTypes;
        private SerializedProperty _spawnPoint;
        private SerializedProperty _fireKey;
        private SerializedProperty _switchToNextKey;
        private SerializedProperty _switchToPreviousKey;
        private SerializedProperty _allowNumbers;
        
        private string _tmpPath = "/Assets";
        private void OnEnable()
        {
            _ammunitionTypes = serializedObject.FindProperty("ammunitionTypes");
            _spawnPoint = serializedObject.FindProperty("spawnPoint");
            _fireKey = serializedObject.FindProperty("fireKey");
            _switchToNextKey = serializedObject.FindProperty("switchToNextKey");
            _switchToPreviousKey = serializedObject.FindProperty("switchToPreviousKey");
            _allowNumbers = serializedObject.FindProperty("allowNumbers");
            
            transform = ((Ammunition) target).gameObject.transform;
        }

        public override void SetUpGUI()
        {
            GUIUtils.PropFieldGUI(_spawnPoint, "Spawn Point");
            GUIUtils.PropFieldGUI(_fireKey, "Fire Key");
            GUIUtils.PropFieldGUI(_switchToNextKey, "Switch To Next Key");
            GUIUtils.PropFieldGUI(_switchToPreviousKey, "Switch To Previous Key");
            GUIUtils.PropFieldGUI(_allowNumbers, "Allow Numbers");
            
            for(int i=0; i < _ammunitionTypes.arraySize; i++)
            {
                SerializedProperty ammunitionType = _ammunitionTypes.GetArrayElementAtIndex(i);
                if (ammunitionType == null)
                    continue;
                AmmunitionTypeGUI(ammunitionType, i);
            }
            EditorGUILayout.Space();   
            CreateAmmunitionType();
            OpenAmmunitionTypeButton();
        }

        private void AmmunitionTypeGUI(SerializedProperty ammunitionType, int index)
        {
            bool foldout = EditorPrefs.GetBool("AmmunitionTypeFoldout" + index, false);
            foldout = EditorGUILayout.Foldout(foldout, "Ammunition Type " + index);            
            EditorPrefs.SetBool("AmmunitionTypeFoldout" + index, foldout);

            if (foldout)
            {
                EditorGUI.indentLevel++;

                AmmunitionType ammoType = (AmmunitionType)ammunitionType.objectReferenceValue;
                SerializedObject ammoTypeSerializedObject = new SerializedObject(ammoType);
                
                SerializedProperty ammunitionProp = ammoTypeSerializedObject.FindProperty("ammunition");
                SerializedProperty ammunitionCountProp = ammoTypeSerializedObject.FindProperty("ammunitionCount");
                SerializedProperty shortCutKeyProp = ammoTypeSerializedObject.FindProperty("shortCutKey");
                
                EditorGUILayout.PropertyField(ammunitionProp, new GUIContent("Ammunition Prefab"));
                EditorGUILayout.PropertyField(ammunitionCountProp, new GUIContent("Ammunition Count"));
                EditorGUILayout.PropertyField(shortCutKeyProp, new GUIContent("Shortcut Key"));
                
                if (GUI.changed || Event.current.commandName == "UndoRedoPerformed")
                {
                    ammoTypeSerializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(ammoType);
                }

                MoveUpAndDownButtons(index);
                DeleteButton(index);

                EditorGUI.indentLevel--;
            }
        }

        private void MoveUpAndDownButtons(int index)
        {
            GUILayout.BeginHorizontal();
            bool isChanged = false;
            if (GUILayout.Button("Move Up") && index > 0)
            {
                _ammunitionTypes.MoveArrayElement(index - 1, index);
                isChanged = true;
            }
            if (GUILayout.Button("Move Down") && index < _ammunitionTypes.arraySize - 1)
            {
                _ammunitionTypes.MoveArrayElement(index + 1, index);
                isChanged = true;
            }
            GUILayout.EndHorizontal();
    
            if (isChanged)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }

        private void DeleteButton(int index)
        {
            if (GUILayout.Button("Delete"))
            {
                _ammunitionTypes.DeleteArrayElementAtIndex(index);
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }

        private void OpenAmmunitionTypeButton()
        {
            if (GUILayout.Button("Import"))
            {
                string assetPath = EditorUtility.OpenFilePanel("Select Ammunition Type", _tmpPath, "asset");
                _tmpPath = assetPath;
                assetPath = FileUtil.GetProjectRelativePath(assetPath);
                if (assetPath.Length != 0)
                {
                    var fileContent = (AmmunitionType) AssetDatabase.LoadAssetAtPath(assetPath, typeof(AmmunitionType));
                    if (fileContent != null)
                    {
                        serializedObject.Update();
                        _ammunitionTypes.arraySize++;
                        _ammunitionTypes.GetArrayElementAtIndex(_ammunitionTypes.arraySize - 1).objectReferenceValue = fileContent;
                        serializedObject.ApplyModifiedProperties();
                    }
                }
            }
        }

        private void CreateAmmunitionType()
        {
            if (GUILayout.Button("Add"))
            {
                // Create a new instance of AmmunitionType
                AmmunitionType newAmmoType = CreateInstance<AmmunitionType>();

                // Open a save file dialog for the developer to choose where to save the new asset
                string assetPath = EditorUtility.SaveFilePanel("Save new Ammunition Type", _tmpPath, "NewAmmunitionType", "asset");
                _tmpPath = assetPath;
        
                // Check if the user actually chose a path (they may have cancelled the dialog)
                if (!string.IsNullOrEmpty(assetPath))
                {
                    // Convert full file path to a relative path (required by AssetDatabase)
                    assetPath = FileUtil.GetProjectRelativePath(assetPath);

                    // Save the new instance as an asset at the chosen path
                    AssetDatabase.CreateAsset(newAmmoType, assetPath);
                    AssetDatabase.SaveAssets();
            
                    // Add the new asset to _ammunitionTypes
                    serializedObject.Update();
                    _ammunitionTypes.arraySize++;
                    _ammunitionTypes.GetArrayElementAtIndex(_ammunitionTypes.arraySize - 1).objectReferenceValue = newAmmoType;
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        public override void BulkUpdateComponents()
        {

        }
    }
}