using UnityEngine;

namespace Controller.Scripts.Utils
{
    public static class LayerUtils
    {
        public const string WheelLayer = "TankWheel";
        public const string HullLayer = "TankHull";

        public static void SetLayer(GameObject gameObject, string layerName)
        {
            gameObject.layer = LayerMask.NameToLayer(layerName);
        }
    }
}
