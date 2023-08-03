using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Controller.Scripts.Managers.Ammunition
{
    public class AmmunitionManager : MonoBehaviour
    {
        public List<AmmunitionType> ammunitionTypes = new();
        public GameObject spawnPoint;
        public Vector3 direction;
        public KeyCode fireKey;
        public KeyCode switchToNextKey;
        public KeyCode switchToPreviousKey;
        public bool allowNumbers;
        public float reloadTime;

        private int _currentAmmunitionTypeIndex;
        private bool _allowShortcuts;
        private float _currentReloadTime;

        private void Start()
        {
            _currentReloadTime = reloadTime;
            _allowShortcuts = ammunitionTypes.Any(ammunitionType =>
                Input.GetKeyDown(ammunitionType.shortCutKey));
            direction = spawnPoint.transform.forward;

            foreach (AmmunitionType ammunitionType in ammunitionTypes)
            {
                ammunitionType.ResetAmmunitionCount();
            }
        }

        private void Update()
        {
            SelectAmmunitionType();
            if (_currentReloadTime < reloadTime)
            {
                _currentReloadTime += Time.deltaTime;
                return;
            }

            CheckFire();
        }

        private void SelectAmmunitionType()
        {
            if (_allowShortcuts)
                UseShortcuts();

            if (allowNumbers)
                UseNumbers();

            SwitchToNext();
            SwitchToPrevious();
        }

        private void Reload()
        {
            _currentReloadTime = 0;
        }

        private void UseShortcuts()
        {
            foreach (AmmunitionType ammunitionType in ammunitionTypes)
            {
                if (Input.GetKeyDown(ammunitionType.shortCutKey))
                {
                    _currentAmmunitionTypeIndex =
                        ammunitionTypes.IndexOf(ammunitionType);
                    Reload();
                }
            }
        }

        private void SwitchToNext()
        {
            if (Input.GetKeyDown(switchToNextKey))
            {
                Debug.LogWarning("Switch to next");
                _currentAmmunitionTypeIndex++;
                Reload();
                if (_currentAmmunitionTypeIndex > ammunitionTypes.Count - 1)
                {
                    _currentAmmunitionTypeIndex = 0;
                }
            }
        }

        private void SwitchToPrevious()
        {
            if (Input.GetKeyDown(switchToPreviousKey))
            {
                Debug.LogWarning("Switch to previous");
                _currentAmmunitionTypeIndex--;
                Reload();
                if (_currentAmmunitionTypeIndex < 0)
                {
                    _currentAmmunitionTypeIndex = ammunitionTypes.Count - 1;
                }
            }
        }

        private void UseNumbers()
        {
            for (int i = 0; i < ProjectileUtils.KeyCodes.Length; i++)
            {
                if (Input.GetKeyDown(ProjectileUtils.KeyCodes[i]) &&
                    i < ammunitionTypes.Count)
                {
                    _currentAmmunitionTypeIndex = i;
                    Reload();
                }
            }
        }

        private void CheckFire()
        {
            if (Input.GetKeyDown(fireKey))
            {
                Fire();
            }
        }

        private void Fire()
        {
            AmmunitionType ammunitionType =
                ammunitionTypes[_currentAmmunitionTypeIndex];
            direction = spawnPoint.transform.forward;

            GameObject ammunition = ammunitionType.FireShot();

            if (ammunition == null)
            {
                Debug.LogWarning("Out of ammunition");
                return;
            }

            Quaternion rotation =
                Quaternion.LookRotation(direction.normalized);
            Instantiate(ammunition, spawnPoint.transform.position, rotation);
            Debug.LogWarning("Fired " + ammunitionType.name);
            Reload();
        }

        public int GetAmmoCount(int index)
        {
            return ammunitionTypes[index].GetAmmoCount();
        }

        public float GetReloadPercent(int index)
        {
            if (index != _currentAmmunitionTypeIndex)
                return 1;

            return _currentReloadTime / reloadTime;
        }
    }
}