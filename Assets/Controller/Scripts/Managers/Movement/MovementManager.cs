using System.Collections.Generic;
using Controller.Scripts.Managers.Wheels;
using UnityEngine;

namespace Controller.Scripts.Managers.Movement
{
    public class MovementManager: MonoBehaviour
    {
        [SerializeField] private float speed;
        
        public List<WheelManager> wheelManagers;
        
        
        
        private void Awake()
        {
            wheelManagers = new List<WheelManager>();
            
            foreach (Transform child in transform)
            {
                var wheelManager = child.GetComponent<WheelManager>();
                if (wheelManager != null)
                    wheelManagers.Add(wheelManager);
            }
        }
    }
}