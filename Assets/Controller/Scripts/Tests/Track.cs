using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Controller.Scripts
{
    public class Track
    {
        private List<GameObject> StaticWheels { get; set; }

        private List<GameObject> SuspensionWheels { get; set; }

        private GameObject DriveWheel { get; set; }
        
        public float RotationSpeed { get; set; }

        public Track(List<GameObject> suspensionWheels, GameObject driveWheel, List<GameObject> staticWheels = null)
        {
            StaticWheels = staticWheels;
            SuspensionWheels = suspensionWheels;
            DriveWheel = driveWheel;
            
            AddCollider(StaticWheels);
            AddCollider(SuspensionWheels);
            AddCollider(DriveWheel);
        }

        private static void AddCollider(GameObject wheel)
        {
            if (wheel.GetComponent<Collider>() == null)
            {
                MeshCollider collider = wheel.AddComponent<MeshCollider>();
                collider.convex = true;
            }
        }
        
        private static void AddCollider(List<GameObject> wheels)
        {
            foreach (var wheel in wheels)
                AddCollider(wheel);
        }
        
        public void Rotate()
        {
            foreach (var wheel in StaticWheels)
            {
                wheel.transform.Rotate(Vector3.back, RotationSpeed);
            }
            
            foreach (var wheel in SuspensionWheels)
            {
                wheel.transform.Rotate(Vector3.back, RotationSpeed);
            }
            
            DriveWheel.transform.Rotate(Vector3.back, RotationSpeed);
        }
    }
}