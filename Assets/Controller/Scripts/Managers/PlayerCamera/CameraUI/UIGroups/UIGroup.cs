using System.Collections.Generic;
using Controller.Scripts.Managers.PlayerCamera.CameraUI.UIElements;
using UnityEngine;

namespace Controller.Scripts.Managers.PlayerCamera.CameraUI.UIGroups
{
    public abstract class UIGroup : MonoBehaviour
    {
        public List<UIElement> uiElements = new ();
        public UIGroupType type;
        
        public virtual void CleanUpUIElements()
        {
            uiElements.RemoveAll(element => element == null);
        }
        
        public virtual void ToggleUIElements(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
        
        public virtual void PerformUpdateAction()
        {
            foreach (UIElement element in uiElements)
            {
                if (element.gameObject.activeInHierarchy)
                {
                    element.PerformUpdateAction();
                }
            }
        }

        public virtual void Initialize()
        {
            type = UIGroupType.Basic;
        }
    }
}