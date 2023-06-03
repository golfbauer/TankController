using System;
using System.Collections.Generic;
using Controller.Scripts.Managers.PlayerCamera.CameraUI.UIElementData;
using Controller.Scripts.Managers.PlayerCamera.CameraUI.UIElements;
using UnityEngine;

namespace Controller.Scripts.Managers.PlayerCamera.CameraUI
{
    [Serializable]
    public class CameraUIController : MonoBehaviour
    {
        [SerializeField] public List<UIElement> uiElements = new List<UIElement>();
        // I doubt that I need this, but this took me an entire day to get this to work, so I'm keeping it
        public List<UIElementData.UIElementData> uiElementsData = new List<UIElementData.UIElementData>();
        
        [SerializeField] public GameObject canvas;
        public bool isActive;

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
        
        public void UpdateActiveUIElements()
        {
            List<int> emptySlots = new List<int>();
            foreach(UIElement element in uiElements)
            {
                if (element == null)
                    return;
                
                if(isActive)
                {
                    element.Activate();
                }
                else
                {
                    element.Deactivate();
                }
            }
            RemoveEmptySlots(emptySlots);
        }
        
        public void ToggleAllUIElements(bool setActive)
        {
            isActive = setActive;
            UpdateActiveUIElements();
        }
        
        public void RemoveEmptySlots(List<int> emptySlots)
        {
            foreach(int index in emptySlots)
            {
                uiElements.RemoveAt(index);
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
            GameObject uiGameObject = new GameObject("UIElement");
            RectTransform rectTransform = uiGameObject.AddComponent<RectTransform>();
            
            uiGameObject.transform.SetParent(canvas.transform, false);
            rectTransform.anchoredPosition = new Vector2(0, 0);
            rectTransform.sizeDelta = new Vector2(100, 100);
            
            UIElementData.UIElementData elementData = CreateUIElementData(elementType);
            UIElement uiElement = CreateUIElement(elementData, uiGameObject);

            return uiElement;
        }


        public UIElement ChangeTypeOfUIElement(int index, UIElementType uiElementType)
        {
            if(index < uiElements.Count)
            {
                UIElement uiElement = uiElements[index];
                GameObject uiGameObject = uiElement.gameObject;
                uiElements.RemoveAt(index);
                
                UIElementData.UIElementData elementData = CreateUIElementData(uiElementType);
                UIElement newUIElement = CreateUIElement(elementData, uiGameObject);
                
                DestroyImmediate(uiElement);
                return newUIElement;
            }

            return null;
        }
        
        private UIElementData.UIElementData CreateUIElementData(UIElementType elementType)
        {
            UIElementData.UIElementData uiElementData;
            switch (elementType)
            {
                case UIElementType.Basic:
                    uiElementData = ScriptableObject.CreateInstance<UIElementData.UIElementData>();
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

        private UIElement CreateUIElement(UIElementData.UIElementData elementData, GameObject uiGameObject)
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