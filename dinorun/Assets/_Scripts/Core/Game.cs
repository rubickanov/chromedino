using System;

namespace Rubickanov.Dino.Core
{
    public class Game
    {
        public Trex Trex;
        public ObstacleGenerator ObstacleGenerator;
        public Score Score;

        public float GameSpeed;

        public bool IsGameStarted = false;
        public bool isGameRunning = false;
        public bool IsGameRunning => isGameRunning;

        public GameConfig config;
        
        public event Action OnGameOver;

        public Game(GameConfig config)
        {
            this.config = config;
            ObstacleGenerator = new ObstacleGenerator(config.generatorConfig);
            Trex = new Trex(config.trexConfig);
            Score = new Score(config.scoreConfig);
        }

        public void PreStart()
        {
            Trex.Start();
        }

        public void Start()
        {
            ObstacleGenerator.Start();
            isGameRunning = true;
            IsGameStarted = true;
            GameSpeed = config.startGameSpeed;
        }

        public void Update(float deltaTime)
        {
            Trex.Update(deltaTime);
            if (!isGameRunning) return;
            
            GameSpeed += config.gameSpeedMultiplier * deltaTime;
            
            ObstacleGenerator.Update(GameSpeed, deltaTime);
            Score.Update(deltaTime, GameSpeed);
            if (CheckCollision())
            {
                GameOver();
            }
        }

        public bool CheckCollision()
        {
            foreach (var Obstacle in ObstacleGenerator.CurrentObstacles)
            {
                if (Obstacle.PosX <= config.trexConfig.initPosX + Trex.width &&
                    Obstacle.PosX + Obstacle.config.width >= config.trexConfig.initPosX &&
                    Trex.CurrentPosY <= Obstacle.PosY + Obstacle.config.height / 2 &&
                    Trex.CurrentPosY + Trex.height >= Obstacle.PosY - Obstacle.config.height / 2)
                {
                    return true;
                }
            }

            return false;
        }

        // public bool CheckCollision()
        // {
        //     float trexPosX = config.trexConfig.initPosX;
        //     float trexPosY = Trex.CurrentPosY;
        //
        //     RectangleF trexRect = new RectangleF(trexPosX, trexPosY, config.trexConfig.width, config.trexConfig.height);
        //
        //     foreach (Obstacle obstacle in ObstacleGenerator.CurrentObstacles)
        //     {
        //         float obstaclePosX = obstacle.PosX;
        //         float obstaclePosY = 0; // HARDCODE
        //
        //         RectangleF obstacleRect = new RectangleF(obstaclePosX, obstaclePosY, obstacle.config.width,
        //             obstacle.config.height);
        //
        //         if (trexRect.IntersectsWith(obstacleRect))
        //         {
        //             return true;
        //         }
        //     }
        //
        //     return false;
        // }

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

        public void Reset()
        {
            Trex.Reset();
            ObstacleGenerator.Reset();
            Score.Reset();
            isGameRunning = false;
            IsGameStarted = false;
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