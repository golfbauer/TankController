using Controller.Scripts.Managers.PlayerCamera.CameraUI.ElementData;
using UnityEngine;

namespace Controller.Scripts.Managers.PlayerCamera.CameraUI.Elements
{
    public abstract class UIElement : MonoBehaviour
    {
        public UIElementData Data;

        public abstract void PerformUpdateAction();
        public abstract void DisplayGUI();

        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        public virtual void InitializeUIElement()
        {
        }
        
        public virtual UIElementType GetUIElementType()
        {
            return Data.Type;
        }
    }
}