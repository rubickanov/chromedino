using System;
using System.Collections.Generic;

namespace Rubickanov.Dino.Core
{
    public class ObstacleGenerator
    {
        private Game game;
        private GeneratorConfig config;

        private float timeSinceLastObstacle = 0f;
        private float timeToNextObstacle = 0f;

        private Random random = new Random();

        public List<Obstacle> CurrentObstacles { get; private set; } = new List<Obstacle>();
        public Action<Obstacle> OnObstacleGenerated;

        public void Start()
        {
            GenerateObstacle();
        }

        public ObstacleGenerator(Game game, GeneratorConfig config)
        {
            this.game = game;
            this.config = config;
            timeToNextObstacle = GetRandomTimeToNextObstacle();
        }

        public void Update(float deltaTime)
        {
            timeSinceLastObstacle += deltaTime;

            if (timeSinceLastObstacle >= timeToNextObstacle)
            {
                GenerateObstacle();
                timeSinceLastObstacle = 0f;
                timeToNextObstacle = GetRandomTimeToNextObstacle();
            }

            for (int i = CurrentObstacles.Count - 1; i >= 0; i--)
            {
                CurrentObstacles[i].PosX -= game.GameSpeed * deltaTime;
                if (CurrentObstacles[i].PosX < config.despawnPosX)
                {
                    CurrentObstacles[i].Destroy();
                    CurrentObstacles.RemoveAt(i);
                }
            }
        }

        private float GetRandomTimeToNextObstacle()
        {
            float time =
                (float)random.NextDouble() * (config.maxTimeBetweenObstacles - config.minTimeBetweenObstacles) +
                config.minTimeBetweenObstacles;

            time -= game.GameSpeed * config.timeDecreaseFactor;

            time = Math.Max(time, 0.75f);

            return time;
        }

        private void GenerateObstacle()
        {
            Array obstacleValues = Enum.GetValues(typeof(ObstacleType));

            ObstacleType randomObstacleType = (ObstacleType)obstacleValues.GetValue(random.Next(obstacleValues.Length));
            Obstacle newObstacle = randomObstacleType switch
            {
                ObstacleType.LargeCactus => new Obstacle(config.largeCactusConfig, ObstacleType.LargeCactus),
                ObstacleType.SmallCactus => new Obstacle(config.smallCactusConfig, ObstacleType.SmallCactus),
                ObstacleType.TwoLargeCactus => new Obstacle(config.twoLargeCactusConfig, ObstacleType.TwoLargeCactus),
                ObstacleType.TwoSmallCactus => new Obstacle(config.twoSmallCactusConfig, ObstacleType.TwoSmallCactus),
                ObstacleType.ThreeSmallCactus => new Obstacle(config.threeSmallCactusConfig,
                    ObstacleType.ThreeSmallCactus),
                ObstacleType.MixCactus => new Obstacle(config.threeSmallCactusConfig, ObstacleType.MixCactus),
                ObstacleType.Bird => new Obstacle(config.birdConfig, ObstacleType.Bird),
                _ => throw new ArgumentOutOfRangeException()
            };

            if (newObstacle.type == ObstacleType.Bird)
            {
                newObstacle.PosY = newObstacle.GetRandomYPos();
            }

            newObstacle.PosX = config.spawnPosX;
            CurrentObstacles.Add(newObstacle);

            OnObstacleGenerated?.Invoke(newObstacle);
        }

        public void Reset()
        {
            foreach (var obstacle in CurrentObstacles)
            {
                obstacle.Destroy();
            }
        }
    }
}