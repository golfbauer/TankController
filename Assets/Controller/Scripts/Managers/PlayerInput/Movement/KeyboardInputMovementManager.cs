using UnityEngine;

namespace Controller.Scripts.Managers.PlayerInput.Movement
{
    public class KeyboardInputMovementManager: MovementInputManager
    {
        public override void MovementInput()
        {
            VerticalInput = 0;
            HorizontalInput = 0;
            
            if (UnityEngine.Input.GetKey(InputUtils.Forward))
            {
                VerticalInput = 1;
            }
            
            if (UnityEngine.Input.GetKey(InputUtils.Backward))
            {
                VerticalInput = -1;
            }
            
            if (UnityEngine.Input.GetKey(InputUtils.Left))
            {
                HorizontalInput = -1;
            }
            
            if (UnityEngine.Input.GetKey(InputUtils.Right))
            {
                HorizontalInput = 1;
            }
            
            MovementManager.breakTank = UnityEngine.Input.GetKey(InputUtils.Break);
            PassDownMovementInput();
        }

        public override void PassDownMovementInput()
        {
            MovementManager.verticalInput = VerticalInput;
            MovementManager.horizontalInput = HorizontalInput;
        }
    }
}