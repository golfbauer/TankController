using Controller.Scripts.Editors.Wheels;

namespace Controller.Scripts.Editors.Tank
{
    public static class CreateTankMessages
    {
        public const string Hull = "Hull";
        public const string Rigidbody = "Rigidbody";
        public const string Collider = "Collider";

        public const string Component = "Component";
        public const string Components = "Components";
        public const string Camera = "Camera";
        public const string CameraType = "Camera Type";
        
        public const string Mass = "Mass";
        public const string PhysicsIterations = "Physics Iterations";
        public const string CenterOfMass = "Center Of Mass";
        
        public const string Mesh = "Mesh";
        public const string Material = "Material";
        
        public const string UseBoxCollider = "Use Box Collider";
        public const string MeshCollider = "Mesh Collider";
        public const string ColliderCenter = "Collider Center";
        public const string ColliderSize = "Collider Size";
        
        public const string Transform = "Transform";
        
        public const string Create = "Create";
    }

    public enum ComponentType
    {
        SupportWheel,
        DriveWheel,
        SuspensionWheel,
        RearWheel,
        Chain,
        Turret,
        Camera,
    }
}