using UnityEngine;

namespace Controller.Scripts.ImpactCollision
{
    public static class CollisionUtils
    {
        public const float CollisionTolerance = 0.1f;
        public const float DefaultThickness = 90f;
    }

    public enum ArmorMaterialType
    {
        LowCarbonSteelPlate,
        NickelSteelPlate,
        GenerallyHomogeneousArmor,
        SurfaceTreatedArmor,
    }
}
