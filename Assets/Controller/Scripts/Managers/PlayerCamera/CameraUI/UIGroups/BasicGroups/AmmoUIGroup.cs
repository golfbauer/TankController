using Controller.Scripts.Managers.Ammunition;
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