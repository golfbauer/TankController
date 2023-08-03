using UnityEngine;

namespace Controller.Scripts.Managers.Ammunition
{
    [CreateAssetMenu(fileName = "AmmunitionType",
        menuName = "ScriptableObjects/AmmunitionType", order = 1)]
    public class AmmunitionType : ScriptableObject
    {
        public string AmmunitionName
        {
            get
            {
                try
                {
                    return projectile.name;
                }
                catch
                {
                    return "Projectile";
                }
            }
        }

        public int ammunitionCount;
        public GameObject projectile;
        public KeyCode shortCutKey;

        private int _runtimeAmmunitionCount;

        public void ResetAmmunitionCount()
        {
            _runtimeAmmunitionCount = ammunitionCount;
        }

        public void DecreaseAmmunitionCount()
        {
            _runtimeAmmunitionCount--;
        }

        public GameObject FireShot()
        {
            if (_runtimeAmmunitionCount < 1)
            {
                return null;
            }

            DecreaseAmmunitionCount();
            return projectile;
        }

        public int GetAmmoCount()
        {
            return _runtimeAmmunitionCount;
        }
    }
}