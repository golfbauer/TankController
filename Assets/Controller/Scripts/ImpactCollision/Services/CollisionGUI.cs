using Controller.Scripts.Utils;
using UnityEngine;

namespace Controller.Scripts.ImpactCollision.Services
{
    public static class CollisionGUI
    {
        public static bool AddVertexButton(bool addVertex)
        {
            return GUILayout.Button(
                !addVertex ? CollisionMessages.AddVertex : GeneralMessages.Cancel
            );
        }
    }
}
