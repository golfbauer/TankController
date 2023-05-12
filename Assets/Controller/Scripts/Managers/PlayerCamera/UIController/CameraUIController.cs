using System.Collections.Generic;
using UnityEngine;

namespace Controller.Scripts.Managers.PlayerCamera.UIController
{
    public class CameraUIController : MonoBehaviour
    {
        public List<UIElement> uiElements;

        public void ActivateUIElements(int index)
        {
            if(index < uiElements.Count)
            {
                uiElements[index].Activate();
            }
        }

        public void DeactivateUIElements(int index)
        {
            if(index < uiElements.Count)
            {
                uiElements[index].Deactivate();
            }
        }

        void Update()
        {
            foreach(UIElement element in uiElements)
            {
                if(element.gameObject.activeInHierarchy)
                {
                    element.PerformUpdateAction();
                }
            }
        }
    }
}