using Controller.Scripts.Ammunition;
using Controller.Scripts.PlayerCamera;
using UnityEngine.PlayerLoop;

namespace Controller.Scripts.PlayerCamera.UI.Groups
{
    public class AmmoUIGroup : UIGroup
    {
        public AmmunitionManager ammunitionManager;
        public AmmunitionType ammunitionType;

        public override void Initialize()
        {
            type = UIGroupType.Ammo;
        }
    }
}
