using Controller.Scripts.Managers.Movement;
using UnityEngine;

namespace Controller.Scripts.Managers.PlayerInput.Movement
{
    public abstract class MovementInputManager : MonoBehaviour
    {
        public MovementManager MovementManager { get; set; }

        public float verticalInput;
        public float horizontalInput;

        public abstract void MovementInput();
    }
}