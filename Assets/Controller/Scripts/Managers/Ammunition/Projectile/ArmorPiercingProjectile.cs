using Controller.Scripts.Editors.Utils;
using UnityEngine;

namespace Controller.Scripts.Managers.Ammunition.Projectile
{
    public class ArmorPiercingProjectile : BaseProjectile
    {

        protected override float GetDragCoefficient()
        {
            throw new System.NotImplementedException();
        }

        protected override float GetReferenceArea()
        {
            throw new System.NotImplementedException();
        }

        protected override void HandleTankCollision(Collision collision, GameObject hitTank)
        {
            
        }

        protected override void HandleGenericCollision(Collision collision, GameObject hitObject)
        {
            
        }

        public override void EditorSetUp()
        {
        }
    }
}