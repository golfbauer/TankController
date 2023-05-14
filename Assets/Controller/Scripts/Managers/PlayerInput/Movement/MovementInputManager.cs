using Controller.Scripts.Managers.Movement;
using UnityEngine;

namespace Controller.Scripts.Managers.PlayerInput.Movement
{
    public abstract class MovementInputManager : MonoBehaviour
    {
        public MovementManager MovementManager { get; set; }
        
        protected float VerticalInput;
        protected float HorizontalInput;

        public abstract void MovementInput();
        public abstract void PassDownMovementInput();
    }
}