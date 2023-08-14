using UnityEngine;

namespace Controller.Scripts.Managers.PlayerCamera
{
    public enum CameraType
    {
        ThirdPerson,
        Scoped
    }
    
    public enum UIElementType
    {
        Basic,
        Text,
        Image,
        AmmoText,
        AmmoImage,
    }

    public enum UIGroupType
    {
        Basic,
        Ammo,
        Group,
    }
}