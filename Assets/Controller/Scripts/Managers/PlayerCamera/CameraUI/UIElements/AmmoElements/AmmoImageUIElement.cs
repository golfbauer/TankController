using System.Collections;
using System.Collections.Generic;
using Controller.Scripts.Managers.Ammunition;
using Controller.Scripts.Managers.PlayerCamera.CameraUI.UIElements.BasicElements;
using Controller.Scripts.Managers.PlayerCamera.CameraUI.UIGroups.BasicUIGroup;
using UnityEditor;
using UnityEngine;

namespace Controller.Scripts.Managers.PlayerCamera.CameraUI.UIElements.AmmoElements
{
    public class AmmoImageUIElement : ImageUIElement
    {
        public AmmoUIGroup ammoUIGroup;
        
        public void Awake()
        {
            ammoUIGroup = GetComponentInParent<AmmoUIGroup>();
            ammoUIGroup.ammunitionManager.OnReload += TriggerReload;
        }
        
        private void TriggerReload(AmmunitionType type, float reloadTime)
        {
            if (type == ammoUIGroup.ammunitionType)
            {
                StartCoroutine(Reload(reloadTime));
            }
        }
        
        private IEnumerator Reload(float reloadTime)
        {
            float time = 0;
            while (time < reloadTime)
            {
                time += Time.deltaTime;
                float fillAmount = time / reloadTime;
                image.fillAmount = fillAmount;
                Debug.Log("Fill Amount: " + fillAmount);
                yield return null;
            }
        }
        
        public override void PerformUpdateAction()
        {
            // Do nothing
        }

        public override void InitializeUIElement()
        {
            base.InitializeUIElement();
            type = UIElementType.AmmoImage;
            ammoUIGroup = GetComponentInParent<AmmoUIGroup>();

            if (ammoUIGroup == null)
            {
                Debug.LogError("Ammo Group could not be assigned! Please select Ammo Group when assigning this UI Element.");
            }
        }
    }
}