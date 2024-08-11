using System;
using Rubickanov.Dino.Core;
using UnityEngine;

public class DinoRunner : MonoBehaviour
{
    public Game game;

    [SerializeField] private MeshRenderer ground;
    [SerializeField] private Transform player;

    [SerializeField] public GameConfig config;

    [SerializeField] private GameObject[] obstacles;

    private void Start()
    {
        game = new Game(config);
        game.ObstacleGenerator.OnObstacleGenerated += OnObstacleGenerated;
        StartGame();
        
        player.position = new Vector3(config.trexConfig.initPosX, config.trexConfig.initPosY, player.position.z);
    }

    private void OnObstacleGenerated(Obstacle obstacle)
    {
        GameObject obstaclePrefab;
        if (obstacle is LargeCactus)
        {
            obstaclePrefab = obstacles[0];
        }
        else if (obstacle is SmallCactus)
        {
            obstaclePrefab = obstacles[1];
        }
        else if (obstacle is TwoLargeCactus)
        {
            obstaclePrefab = obstacles[2];
        }
        else
        {
            throw new ArgumentOutOfRangeException();
        }

        GameObject obstacleInstance = Instantiate(obstaclePrefab);
        obstacle.OnPositionChanged += (obstacle) => UpdateObstaclePosition(obstacle, obstacleInstance);
    }

    private void UpdateObstaclePosition(Obstacle obstacle, GameObject obstacleInstance)
    {
        obstacleInstance.transform.position = new Vector3(obstacle.posX, 0, 0);
    }

    private void StartGame()
    {
        game.Start();
    }

    private void Update()
    {
        game.Update(Time.deltaTime);

        ground.material.mainTextureOffset += new UnityEngine.Vector2(game.GameSpeed * Time.deltaTime, 0);

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space))
        {
            game.OnJump();
        }

        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.Space))
        {
            game.OnJumpReleased();
        }

        player.transform.position = new Vector3(player.position.x, game.GetTrexPosY(), player.position.z);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(player.transform.position,
            new Vector3(config.trexConfig.width, config.trexConfig.height, 1));
        Gizmos.DrawWireSphere(new Vector3(config.generatorConfig.spawnPosX, 0, 0), 0.3f);
    }
}