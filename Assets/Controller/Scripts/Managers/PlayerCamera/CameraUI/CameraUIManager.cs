using System;
using System.Collections.Generic;
using Controller.Scripts.Managers.PlayerCamera.CameraUI.ElementData;
using Controller.Scripts.Managers.PlayerCamera.CameraUI.Elements;
using UnityEngine;

namespace Controller.Scripts.Managers.PlayerCamera.CameraUI
{
    [Serializable]
    public class CameraUIManager : MonoBehaviour
    {
        [SerializeField]
        public List<UIElement> uiElements = new List<UIElement>();

        // I doubt that I need this, but this took me an entire day to get this to work, so I'm keeping it
        public List<UIElementData> uiElementsData = new List<UIElementData>();

        [SerializeField] public GameObject canvas;
        public bool isActive;

        private void Awake()
        {
            UpdateUIElements();
        }

        public void ActivateUIElement(int index)
        {
            if (index < uiElements.Count)
            {
                uiElements[index].Activate();
            }
        }

        public void DeactivateUIElement(int index)
        {
            if (index < uiElements.Count)
            {
                uiElements[index].Deactivate();
            }
        }

        public void UpdateUIElement(UIElement uiElement)
        {
            if (isActive)
                uiElement.Activate();
            else
                uiElement.Deactivate();
        }

        public void ToggleUIElements(bool setActive)
        {
            isActive = setActive;
            UpdateUIElements();
        }

        public void UpdateUIElements()
        {
            uiElements.RemoveAll(element => element == null);
            uiElementsData.RemoveAll(element => element == null);

            foreach (var element in uiElements)
            {
                UpdateUIElement(element);
            }

            for (int i = uiElementsData.Count - 1; i >= 0; i--)
            {
                bool isOrphan = true;
                foreach (var element in uiElements)
                {
                    if (element.Data == uiElementsData[i])
                    {
                        isOrphan = false;
                        break;
                    }
                }

                if (isOrphan)
                {
                    DestroyImmediate(uiElementsData[i]);
                    uiElementsData.RemoveAt(i);
                }
            }
        }

        void Update()
        {
            foreach (UIElement element in uiElements)
            {
                if (element.gameObject.activeInHierarchy)
                {
                    element.PerformUpdateAction();
                }
            }
        }
    }
}