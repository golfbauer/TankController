using UnityEngine;

namespace Controller.Scripts.Managers.Wheels
{
    public class WheelResize : MonoBehaviour
    {
        public float wheelResizeScale = 1.0f;
        public float wheelResizeSpeed = 1.0f;

        private Vector3 _initWheelScale;
        
        private void Start()
        {
            _initWheelScale = transform.localScale;
            transform.localScale = _initWheelScale * wheelResizeScale;
        }

        private void Update()
        {
            transform.localScale = Vector3.Lerp(transform.localScale, _initWheelScale, wheelResizeSpeed * Time.deltaTime);
        }
    }
}