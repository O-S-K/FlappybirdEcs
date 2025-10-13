using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyECS
{
    public class ScoreData 
    {
        public const string HighScoreKey = "HighScore";
        public static int CurrentScore { get; private set; }
        public static int HighScore { get; private set; }
        
        public ScoreData()
        {
            CurrentScore = 0;
            HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        }
        
        public static void AddScore(int amount)
        {
            CurrentScore += amount;
            if (CurrentScore > HighScore)
            {
                HighScore = CurrentScore;
                PlayerPrefs.SetInt(HighScoreKey, HighScore);
                PlayerPrefs.Save();
            }
        }
        
        public static void ResetScore()
        {
            CurrentScore = 0;
        }
        
        public static void LoadHighScore()
        {
            HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        }
    }
}
