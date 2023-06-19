namespace Controller.Scripts.Managers.ImpactCollision
{
    public static class CollisionUtils
    {
        public const float CollisionTolerance = 0.1f;
    }

    public enum ArmorMaterialType
    {
        LowCarbonSteelPlate = 1530,
        NickelSteelPlate = 1900,
        GenerallyHomogeneousArmor = 2000-2400,
        SurfaceTreatedArmor = 2400-2600,
    }
}