using UnityEngine;

namespace Controller.Scripts.Managers.PlayerInput
{
    public abstract class InputUtils
    {
        public static readonly KeyCode Forward = KeyCode.W;
        public static readonly KeyCode Backward = KeyCode.S;
        public static readonly KeyCode Left = KeyCode.A;
        public static readonly KeyCode Right = KeyCode.D;
        
        public static readonly KeyCode Break = KeyCode.Space;
    }
    
    public enum MovementInputType
    {
        Keyboard,
        Joystick
    }
}