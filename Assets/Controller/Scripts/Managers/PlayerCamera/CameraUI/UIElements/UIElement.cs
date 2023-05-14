using UnityEngine;

namespace Controller.Scripts.Managers.PlayerCamera.CameraUI.UIElements
{
    public abstract class UIElement : MonoBehaviour
    {
        public UIElementData.UIElementData Data;

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