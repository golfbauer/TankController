using UnityEngine;

namespace Controller.Scripts.Managers.PlayerCamera.CameraUI.ElementData
{
    [CreateAssetMenu(fileName = "SpriteData",
        menuName = "ScriptableObjects/UISpriteElementData", order = 2)]
    public class UISpriteElementData : UIElementData
    {
        public Sprite Sprite;
    }
}