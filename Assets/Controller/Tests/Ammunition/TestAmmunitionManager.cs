using System.Collections;
using System.Collections.Generic;
using Controller.Scripts.Ammunition;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

namespace Controller.Scripts.Ammunition.Tests.AmmunitionTests
{
    public class TestAmmunitionManager : InputTestFixture
    {
        private Keyboard _keyboard;
        private AmmunitionManager _ammunitionManager;
        private InputAction _switchToNextKey;
        private InputAction _switchToPreviousKey;
        private InputAction _fireKey;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _keyboard = InputSystem.AddDevice<Keyboard>();

            _switchToNextKey = new InputAction(binding: "<Keyboard>/a");
            _switchToPreviousKey = new InputAction(binding: "<Keyboard>/b");
            _fireKey = new InputAction(binding: "<Keyboard>/c");

            GameObject gameObject = new GameObject();
            gameObject.SetActive(false);
            _ammunitionManager = gameObject.AddComponent<AmmunitionManager>();
            _ammunitionManager.SwitchToNextAction = _switchToNextKey;
            _ammunitionManager.SwitchToPreviousAction = _switchToPreviousKey;
            _ammunitionManager.FireAction = _fireKey;
            _ammunitionManager.AmmunitionTypes = new List<AmmunitionType>();
            _ammunitionManager.ReloadTime = 1;
            _ammunitionManager.SpawnPoint = new GameObject("SpawnPoint");

            for (int i = 0; i < 3; i++)
            {
                _ammunitionManager.AmmunitionTypes.Add(AddAmmunitionType("AmmunitionType" + i, 10));
            }
            gameObject.SetActive(true);
        }

        private AmmunitionType AddAmmunitionType(string name, int ammunitionCount)
        {
            AmmunitionType ammunitionType = ScriptableObject.CreateInstance<AmmunitionType>();
            ammunitionType.name = name;
            ammunitionType.ammunitionCount = ammunitionCount;
            ammunitionType.projectile = new GameObject("Projectile" + ammunitionCount);
            return ammunitionType;
        }

        [UnityTest]
        public IEnumerator TestLoadAmmunition()
        {
            yield return null;

            Assert.AreEqual(3, _ammunitionManager.AmmunitionTypes.Count);
            foreach (AmmunitionType ammunitionType in _ammunitionManager.AmmunitionTypes)
            {
                Assert.AreEqual(10, ammunitionType.GetAmmoCount());
            }
        }

        [UnityTest]
        public IEnumerator TestSwitchToNext()
        {
            _switchToNextKey.Enable();

            Press(_keyboard.aKey);
            yield return null;
            Release(_keyboard.aKey);
            yield return null;

            Assert.AreEqual(1, _ammunitionManager.GetLoadedAmmoTypeIndex());
            _switchToNextKey.Disable();
        }

        [UnityTest]
        public IEnumerator TestSwitchToPrevious()
        {
            _switchToPreviousKey.Enable();

            Press(_keyboard.bKey);
            yield return null;
            Release(_keyboard.bKey);
            yield return null;

            Assert.AreEqual(2, _ammunitionManager.GetLoadedAmmoTypeIndex());
            _switchToPreviousKey.Disable();
        }

        [UnityTest]
        public IEnumerator TestSwitchToNumber()
        {
            Press(_keyboard.digit1Key);
            yield return null;
            Release(_keyboard.digit1Key);
            yield return null;
            Assert.AreEqual(1, _ammunitionManager.GetLoadedAmmoTypeIndex());

            Press(_keyboard.digit2Key);
            yield return null;
            Release(_keyboard.digit2Key);
            yield return null;
            Assert.AreEqual(2, _ammunitionManager.GetLoadedAmmoTypeIndex());

            Press(_keyboard.digit4Key);
            yield return null;
            Release(_keyboard.digit4Key);
            yield return null;
            Assert.AreEqual(2, _ammunitionManager.GetLoadedAmmoTypeIndex());
        }

        [UnityTest]
        public IEnumerator TestFire()
        {
            _ammunitionManager.FireAction.Enable();

            Press(_keyboard.cKey);
            yield return null;
            // Release(_keyboard.cKey);
            // yield return null;

            Assert.AreEqual(9, _ammunitionManager.GetLoadedAmmoTypeCount());
            Assert.AreEqual(0, _ammunitionManager.GetCurrentReloadTime());

            GameObject projectile = Resources.Load<GameObject>("Projectile10");
            Assert.AreEqual(projectile, _ammunitionManager.GetLoadedAmmoType().projectile);
            Assert.AreEqual(
                _ammunitionManager.SpawnPoint.transform.position,
                projectile.transform.position
            );
            Assert.AreEqual(
                _ammunitionManager.SpawnPoint.transform.rotation,
                projectile.transform.rotation
            );

            _ammunitionManager.FireAction.Disable();
        }
    }
}
