using System.Drawing;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rubickanov.Dino.Core
{
    public class Game
    {
        public Trex Trex;
        public ObstacleGenerator ObstacleGenerator;

        private int currentSpeedIndex = 0;
        public float GameSpeed => config.isSlowMode
            ? config.slowGameSpeeds[currentSpeedIndex]
            : config.normalGameSpeeds[currentSpeedIndex];


        private GameConfig config;

        public Game(GameConfig config)
        {
            this.config = config;
            ObstacleGenerator = new ObstacleGenerator(this, config.generatorConfig);
            Trex = new Trex(config.trexConfig);
        }

        public void Start()
        {
            OnJumpReleased(); // HOTFIX
            Trex.Start();
            ObstacleGenerator.Start();
        }

        public void Update(float deltaTime)
        {
            ObstacleGenerator.Update(deltaTime);
            Trex.Update(deltaTime);
            if (BetterCheckCollision())
            {
                SceneManager.LoadScene(0);
            }
        }

        public void NextSpeed()
        {
        }

        public bool BetterCheckCollision()
        {
            foreach (var Obstacle in ObstacleGenerator.CurrentObstacles)
            {
                if (Obstacle.posX <= config.trexConfig.initPosX + config.trexConfig.width &&
                    Obstacle.posX + Obstacle.config.width >= config.trexConfig.initPosX &&
                    Trex.CurrentPosY <= Obstacle.config.height)
                {
                    Debug.Log("DEAD");
                    return true;
                }
            }

            return false;
        }

        public bool CheckCollision()
        {
            float trexPosX = config.trexConfig.initPosX;
            float trexPosY = Trex.CurrentPosY;

            RectangleF trexRect = new RectangleF(trexPosX, trexPosY, config.trexConfig.width, config.trexConfig.height);

            foreach (Obstacle obstacle in ObstacleGenerator.CurrentObstacles)
            {
                float obstaclePosX = obstacle.posX;
                float obstaclePosY = 0; // HARDCODE

                RectangleF obstacleRect = new RectangleF(obstaclePosX, obstaclePosY, obstacle.config.width,
                    obstacle.config.height);

                if (trexRect.IntersectsWith(obstacleRect))
                {
                    return true;
                }
            }

            return false;
        }

        public void OnJump()
        {
            Trex.wantToJump = true;
        }

        public void OnJumpReleased()
        {
            Trex.jumpReleased = true;
            Trex.wantToJump = false;
        }

        public float GetTrexPosY()
        {
            return Trex.CurrentPosY;
        }
    }
}