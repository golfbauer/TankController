using System.Collections.Generic;
using Controller.Scripts.Ammunition.Services;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller.Scripts.Ammunition
{
    public class AmmunitionManager : MonoBehaviour
    {
        public List<AmmunitionType> AmmunitionTypes;
        public GameObject SpawnPoint;
        public float ReloadTime;

        public InputAction FireAction;
        public InputAction SwitchToNextAction;
        public InputAction SwitchToPreviousAction;

        private readonly List<InputAction> _numberKeyActions = new();
        private int _currentAmmunitionTypeIndex;
        private float _currentReloadTime;

        public delegate void ReloadAction(AmmunitionType type, float reloadTime);
        public event ReloadAction OnReload;

        private void Start()
        {
            _currentReloadTime = ReloadTime;

            foreach (AmmunitionType ammunitionType in AmmunitionTypes)
            {
                ammunitionType.ResetAmmunitionCount();
            }
        }

        private void OnEnable()
        {
            SwitchToNextAction.Enable();
            SwitchToPreviousAction.Enable();
            FireAction.Enable();

            SwitchToNextAction.performed += _ => SwitchToNext();
            SwitchToPreviousAction.performed += _ => SwitchToPrevious();
            FireAction.performed += _ => CheckFire();

            for (int i = 0; i < AmmunitionTypes.Count; i++)
            {
                string binding = $"<Keyboard>/{i}";
                InputAction numberKeyAction = new InputAction(binding: binding);
                int index = i;
                numberKeyAction.performed += ctx => SwitchToNumber(index);
                numberKeyAction.Enable();
                _numberKeyActions.Add(numberKeyAction);
            }
        }

        private void OnDisable()
        {
            SwitchToNextAction.Disable();
            SwitchToPreviousAction.Disable();
            FireAction.Disable();

            SwitchToNextAction.performed -= _ => SwitchToNext();
            SwitchToPreviousAction.performed -= _ => SwitchToPrevious();
            FireAction.performed -= _ => CheckFire();

            foreach (var action in _numberKeyActions)
            {
                action.Disable();
                action.Dispose();
            }
        }

        private void Update()
        {
            if (_currentReloadTime < ReloadTime)
            {
                float percentage = _currentReloadTime / ReloadTime * 100;
                Debug.Log("Reloading, " + percentage + "% done.");
                _currentReloadTime += Time.deltaTime;
            }
        }

        private void SwitchToNext()
        {
            _currentAmmunitionTypeIndex++;
            if (_currentAmmunitionTypeIndex > AmmunitionTypes.Count - 1)
            {
                _currentAmmunitionTypeIndex = 0;
            }
            Debug.Log(
                "Switched to "
                    + GetLoadedAmmoTypeName()
                    + ", "
                    + GetLoadedAmmoTypeCount()
                    + " shots left!"
            );
            Reload();
        }

        private void SwitchToPrevious()
        {
            _currentAmmunitionTypeIndex--;
            if (_currentAmmunitionTypeIndex < 0)
            {
                _currentAmmunitionTypeIndex = AmmunitionTypes.Count - 1;
            }
            Debug.Log(
                "Switched to "
                    + GetLoadedAmmoTypeName()
                    + ", "
                    + GetLoadedAmmoTypeCount()
                    + " shots left!"
            );
            Reload();
        }

        private void SwitchToNumber(int index)
        {
            if (index < 0 || index > AmmunitionTypes.Count - 1)
            {
                Debug.LogWarning($"Invalid ammunition index: {index}");
                return;
            }

            _currentAmmunitionTypeIndex = index;
            Debug.Log(
                "Switched to "
                    + GetLoadedAmmoTypeName()
                    + ", "
                    + GetLoadedAmmoTypeCount()
                    + " shots left!"
            );
            Reload();
        }

        private void Reload()
        {
            _currentReloadTime = 0;
            OnReload?.Invoke(AmmunitionTypes[_currentAmmunitionTypeIndex], ReloadTime);
        }

        private void CheckFire()
        {
            if (_currentReloadTime < ReloadTime)
            {
                Debug.Log("Tried to fire, but still reloading. Please wait.");
                return;
            }

            AmmunitionType ammunitionType = GetLoadedAmmoType();
            GameObject projectile = ammunitionType.GetProjectile();

            if (projectile == null)
            {
                Debug.LogWarning("Out of ammunition.");
                return;
            }

            Fire(projectile, ammunitionType);
            Reload();
        }

        private void Fire(GameObject projectile, AmmunitionType ammunitionType)
        {
            Vector3 fireDirection = SpawnPoint.transform.forward;

            Quaternion rotation = Quaternion.LookRotation(fireDirection.normalized);
            Instantiate(projectile, SpawnPoint.transform.position, rotation);
            Debug.LogWarning("Fired " + ammunitionType.name + "!");
        }

        public AmmunitionType GetLoadedAmmoType()
        {
            return AmmunitionTypes[_currentAmmunitionTypeIndex];
        }

        public int GetLoadedAmmoTypeCount()
        {
            return AmmunitionTypes[_currentAmmunitionTypeIndex].GetAmmoCount();
        }

        private string GetLoadedAmmoTypeName()
        {
            return AmmunitionTypes[_currentAmmunitionTypeIndex].name;
        }

        public int GetLoadedAmmoTypeIndex()
        {
            return _currentAmmunitionTypeIndex;
        }

        public float GetCurrentReloadTime()
        {
            return _currentReloadTime;
        }
    }
}
