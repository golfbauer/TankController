using TMPro;

namespace Controller.Scripts.Managers.PlayerCamera.CameraUI.UIElements.BasicElements
{
    public class TextUIElement : UIElement
    {
        public TextMeshProUGUI text;
        public override void PerformUpdateAction()
        {
            // Do nothing
        }
        
        public override void InitializeUIElement()
        {
            type = UIElementType.Text;
            text = gameObject.GetComponent<TextMeshProUGUI>();
            if (text == null)
            {
                text = gameObject.AddComponent<TextMeshProUGUI>();
            }
        }

        public override void DisplayGUI()
        {
            text.text = UnityEditor.EditorGUILayout.TextField("Text", text.text);
        }
    }
}