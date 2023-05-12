using UnityEngine;

namespace Controller.Scripts.Managers.PlayerCamera.UIController
{
    public abstract class UIElement : MonoBehaviour
    {
        public abstract void PerformUpdateAction();

        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}