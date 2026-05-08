using Controller.Scripts.PlayerCamera;
using TMPro;

namespace Controller.Scripts.PlayerCamera.UI.Elements
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
#if UNITY_EDITOR
            text.text = UnityEditor.EditorGUILayout.TextField("Text", text.text);
#endif
        }
    }
}
