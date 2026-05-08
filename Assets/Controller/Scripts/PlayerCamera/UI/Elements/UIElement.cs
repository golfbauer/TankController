using Controller.Scripts.PlayerCamera;
using UnityEngine;

namespace Controller.Scripts.PlayerCamera.UI.Elements
{
    public abstract class UIElement : MonoBehaviour
    {
        public UIElementType type;

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
            type = UIElementType.Basic;
        }

        public virtual UIElementType GetUIElementType()
        {
            return type;
        }
    }
}
