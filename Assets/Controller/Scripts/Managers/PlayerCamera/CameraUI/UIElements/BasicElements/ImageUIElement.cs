using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Controller.Scripts.Managers.PlayerCamera.CameraUI.UIElements.BasicElements
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
            Sprite newSprite = (Sprite)EditorGUILayout.ObjectField("Sprite", image.sprite, typeof(Sprite), true);
            if (newSprite != image.sprite)
            {
                image.sprite = newSprite;
            }
        }
    }
}