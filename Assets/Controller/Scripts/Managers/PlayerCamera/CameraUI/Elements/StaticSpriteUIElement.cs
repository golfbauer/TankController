using Controller.Scripts.Managers.PlayerCamera.CameraUI.ElementData;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Controller.Scripts.Managers.PlayerCamera.CameraUI.Elements
{
    public class StaticSpriteUIElement : UIElement
    {
        public Sprite Sprite
        {
            get => ((UISpriteElementData)Data).Sprite;
            set => ((UISpriteElementData)Data).Sprite = value;
        }

        public override void PerformUpdateAction()
        {
            // Do nothing
        }

        public override void DisplayGUI()
        {
            Sprite newSprite =
                (Sprite)EditorGUILayout.ObjectField("Sprite", Sprite,
                    typeof(Sprite), true);
            if (newSprite != Sprite)
            {
                Sprite = newSprite;
                InitializeUIElement();
            }
        }

        public override void InitializeUIElement()
        {
            Image spriteRenderer = gameObject.GetComponent<Image>();
            if (spriteRenderer == null)
            {
                spriteRenderer = gameObject.AddComponent<Image>();
            }

            spriteRenderer.sprite = Sprite;
        }
    }
}