using System;
using UnityEngine;

namespace Rubickanov.Dino.Core
{
    public class Score
    {
        private float bestScore;
        private float score;
        private float lastScoreEvent;
        
        private ScoreConfig config;

        public event Action OnHundredScore;
        public event Action<float> OnHighScoreChanged;
        public event Action<float> OnScoreChanged;
        public Score(ScoreConfig config)
        {
            this.config = config;
        }

        public void Update(float deltaTime, float gameSpeed)
        {
            score += config.scoreMultiplier * gameSpeed * deltaTime;
            OnScoreChanged?.Invoke(score);

            float currentScoreEvent = Mathf.Floor(score / 100);
            if (currentScoreEvent > lastScoreEvent)
            {
                OnHundredScore?.Invoke();
                lastScoreEvent = currentScoreEvent;
            }
        }

        public void CheckBestScore()
        {
            if (score > bestScore)
            {
                bestScore = score;
                OnHighScoreChanged?.Invoke(bestScore);
            }
        }
    }
}