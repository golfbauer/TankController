using System;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Controller.Scripts.Managers.Tracks
{
    public class ChainLinkBalancer: MonoBehaviour
    {
        /*
         * Lets hope this is only a temporary solution. -> Nvmd doesnt work either.
         * The issue is, that the chain needs to have a steady position (Z) and rotation (X, Y).
         * I already tried using a FixedJoint on a couple of links, but that didn't work out.
         * I also tried to freeze the position and rotation of the chain, using the Rigidbody.
         * And I tried using a ConfigurableJoint, which also only had some minor successes.
         *
         * Maybe I can freeze every x link???
         */
        private Vector3 _fixedPosition;
        private Quaternion _fixedRotation;
        
        private void Awake()
        {
            _fixedPosition = transform.localPosition;
            _fixedRotation = transform.localRotation;
        }
        
        private void FixedUpdate()
        {
            Vector3 newPosition = new Vector3(_fixedPosition.x, _fixedPosition.y, transform.localPosition.z);
            Quaternion newRotation = Quaternion.Euler(_fixedRotation.eulerAngles.x, _fixedRotation.eulerAngles.y, transform.localRotation.eulerAngles.z);
        
            transform.localPosition = newPosition;
            transform.localRotation = newRotation;
        }
    }
}