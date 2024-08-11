using System;

namespace Rubickanov.Dino.Core
{
    [Serializable]
    public struct GameConfig
    {
        public bool isSlowMode;
        public float[] slowGameSpeeds;
        public float[] normalGameSpeeds;
        public TrexConfig trexConfig;
        public GeneratorConfig generatorConfig;
    }
    
    [Serializable]
    public struct TrexConfig
    {
        public float width;
        public float height;
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
        public float width;
        public float height;
    }

    [Serializable]
    public struct GeneratorConfig
    {
        public float speedMultiplier; // Didn't want to add this but its the only way to sync ground and obstacles speed
        public float spawnPosX;
        public float despawnPosX;
        public float minTimeBetweenObstacles;
        public float maxTimeBetweenObstacles;
        public ObstacleConfig smallCactusConfig;
        public ObstacleConfig largeCactusConfig;
        public ObstacleConfig twoLargeCactusConfig;
    }
}