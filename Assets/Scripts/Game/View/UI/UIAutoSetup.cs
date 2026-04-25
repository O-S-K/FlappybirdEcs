using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace FlappyECS
{
    public class UIAutoSetup : MonoBehaviour
    {
        public Sprite defaultBuffIcon; // Bạn có thể kéo icon vào đây

        void Start()
        {
            CreateBuffUI();
        }

        void CreateBuffUI()
        {
            // 1. Tạo Canvas nếu chưa có
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasGo = new GameObject("UI_Canvas");
                canvas = canvasGo.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasGo.AddComponent<CanvasScaler>();
                canvasGo.AddComponent<GraphicRaycaster>();
            }

            // 2. Tạo Buff Container (Góc trái trên)
            GameObject containerGo = new GameObject("BuffContainer");
            containerGo.transform.SetParent(canvas.transform, false);
            
            RectTransform rect = containerGo.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(20, -20);
            rect.sizeDelta = new Vector2(300, 100);

            VerticalLayoutGroup layout = containerGo.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 5;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
            layout.childAlignment = TextAnchor.UpperLeft;

            // 3. Tạo Prefab cho Buff Item (ẩn đi)
            GameObject itemPrefab = CreateBuffItemPrefab();
            itemPrefab.transform.SetParent(transform, false);
            itemPrefab.SetActive(false);

            // 4. Gắn script quản lý chính
            UIBuffDisplay display = containerGo.AddComponent<UIBuffDisplay>();
            display.container = containerGo.transform;
            display.buffItemPrefab = itemPrefab;
        }

        GameObject CreateBuffItemPrefab()
        {
            GameObject item = new GameObject("BuffItemTemplate");
            RectTransform rect = item.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(50, 50);

            // Icon nền
            Image icon = item.AddComponent<Image>();
            icon.sprite = defaultBuffIcon;

            // Thanh Fill (để hiện thời gian chạy ngược)
            GameObject fillGo = new GameObject("FillOverlay");
            fillGo.transform.SetParent(item.transform, false);
            RectTransform fillRect = fillGo.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.sizeDelta = Vector2.zero;

            Image fillImg = fillGo.AddComponent<Image>();
            fillImg.color = new Color(0, 0, 0, 0.5f);
            fillImg.type = Image.Type.Filled;
            fillImg.fillMethod = Image.FillMethod.Radial360;
            fillImg.fillAmount = 1f;

            // Gắn script BuffItem
            UIBuffItem buffItem = item.AddComponent<UIBuffItem>();
            buffItem.iconImage = icon;
            buffItem.fillImage = fillImg;

            return item;
        }
    }
}
