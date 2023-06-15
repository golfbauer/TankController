using UnityEngine;

namespace Controller.Scripts.Managers.Ammunition
{
    [CreateAssetMenu(fileName = "AmmunitionType", menuName = "ScriptableObjects/AmmunitionType", order = 1)]
    public class AmmunitionType : ScriptableObject
    {
        public GameObject ammunition;
        public int ammunitionCount;
        public KeyCode shortCutKey;

        public string AmmunitionName
        {
            get
            {
                try
                {
                    return ammunition.name;
                }
                catch
                {
                    return "Projectile";
                }
            }
        }

        public GameObject FireShot()
        {
            if (ammunitionCount < 1)
            {
                return null;
            }

            ammunitionCount--;
            return ammunition;
        }
    }

}