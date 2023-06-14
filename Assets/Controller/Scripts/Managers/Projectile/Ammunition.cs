using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Controller.Scripts.Managers.Projectile
{
    public class Ammunition : MonoBehaviour
    {
        public List<AmmunitionType.AmmunitionType> ammunitionTypes = new ();
        public GameObject spawnPoint;
        public Vector3 direction;
        public KeyCode fireKey;
        public KeyCode switchToNextKey;
        public KeyCode switchToPreviousKey;
        public bool allowNumbers;
        
        private int _currentAmmunitionTypeIndex;
        private bool _allowShortcut;

        private void Start()
        {
            _allowShortcut = ammunitionTypes.Any(ammunitionType => Input.GetKeyDown(ammunitionType.shortCutKey));
            direction = spawnPoint.transform.forward;
        }

        private void Update()
        {
            SelectAmmunitionType();
            FireShot();
        }

        private void SelectAmmunitionType()
        {
            if (_allowShortcut)
                UseShortcut();
            
            if (allowNumbers)
                Numbers();
            
            SwitchToNext();
            SwitchToPrevious();
        }

        private void UseShortcut()
        {
            foreach(AmmunitionType.AmmunitionType ammunitionType in ammunitionTypes)
            {
                if (Input.GetKeyDown(ammunitionType.shortCutKey))
                {
                    _currentAmmunitionTypeIndex = ammunitionTypes.IndexOf(ammunitionType);
                }
            }
        }

        private void SwitchToNext()
        {
            if (Input.GetKeyDown(switchToNextKey))
            {
                _currentAmmunitionTypeIndex++;
                if (_currentAmmunitionTypeIndex > ammunitionTypes.Count - 1)
                {
                    _currentAmmunitionTypeIndex = 0;
                }
            }
        }
        
        private void SwitchToPrevious()
        {
            if(Input.GetKeyDown(switchToPreviousKey))
            {
                _currentAmmunitionTypeIndex--;
                if (_currentAmmunitionTypeIndex < 0)
                {
                    _currentAmmunitionTypeIndex = ammunitionTypes.Count - 1;
                }
            }
        }
        
        private void Numbers()
        {
            for (int i = 0; i < ProjectileUtils.KeyCodes.Length; i++)
            {
                if (Input.GetKeyDown(ProjectileUtils.KeyCodes[i]))
                {
                    _currentAmmunitionTypeIndex = i - 1;
                }
            }
        }

        private void FireShot()
        {
            if(Input.GetKeyDown(fireKey))
            {
                ShotFired();
            }
        }


        private void ShotFired()
        {
            AmmunitionType.AmmunitionType ammunitionType = ammunitionTypes[_currentAmmunitionTypeIndex];
            direction = spawnPoint.transform.forward;
            
            GameObject ammunition = ammunitionType.FireShot();
            
            if (ammunition == null)
            {
                return;
            }
            
            Quaternion rotation = Quaternion.LookRotation(direction.normalized);
            Instantiate(ammunition, spawnPoint.transform.position, rotation);
        }
    }
}