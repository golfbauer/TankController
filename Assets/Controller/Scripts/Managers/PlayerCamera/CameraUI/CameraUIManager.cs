using System;
using System.Collections.Generic;
using Controller.Scripts.Managers.PlayerCamera.CameraUI.UIGroups;
using UnityEngine;

namespace Controller.Scripts.Managers.PlayerCamera.CameraUI
{
    [Serializable]
    public class CameraUIManager : MonoBehaviour
    {
        [SerializeField] public List<UIGroup> uiGroups = new ();
        [SerializeField] public GameObject canvas;
        public bool isActive;

        private void Awake()
        {
            CleanUp();
            ToggleUI(false);
        }
        
        public void CleanUp()
        {
            uiGroups.RemoveAll(element => element == null);
            foreach(var uiGroup in uiGroups)
                uiGroup.CleanUpUIElements();
        }
        
        public void ToggleUI(bool setActive)
        {
            isActive = setActive;
            foreach(var uiGroup in uiGroups)
                uiGroup.ToggleUIElements(setActive);
        }

        void Update()
        {
            foreach (UIGroup uiGroup in uiGroups)
            {
                if(uiGroup.gameObject.activeInHierarchy)
                    uiGroup.PerformUpdateAction();
            }
        }
    }
}