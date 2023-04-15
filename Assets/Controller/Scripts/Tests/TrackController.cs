using System.Collections.Generic;
using UnityEngine;

namespace Controller.Scripts
{
    public class TrackController: MonoBehaviour
    {
        private Track RightTrack { get; set; }
        private Track LeftTrack { get; set; }
        
        public TrackController(Track rightTrack, Track leftTrack)
        {
            RightTrack = rightTrack;
            LeftTrack = leftTrack;
        }
        
        public void SetUpTracks(Track rightTrack, Track leftTrack)
        {
            RightTrack = rightTrack;
            LeftTrack = leftTrack;
            rightTrack.RotationSpeed = 5;
            leftTrack.RotationSpeed = 5;
        }

        private void FixedUpdate()
        {
            RightTrack.Rotate();
            LeftTrack.Rotate();
        }
    }
}