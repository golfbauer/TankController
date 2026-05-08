using Controller.Scripts.Ammunition;
using Controller.Scripts.PlayerCamera.UI.Elements;
using Controller.Scripts.PlayerCamera.UI.Groups;
using Controller.Scripts.PlayerCamera;

namespace Controller.Scripts.PlayerCamera.UI.Elements
{
    public class AmmoTextUIElement : TextUIElement
    {
        public AmmoUIGroup ammoUIGroup;

        public void Awake()
        {
            ammoUIGroup = GetComponentInParent<AmmoUIGroup>();
            ammoUIGroup.ammunitionType.OnAmmunitionChanged += UpdateAmmoCount;
        }

        private void UpdateAmmoCount(AmmunitionType type, int newCount)
        {
            string ammoCount = newCount + "/" + ammoUIGroup.ammunitionType.ammunitionCount;
            text.text = ammoCount;
        }

        public override void PerformUpdateAction()
        {
            // Do nothing
        }

        public override void InitializeUIElement()
        {
            base.InitializeUIElement();
            type = UIElementType.AmmoText;
            ammoUIGroup = GetComponentInParent<AmmoUIGroup>();
        }

        public override void DisplayGUI()
        {
            base.DisplayGUI();
            text.text =
                ammoUIGroup.ammunitionType.ammunitionCount
                + "/"
                + ammoUIGroup.ammunitionType.ammunitionCount;
        }
    }
}
