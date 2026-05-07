using Controller.Scripts.Ammunition;
using Controller.Scripts.Ammunition.Services;
using Controller.Scripts.PlayerCamera.Services;
using UnityEngine.PlayerLoop;

namespace Controller.Scripts.Managers.PlayerCamera.CameraUI.UIGroups.BasicUIGroup
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
