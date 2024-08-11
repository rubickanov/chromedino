using System;

namespace Rubickanov.Dino.Core
{
    public abstract class Obstacle
    {
        public ObstacleConfig config;
        private float _posX;

        public float posX
        {
            get => _posX;
            set
            {
                _posX = value;
                OnPositionChanged?.Invoke(this);
            }
        }

        public event Action<Obstacle> OnPositionChanged;

        public Obstacle(ObstacleConfig config)
        {
            this.config = config;
        }
    }
}