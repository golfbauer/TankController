using UnityEngine;

namespace Controller.Scripts.Movement.PlayerInput
{
    public abstract class MovementInputManager : MonoBehaviour
    {
        public MovementManager MovementManager { get; set; }

        public float verticalInput;
        public float horizontalInput;

        public abstract void MovementInput();
    }
}
