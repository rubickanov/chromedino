using System;

namespace Rubickanov.Dino.Core
{
    public class Obstacle
    {
        public ObstacleConfig config;
        public ObstacleType type;
        private float _posX;
        private float _posY;

        public float posX
        {
            get => _posX;
            set
            {
                _posX = value;
                OnPositionChanged?.Invoke();
            }
        }

        public float posY
        {
            get => _posY;
            set => _posY = value;
        }

        public event Action OnPositionChanged;
        public event Action OnDestroyed;

        public Obstacle(ObstacleConfig config, ObstacleType type)
        {
            this.config = config;
            this.type = type;
        }

        public void Destroy()
        {
            OnDestroyed?.Invoke();
        }

        public float GetRandomYPos()
        {
            Random random = new Random();
            float randomHeight = (float)random.NextDouble() * (1.6f - 0.4f) + 0.4f;
            return randomHeight;
        }
    }
}