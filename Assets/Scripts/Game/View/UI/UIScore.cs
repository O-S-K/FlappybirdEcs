using UnityEngine;
using TMPro;

namespace FlappyECS
{
    public class UIScore : MonoBehaviour
    {
        [SerializeField] private TMP_Text scoreText;

        private void Start()
        {
            ScoreIecsSystem.OnScoreChanged += UpdateScore;
        }

        private void OnDestroy()
        {
            ScoreIecsSystem.OnScoreChanged -= UpdateScore;
        }

        private void UpdateScore(int score)
        {
            scoreText.text = score.ToString();
        }
    }
}