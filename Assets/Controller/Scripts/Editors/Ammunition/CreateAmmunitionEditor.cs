using Controller.Scripts.Editors.Utils;
using Controller.Scripts.Managers.Ammunition;
using UnityEditor;
using UnityEngine;

namespace Controller.Scripts.Editors.Ammunition
{
    [CustomEditor(typeof(AmmunitionManager))]
    public class CreateAmmunitionEditor : TankComponentEditor
    {
        private SerializedProperty _ammunitionTypes;
        private SerializedProperty _spawnPoint;
        private SerializedProperty _fireKey;
        private SerializedProperty _switchToNextKey;
        private SerializedProperty _switchToPreviousKey;
        private SerializedProperty _allowNumbers;
        private SerializedProperty _reloadTime;

        private string _tmpPath = "/Assets";

        private void OnEnable()
        {
            _ammunitionTypes =
                serializedObject.FindProperty("ammunitionTypes");
            _spawnPoint = serializedObject.FindProperty("spawnPoint");
            _fireKey = serializedObject.FindProperty("fireKey");
            _switchToNextKey =
                serializedObject.FindProperty("switchToNextKey");
            _switchToPreviousKey =
                serializedObject.FindProperty("switchToPreviousKey");
            _allowNumbers = serializedObject.FindProperty("allowNumbers");
            _reloadTime = serializedObject.FindProperty("reloadTime");

            transform = ((AmmunitionManager)target).gameObject.transform;
        }

        public override void SetUpGUI()
        {
            GUIUtils.PropFieldGUI(_spawnPoint, AmmunitionMessages.SpawnPoint);
            GUIUtils.PropFieldGUI(_fireKey, AmmunitionMessages.FireKey);
            GUIUtils.PropFieldGUI(_switchToNextKey,
                AmmunitionMessages.SwitchToNextKey);
            GUIUtils.PropFieldGUI(_switchToPreviousKey,
                AmmunitionMessages.SwitchToPreviousKey);
            GUIUtils.PropFieldGUI(_allowNumbers,
                AmmunitionMessages.AllowNumbers);
            GUIUtils.PropFieldGUI(_reloadTime, AmmunitionMessages.ReloadTime);

            EditorGUILayout.Space();

            for (int i = 0; i < _ammunitionTypes.arraySize; i++)
            {
                SerializedProperty ammunitionType =
                    _ammunitionTypes.GetArrayElementAtIndex(i);
                if (ammunitionType == null)
                    continue;
                AmmunitionTypeGUI(ammunitionType, i);
                EditorGUILayout.Space();
            }

            EditorGUILayout.Space();

            Create();
            Open();
        }

        private void AmmunitionTypeGUI(SerializedProperty ammunitionType,
            int index)
        {
            bool foldout =
                EditorPrefs.GetBool("AmmunitionTypeFoldout" + index, false);
            string name = ammunitionType.objectReferenceValue == null
                ? "Empty"
                : ammunitionType.objectReferenceValue.name;
            foldout = EditorGUILayout.Foldout(foldout, name);
            EditorPrefs.SetBool("AmmunitionTypeFoldout" + index, foldout);

            if (foldout)
            {
                EditorGUI.indentLevel++;

                AmmunitionType ammoType =
                    (AmmunitionType)ammunitionType.objectReferenceValue;
                SerializedObject ammoTypeSerializedObject =
                    new SerializedObject(ammoType);

                SerializedProperty ammunitionProp =
                    ammoTypeSerializedObject.FindProperty("projectile");
                SerializedProperty ammunitionCountProp =
                    ammoTypeSerializedObject.FindProperty("ammunitionCount");
                SerializedProperty shortCutKeyProp =
                    ammoTypeSerializedObject.FindProperty("shortCutKey");

                GUIUtils.PropFieldGUI(ammunitionProp,
                    AmmunitionMessages.AmmunitionPrefab);
                GUIUtils.PropFieldGUI(ammunitionCountProp,
                    AmmunitionMessages.AmmunitionCount);
                GUIUtils.PropFieldGUI(shortCutKeyProp,
                    AmmunitionMessages.ShortcutKey);

                if (GUI.changed ||
                    Event.current.commandName == "UndoRedoPerformed")
                {
                    ammoTypeSerializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(ammoType);
                }

                MoveUpAndDown(index);
                Delete(index);

                EditorGUI.indentLevel--;
            }
        }

        private void MoveUpAndDown(int index)
        {
            GUILayout.BeginHorizontal();
            bool isChanged = false;
            if (GUILayout.Button(GeneralMessages.MoveUp) && index > 0)
            {
                _ammunitionTypes.MoveArrayElement(index - 1, index);
                isChanged = true;
            }

            if (GUILayout.Button(GeneralMessages.MoveDown) &&
                index < _ammunitionTypes.arraySize - 1)
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

        private void Delete(int index)
        {
            if (GUILayout.Button(GeneralMessages.Remove))
            {
                _ammunitionTypes.DeleteArrayElementAtIndex(index);
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }

        private void Open()
        {
            if (GUILayout.Button(GeneralMessages.Import))
            {
                string assetPath = EditorUtility.OpenFilePanel(
                    AmmunitionMessages.OpenAmmunitionType, _tmpPath, "asset");
                _tmpPath = assetPath;
                assetPath = FileUtil.GetProjectRelativePath(assetPath);
                if (assetPath.Length != 0)
                {
                    var fileContent =
                        (AmmunitionType)AssetDatabase.LoadAssetAtPath(
                            assetPath, typeof(AmmunitionType));
                    if (fileContent != null)
                    {
                        serializedObject.Update();
                        _ammunitionTypes.arraySize++;
                        _ammunitionTypes
                            .GetArrayElementAtIndex(
                                _ammunitionTypes.arraySize - 1)
                            .objectReferenceValue = fileContent;
                        serializedObject.ApplyModifiedProperties();
                    }
                }
            }
        }

        private void Create()
        {
            if (GUILayout.Button(GeneralMessages.Add))
            {
                // Create a new instance of AmmunitionType
                AmmunitionType newAmmoType = CreateInstance<AmmunitionType>();

                // Open a save file dialog for the developer to choose where to save the new asset
                string assetPath = EditorUtility.SaveFilePanel(
                    AmmunitionMessages.SaveNewAmmunitionType,
                    _tmpPath, AmmunitionMessages.NewAmmunitionTypeName,
                    "asset");
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
                    _ammunitionTypes
                        .GetArrayElementAtIndex(
                            _ammunitionTypes.arraySize - 1)
                        .objectReferenceValue = newAmmoType;
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
    }
}