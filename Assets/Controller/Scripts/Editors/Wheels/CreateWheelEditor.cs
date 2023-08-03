using System;
using Controller.Scripts.Editors.Utils;
using Controller.Scripts.Managers.Wheels;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Controller.Scripts.Editors.Wheels
{
    public class CreateWheelEditor : TankComponentEditor
    {
        protected void BulkDestroyComponents()
        {
            var childCount = transform.childCount;
            for (var i = 0; i < childCount; i++)
                DestroyImmediate(transform.GetChild(0).gameObject);
        }

        protected void AttachWheelHingeJoint(GameObject wheel,
            Transform connectedTo, Vector3 axis)
        {
            HingeJoint wheelHingeJoint = wheel.AddComponent<HingeJoint>();
            wheelHingeJoint.anchor = Vector3.zero;
            wheelHingeJoint.axis = axis;
            wheelHingeJoint.useSpring = false;
            wheelHingeJoint.useMotor = false;
            wheelHingeJoint.useLimits = false;
            wheelHingeJoint.connectedBody =
                connectedTo.GetComponent<Rigidbody>();
        }

        protected void AttachWheelScript(GameObject wheel, bool isLeft,
            Vector3 torqueDirection)
        {
            Wheel wheelController = wheel.AddComponent<Wheel>();

            wheelController.WheelManager =
                transform.parent.GetComponent<WheelManager>();
            wheelController.isLeftWheel = isLeft;
            wheelController.torqueDirection = torqueDirection;
        }

        protected void AttachWheelManager(float wheelRadius)
        {
            WheelManager wheelManager =
                transform.GetComponent<WheelManager>();
            if (wheelManager == null)
                wheelManager =
                    transform.gameObject.AddComponent<WheelManager>();

            wheelManager.wheelRadius = wheelRadius;
        }

        protected void AttachWheelResizeScript(GameObject wheel,
            bool attachResize, float wheelScale, float time)
        {
            if (!attachResize)
            {
                WheelResize resize = wheel.GetComponent<WheelResize>();
                if (resize != null)
                    DestroyImmediate(resize);
                return;
            }

            WheelResize wheelResize = wheel.GetComponent<WheelResize>();
            if (wheelResize == null)
                wheelResize = wheel.AddComponent<WheelResize>();

            wheelResize.wheelResizeScale = wheelScale;
            wheelResize.wheelResizeSpeed = time;
        }

        protected virtual void OnSceneGUI()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform wheel = transform.GetChild(i);

                if (!wheel.name.Contains("Wheel"))
                    continue;

                Vector3 hingeAxis = wheel.GetComponent<HingeJoint>().axis;
                Vector3 torqueDir =
                    wheel.GetComponent<Wheel>().torqueDirection;
                DrawUtils.DrawCircleWithDirection(wheel, hingeAxis, torqueDir,
                    0.1f);
            }
        }

        protected virtual void SetLayers()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform components = transform.GetChild(i);
                if (components.name.Contains("Wheel"))
                    LayerUtils.SetLayer(components.gameObject,
                        LayerUtils.WheelLayer);
            }
        }
    }
}