using System;

namespace Rubickanov.Dino.Core
{
    [Serializable]
    public struct GameConfig
    {
        public float startGameSpeed;
        public float gameSpeedMultiplier;
        public TrexConfig trexConfig;
        public GeneratorConfig generatorConfig;
        public ScoreConfig scoreConfig;
    }
    
    [Serializable]
    public struct TrexConfig
    {
        public float width;
        public float height;
        public float duckHeight;
        public float initPosX;
        public float initPosY;
        public float maxPosY;
        public float jumpMultiplier;
        public float gravityMultiplier;
        public float maxJumpTime;
        public float minJumpTime;
    }
    
    [Serializable]
    public struct ObstacleConfig
    {
        public float yPos;
        public float width;
        public float height;
    }

    [Serializable]
    public struct GeneratorConfig
    {
        public float spawnPosX;
        public float despawnPosX;
        public float timeDecreaseFactor;
        public float minTimeBetweenObstacles;
        public float maxTimeBetweenObstacles;
        public ObstacleConfig smallCactusConfig;
        public ObstacleConfig largeCactusConfig;
        public ObstacleConfig twoLargeCactusConfig;
        public ObstacleConfig twoSmallCactusConfig;
        public ObstacleConfig threeSmallCactusConfig;
        public ObstacleConfig mixCactusConfig;
        public ObstacleConfig birdConfig;
    }

    [Serializable]
    public struct ScoreConfig
    {
        public int scoreMultiplier;
    }
}