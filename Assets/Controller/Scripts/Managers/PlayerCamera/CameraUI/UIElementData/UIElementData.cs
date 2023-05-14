using UnityEngine;

namespace Controller.Scripts.Managers.PlayerCamera.CameraUI.UIElementData
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/UIElementData", order = 1)]
    public class UIElementData : ScriptableObject
    {
        public UIElementType Type;
    }
}