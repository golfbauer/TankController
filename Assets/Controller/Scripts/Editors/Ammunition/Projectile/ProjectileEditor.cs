using System.Reflection;
using Controller.Scripts.Editors.Utils;
using Controller.Scripts.Managers.Ammunition.Projectile;
using UnityEditor;
using UnityEngine;

namespace Controller.Scripts.Editors.Ammunition.Projectile
{
    [CustomEditor(typeof(BaseProjectile), true)]
    public class ProjectileEditor : TankComponentEditor
    {
        private SerializedProperty _caliber;
        private SerializedProperty _mass;
        private SerializedProperty _initVelocity;
        private SerializedProperty _maxTravelDistance;
        private SerializedProperty _maxLifetime;

        private void OnEnable()
        {
            _caliber = serializedObject.FindProperty("caliber");
            _mass = serializedObject.FindProperty("mass");
            _initVelocity = serializedObject.FindProperty("initVelocity");
            _maxTravelDistance = serializedObject.FindProperty("maxTravelDistance");
            _maxLifetime = serializedObject.FindProperty("maxLifetime");

            transform = ((BaseProjectile)target).gameObject.transform;
        }

        public override void SetUpGUI()
        {
            GUIUtils.PropFieldGUI(_caliber, "Caliber");
            GUIUtils.PropFieldGUI(_mass, "Mass");
            GUIUtils.PropFieldGUI(_initVelocity, "Initial Velocity");
            GUIUtils.PropFieldGUI(_maxTravelDistance, "Max Travel Distance");
            GUIUtils.PropFieldGUI(_maxLifetime, "Max Lifetime");

            BaseProjectile projectile = (BaseProjectile)target;
            MethodInfo editorMethod = projectile.GetType().GetMethod("EditorSetUp", BindingFlags.Public | BindingFlags.Instance);
            
            if (editorMethod != null)
            {
                EditorGUILayout.LabelField("Projectile Type Settings", EditorStyles.boldLabel);
                editorMethod.Invoke(projectile, null);
            }
        }
    }
}