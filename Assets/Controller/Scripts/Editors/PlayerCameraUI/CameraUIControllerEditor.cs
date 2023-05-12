using System;
using Controller.Scripts.Managers.PlayerCamera.UIController;
using UnityEditor;

namespace Controller.Scripts.Editors.PlayerCameraUI
{
    [CustomEditor(typeof(CameraUIController))]
    [CanEditMultipleObjects]
    public class CameraUIControllerEditor : GeneralEditor
    {
    }
}