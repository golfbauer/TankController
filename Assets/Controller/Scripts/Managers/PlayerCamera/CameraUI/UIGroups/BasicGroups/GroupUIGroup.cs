using System.Collections.Generic;

namespace Controller.Scripts.Managers.PlayerCamera.CameraUI.UIGroups.BasicUIGroup
{
    public class GroupUIGroup : UIGroup
    {
        public List<UIGroup> groups = new ();
        
        public override void CleanUpUIElements()
        {
            groups.RemoveAll(group => group == null);
        }
        
        public override void PerformUpdateAction()
        {
            // Do nothing
        }
        
        public override void Initialize()
        {
            type = UIGroupType.Group;
        }
    }
}