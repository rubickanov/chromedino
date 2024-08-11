using System;
using System.Drawing;
using UnityEngine;
using UnityEngine.SceneManagement;
using Action = System.Action;

namespace Rubickanov.Dino.Core
{
    public class Game
    {
        public Trex Trex;
        public ObstacleGenerator ObstacleGenerator;
        public Score Score;

        public float GameSpeed;

        public bool IsGameStarted = false;
        private bool isGameRunning = false;
        public bool IsGameRunning => isGameRunning;

        private GameConfig config;
        
        public event Action OnGameOver;

        public Game(GameConfig config)
        {
            this.config = config;
            ObstacleGenerator = new ObstacleGenerator(this, config.generatorConfig);
            Trex = new Trex(config.trexConfig);
            Score = new Score(config.scoreConfig);
        }

        public void Start()
        {
            OnJumpReleased(); // HOTFIX
            Trex.Start();
            ObstacleGenerator.Start();
            isGameRunning = true;
            IsGameStarted = true;
            GameSpeed = config.startGameSpeed;
        }

        public void Update(float deltaTime)
        {
            if (!isGameRunning) return;
            
            GameSpeed += config.gameSpeedMultiplier * deltaTime;
            
            ObstacleGenerator.Update(deltaTime);
            Trex.Update(deltaTime);
            Score.Update(deltaTime, GameSpeed);
            if (BetterCheckCollision())
            {
                GameOver();
            }
        }

        public bool BetterCheckCollision()
        {
            foreach (var Obstacle in ObstacleGenerator.CurrentObstacles)
            {
                if (Obstacle.posX <= config.trexConfig.initPosX + Trex.width &&
                    Obstacle.posX + Obstacle.config.width >= config.trexConfig.initPosX &&
                    Trex.CurrentPosY <= Obstacle.config.height + Obstacle.posY &&
                    Trex.CurrentPosY + Trex.height >= Obstacle.posY)
                {
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

        public void GameOver()
        {
            isGameRunning = false;
            OnGameOver?.Invoke();
            Trex.Die();
            Score.CheckBestScore();
        }

        public void OnDuck()
        {
            Trex.wantToDuck = true;
        }

        public void OnDuckReleased()
        {
            Trex.wantToDuck = false;
        }
    }
}