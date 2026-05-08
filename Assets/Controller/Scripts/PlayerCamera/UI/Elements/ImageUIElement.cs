using Controller.Scripts.PlayerCamera;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace Controller.Scripts.PlayerCamera.UI.Elements
{
    public class ImageUIElement : UIElement
    {
        public Image image;

        public override void PerformUpdateAction()
        {
            // Do nothing
        }

        public override void InitializeUIElement()
        {
            type = UIElementType.Image;
            image = gameObject.GetComponent<Image>();
            if (image == null)
            {
                image = gameObject.AddComponent<Image>();
            }

            image.type = Image.Type.Filled;
        }

        public override void DisplayGUI()
        {
#if UNITY_EDITOR
            Sprite newSprite = (Sprite)
                EditorGUILayout.ObjectField("Sprite", image.sprite, typeof(Sprite), true);
            if (newSprite != image.sprite)
            {
                image.sprite = newSprite;
            }
#endif
        }
    }
}
