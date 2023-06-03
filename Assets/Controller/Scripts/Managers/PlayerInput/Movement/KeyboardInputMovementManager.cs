using UnityEngine;

namespace Controller.Scripts.Managers.PlayerInput.Movement
{
    public class KeyboardInputMovementManager: MovementInputManager
    {
        public override void MovementInput()
        {
            // Check if conflicting vertical keys are pressed, otherwise set vertical input
            if (Input.GetKey(InputUtils.Forward) && Input.GetKey(InputUtils.Backward))
                verticalInput = 0;
            else
                verticalInput = Input.GetKey(InputUtils.Forward) ? 1 : Input.GetKey(InputUtils.Backward) ? -1 : 0;

            
            // Check if conflicting horizontal keys are pressed, otherwise set horizontal input
            if (Input.GetKey(InputUtils.Left) && Input.GetKey(InputUtils.Right))
                horizontalInput = 0;
            else
                horizontalInput = Input.GetKey(InputUtils.Left) ? -1 : Input.GetKey(InputUtils.Right) ? 1 : 0;

            
            if(Input.GetKey(InputUtils.Break))
                MovementManager.ToggleBreak();
        }
    }
}