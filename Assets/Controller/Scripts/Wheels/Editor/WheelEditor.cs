using Controller.Scripts.Utils;
using Controller.Scripts.Wheels.Services;
using UnityEngine;

namespace Controller.Scripts.Wheels
{
    public class WheelEditor : TankComponentEditor
    {
        protected virtual void OnSceneGUI()
        {
            for (var i = 0; i < transform.childCount; i++)
            {
                var wheel = transform.GetChild(i);

                if (!wheel.name.Contains("Wheel"))
                    continue;

                var hingeAxis = wheel.GetComponent<HingeJoint>().axis;
                var torqueDir = wheel.GetComponent<WheelManager>().torqueDirection;
                DrawUtils.DrawCircleWithDirection(wheel, hingeAxis, torqueDir, 0.1f);
            }
        }

        protected void BulkDestroyComponents()
        {
            var childCount = transform.childCount;
            for (var i = 0; i < childCount; i++)
                DestroyImmediate(transform.GetChild(0).gameObject);
        }

        protected void AttachWheelHingeJoint(GameObject wheel, Transform connectedTo, Vector3 axis)
        {
            var wheelHingeJoint = wheel.AddComponent<HingeJoint>();
            wheelHingeJoint.anchor = Vector3.zero;
            wheelHingeJoint.axis = axis;
            wheelHingeJoint.useSpring = false;
            wheelHingeJoint.useMotor = false;
            wheelHingeJoint.useLimits = false;
            wheelHingeJoint.connectedBody = connectedTo.GetComponent<Rigidbody>();
        }

        protected void AttachWheelScript(GameObject wheel, bool isLeft, Vector3 torqueDirection)
        {
            var wheelManagerController = wheel.AddComponent<WheelManager>();

            wheelManagerController.WheelGroupManager =
                transform.parent.GetComponent<WheelGroupManager>();
            wheelManagerController.isLeftWheel = isLeft;
            wheelManagerController.torqueDirection = torqueDirection;
        }

        protected void AttachWheelManager(float wheelRadius)
        {
            var wheelGroupManager = transform.GetComponent<WheelGroupManager>();
            if (wheelGroupManager == null)
                wheelGroupManager = transform.gameObject.AddComponent<WheelGroupManager>();

            wheelGroupManager.wheelRadius = wheelRadius;
        }

        protected void AttachWheelResizeScript(
            GameObject wheel,
            bool attachResize,
            float wheelScale,
            float time
        )
        {
            if (!attachResize)
            {
                var resize = wheel.GetComponent<WheelResize>();
                if (resize != null)
                    DestroyImmediate(resize);
                return;
            }

            var wheelResize = wheel.GetComponent<WheelResize>();
            if (wheelResize == null)
                wheelResize = wheel.AddComponent<WheelResize>();

            wheelResize.wheelResizeScale = wheelScale;
            wheelResize.wheelResizeSpeed = time;
        }

        protected virtual void SetLayers()
        {
            for (var i = 0; i < transform.childCount; i++)
            {
                var components = transform.GetChild(i);
                if (components.name.Contains("Wheel"))
                    LayerUtils.SetLayer(components.gameObject, LayerUtils.WheelLayer);
            }
        }
    }
}
