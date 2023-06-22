using System;
using System.Collections.Generic;
using Controller.Scripts.Managers.PlayerCamera.CameraUI.ElementData;
using Controller.Scripts.Managers.PlayerCamera.CameraUI.Elements;
using Unity.VisualScripting;
using UnityEngine;

namespace Controller.Scripts.Managers.PlayerCamera.CameraUI
{
    [Serializable]
    public class CameraUIManager : MonoBehaviour
    {
        [SerializeField] public List<UIElement> uiElements = new List<UIElement>();
        // I doubt that I need this, but this took me an entire day to get this to work, so I'm keeping it
        public List<UIElementData> uiElementsData = new List<UIElementData>();
        
        [SerializeField] public GameObject canvas;
        public bool isActive = false;

        private void Awake()
        {
            UpdateUIElements();
        }

        public void ActivateUIElement(int index)
        {
            if(index < uiElements.Count)
            {
                uiElements[index].Activate();
            }
        }

        public void DeactivateUIElement(int index)
        {
            if(index < uiElements.Count)
            {
                uiElements[index].Deactivate();
            }
        }

        public void UpdateUIElement(UIElement uiElement)
        {
            if(isActive)
                uiElement.Activate();
            else
                uiElement.Deactivate();
        }
        
        public void ToggleUIElements(bool setActive)
        {
            isActive = setActive;
            UpdateUIElements();
        }

        public void UpdateUIElements()
        {
            uiElements.RemoveAll(element => element == null);
            uiElementsData.RemoveAll(element => element == null);

            foreach(var element in uiElements)
            {
                UpdateUIElement(element);
            }
            
            for(int i = uiElementsData.Count - 1; i >= 0; i--)
            {
                bool isOrphan = true;
                foreach(var element in uiElements)
                {
                    if (element.Data == uiElementsData[i])
                    {
                        isOrphan = false;
                        break;
                    }
                }

                if (isOrphan)
                {
                    DestroyImmediate(uiElementsData[i]);
                    uiElementsData.RemoveAt(i);
                }
            }
        }

        void Update()
        {
            foreach(UIElement element in uiElements)
            {
                if(element.gameObject.activeInHierarchy)
                {
                    element.PerformUpdateAction();
                }
            }
        }

        public UIElement AddNewUIElement(UIElementType elementType)
        {

            if (canvas == null)
            {
                throw new Exception("No canvas assigned to CameraUIController!");
            }
            
            GameObject uiGameObject = new GameObject("UIElement");
            RectTransform rectTransform = uiGameObject.AddComponent<RectTransform>();
            
            uiGameObject.transform.SetParent(canvas.transform, false);
            rectTransform.anchoredPosition = new Vector2(0, 0);
            rectTransform.sizeDelta = new Vector2(100, 100);
            
            UIElementData elementData = CreateUIElementData(elementType);
            UIElement uiElement = CreateUIElement(elementData, uiGameObject);
            
            UpdateUIElement(uiElement);

            return uiElement;
        }


        public UIElement ChangeTypeOfUIElement(int index, UIElementType uiElementType)
        {
            if(index < uiElements.Count)
            {
                UIElement uiElement = uiElements[index];
                GameObject uiGameObject = uiElement.gameObject;
                uiElements.RemoveAt(index);
                uiElementsData.RemoveAt(index);
                
                UIElementData elementData = CreateUIElementData(uiElementType);
                UIElement newUIElement = CreateUIElement(elementData, uiGameObject);
                
                DestroyImmediate(uiElement);
                uiElements.Insert(index, newUIElement);
                uiElementsData.Insert(index, elementData);
                return newUIElement;
            }

            return null;
        }
        
        private UIElementData CreateUIElementData(UIElementType elementType)
        {
            UIElementData uiElementData;
            switch (elementType)
            {
                case UIElementType.Basic:
                    uiElementData = ScriptableObject.CreateInstance<UIElementData>();
                    break;
                case UIElementType.StaticSprite:
                    uiElementData = ScriptableObject.CreateInstance<UISpriteElementData>();
                    break;
                // Add more cases for additional UIElement types
                default:
                    throw new ArgumentOutOfRangeException();

            }
            uiElementData.Type = elementType;
            return uiElementData;
        }

        private UIElement CreateUIElement(UIElementData elementData, GameObject uiGameObject)
        {
            UIElement uiElement;

            switch (elementData.Type)
            {
                case UIElementType.Basic:
                    uiElement = uiGameObject.AddComponent<BasicUIElement>();
                    break;
                case UIElementType.StaticSprite:
                    uiElement = uiGameObject.AddComponent<StaticSpriteUIElement>();
                    break;
                // Add more cases for additional UIElement types
                default:
                    throw new ArgumentOutOfRangeException();
            }

            uiElement.Data = elementData;
            uiElement.InitializeUIElement();

            return uiElement;
        }
    }
}