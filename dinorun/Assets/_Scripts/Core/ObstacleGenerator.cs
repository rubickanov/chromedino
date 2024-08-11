using System;
using System.Collections.Generic;
using Rubickanov.Dino.Core;
using UnityEngine;
using Random = System.Random;

public class ObstacleGenerator
{
    public enum Obstacles
    {
        LargeCactus,
        SmallCactus,
        TwoLargeCactus
    }
    
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
        //this.obstacleConfigs = obstacleConfigs;
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
            CurrentObstacles[i].posX -= game.GameSpeed * deltaTime * config.speedMultiplier;
            if (CurrentObstacles[i].posX + CurrentObstacles[i].config.width < 0)
            {
               // CurrentObstacles.RemoveAt(i);
            }
        }
    }

    private float GetRandomTimeToNextObstacle()
    {
        return (float)random.NextDouble() * (config.maxTimeBetweenObstacles - config.minTimeBetweenObstacles) +
               config.minTimeBetweenObstacles;
    }

    private void GenerateObstacle()
    {
        Array obstacleValues = Enum.GetValues(typeof(Obstacles));

        Obstacles randomObstacle = (Obstacles)obstacleValues.GetValue(random.Next(obstacleValues.Length));

        Obstacle newObstacle;
        switch (randomObstacle)
        {
            case Obstacles.LargeCactus:
                newObstacle = new LargeCactus(config.largeCactusConfig);
                break;
            case Obstacles.SmallCactus:
                newObstacle = new SmallCactus(config.smallCactusConfig);
                break;
            case Obstacles.TwoLargeCactus:
                newObstacle = new TwoLargeCactus(config.twoLargeCactusConfig);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        newObstacle.posX = config.spawnPosX;
        CurrentObstacles.Add(newObstacle);
        
        OnObstacleGenerated?.Invoke(newObstacle);
    }
}