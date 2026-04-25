using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FlappyECS
{
    public class UIBuffItem : MonoBehaviour
    {
        public Image iconImage;
        public Image fillImage;
        public TMP_Text timeText;

        public void UpdateBuff(BuffDisplayData data)
        {
            fillImage.fillAmount = data.remainingTime / data.totalTime;
            if (timeText != null)
                timeText.text = data.remainingTime.ToString("F1") + "s";
            
            // Đổi màu hoặc icon tùy loại (nếu bạn có sprite)
            iconImage.color = GetColor(data.type);
        }

        private Color GetColor(ItemType type)
        {
            return type switch
            {
                ItemType.Invincibility => Color.white,
                ItemType.ScoreMultiplier => Color.yellow,
                ItemType.TimeWarp => Color.cyan,
                ItemType.Shield => Color.blue,
                ItemType.Shrink => Color.green,
                ItemType.GravityFlip => Color.magenta,
                ItemType.Magnet => new Color(1f, 0.5f, 0f), // Cam
                _ => Color.white
            };
        }
    }
}
