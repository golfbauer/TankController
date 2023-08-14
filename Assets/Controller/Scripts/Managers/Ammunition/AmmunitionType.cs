using UnityEngine;

namespace Controller.Scripts.Managers.Ammunition
{
    [CreateAssetMenu(fileName = "AmmunitionType", menuName = "ScriptableObjects/AmmunitionType", order = 1)]
    public class AmmunitionType : ScriptableObject
    {
        public int ammunitionCount;
        public GameObject projectile;
        public KeyCode shortCutKey;
        
        public delegate void AmmunitionChangeAction(AmmunitionType type, int newCount);
        public event AmmunitionChangeAction OnAmmunitionChanged;
        
        private int _runtimeAmmunitionCount;
        
        public void ResetAmmunitionCount()
        {
            _runtimeAmmunitionCount = ammunitionCount;
        }

        public void DecreaseAmmunitionCount()
        {
            _runtimeAmmunitionCount--;
            OnAmmunitionChanged?.Invoke(this, _runtimeAmmunitionCount);
        }
        
        public void IncreaseAmmunitionCount()
        {
            _runtimeAmmunitionCount++;
            OnAmmunitionChanged?.Invoke(this, _runtimeAmmunitionCount);
        }
        
        public void SetAmmunitionCount(int count)
        {
            _runtimeAmmunitionCount = count;
            OnAmmunitionChanged?.Invoke(this, _runtimeAmmunitionCount);
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

        public override string ToString()
        {
            return name;
        }
    }

}