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
        private SerializedProperty _diameter;
        private SerializedProperty _mass;
        private SerializedProperty _initVelocity;
        private SerializedProperty _maxTravelDistance;
        private SerializedProperty _maxLifetime;

        private void OnEnable()
        {
            _diameter = serializedObject.FindProperty("diameter");
            _mass = serializedObject.FindProperty("mass");
            _initVelocity = serializedObject.FindProperty("initVelocity");
            _maxTravelDistance = serializedObject.FindProperty("maxTravelDistance");
            _maxLifetime = serializedObject.FindProperty("maxLifetime");

            transform = ((BaseProjectile)target).gameObject.transform;
        }

        public override void SetUpGUI()
        {
            GUIUtils.PropFieldGUI(_diameter, ProjectileMessages.Diameter);
            GUIUtils.PropFieldGUI(_mass, ProjectileMessages.Mass);
            GUIUtils.PropFieldGUI(_initVelocity, ProjectileMessages.InitialVelocity);
            GUIUtils.PropFieldGUI(_maxTravelDistance, ProjectileMessages.MaxTravelDistance);
            GUIUtils.PropFieldGUI(_maxLifetime, ProjectileMessages.MaxLifetime);

            BaseProjectile projectile = (BaseProjectile)target;
            MethodInfo editorMethod = projectile.GetType().GetMethod("EditorSetUp", BindingFlags.Public | BindingFlags.Instance);
            
            if (editorMethod != null)
            {
                EditorGUILayout.LabelField(ProjectileMessages.ProjectileTypeSettings, EditorStyles.boldLabel);
                editorMethod.Invoke(projectile, null);
            }
        }
    }
}