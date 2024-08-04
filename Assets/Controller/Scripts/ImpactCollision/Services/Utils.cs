namespace Controller.Scripts.ImpactCollision.Services
{
    public static class CollisionUtils
    {
        public const float CollisionTolerance = 0.1f;
    }

    public enum ArmorMaterialType
    {
        LowCarbonSteelPlate,
        NickelSteelPlate,
        GenerallyHomogeneousArmor,
        SurfaceTreatedArmor,
    }
}
