using System;
using System.Collections.Generic;
using Controller.Scripts.Ammunition;
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

        private Action<InputAction.CallbackContext> _onSwitchNext;
        private Action<InputAction.CallbackContext> _onSwitchPrev;
        private Action<InputAction.CallbackContext> _onFire;

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

            _onSwitchNext = _ => SwitchToNext();
            _onSwitchPrev = _ => SwitchToPrevious();
            _onFire = _ => CheckFire();

            SwitchToNextAction.performed += _onSwitchNext;
            SwitchToPreviousAction.performed += _onSwitchPrev;
            FireAction.performed += _onFire;

            // TODO(refactor 3.4.a): Replace this per-slot InputAction rebuild with a single
            // InputActionMap, or one action bound to `<Keyboard>/#(0-9)` that reads the digit
            // from the callback context. Currently allocates N actions on every OnEnable and
            // disposes them on OnDisable.
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

            SwitchToNextAction.performed -= _onSwitchNext;
            SwitchToPreviousAction.performed -= _onSwitchPrev;
            FireAction.performed -= _onFire;

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
            Reload();
        }

        private void SwitchToPrevious()
        {
            _currentAmmunitionTypeIndex--;
            if (_currentAmmunitionTypeIndex < 0)
            {
                _currentAmmunitionTypeIndex = AmmunitionTypes.Count - 1;
            }
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
