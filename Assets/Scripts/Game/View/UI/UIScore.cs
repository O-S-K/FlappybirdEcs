using UnityEngine;
using TMPro;

namespace FlappyECS
{
    public class UIScore : MonoBehaviour
    {
        [SerializeField] private TMP_Text scoreText;

        private void Start()
        {
            ScoreSystem.OnScoreChanged += UpdateScore;
        }

        private void OnDestroy()
        {
            ScoreSystem.OnScoreChanged -= UpdateScore;
        }

        private void UpdateScore(int score)
        {
            scoreText.text = score.ToString();
        }
    }
}