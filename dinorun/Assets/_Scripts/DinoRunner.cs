using System;
using Rubickanov.Dino.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DinoRunner : MonoBehaviour
{
    public static DinoRunner Instance { get; private set; }
    
    public Game Game;

    [SerializeField] private MeshRenderer ground;
    [SerializeField] private Transform player;

    [SerializeField] public GameConfig config;

    [SerializeField] private GameObject[] obstaclesPrefabs;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject startGameUI;
    [SerializeField] private StartGameAnim startGameAnim;

    private bool isGameOver;

    private void Awake()
    {
        Instance = this;
        
        gameOverUI.SetActive(false);

        Game = new Game(config);
        player.position = new Vector3(config.trexConfig.initPosX, config.trexConfig.initPosY, player.position.z);

        Game.OnGameOver += OnGameOver;
        Game.ObstacleGenerator.OnObstacleGenerated += OnObstacleGenerated;

        startGameAnim.OnAnimEnd += StartGame;
    }
    private void OnGameOver()
    {
        isGameOver = true;
        gameOverUI.SetActive(true);
    }


    private void OnObstacleGenerated(Obstacle obstacle)
    {
        GameObject obstaclePrefab = obstacle.type switch
        {
            ObstacleType.LargeCactus => obstaclesPrefabs[0],
            ObstacleType.SmallCactus => obstaclesPrefabs[1],
            ObstacleType.TwoLargeCactus => obstaclesPrefabs[2],
            ObstacleType.TwoSmallCactus => obstaclesPrefabs[3],
            ObstacleType.ThreeSmallCactus => obstaclesPrefabs[4],
            ObstacleType.MixCactus => obstaclesPrefabs[5],
            ObstacleType.Bird => obstaclesPrefabs[6],
            _ => throw new ArgumentOutOfRangeException()
        };

        GameObject obstacleInstance = Instantiate(obstaclePrefab, new Vector3(10, obstacle.posY, 0), Quaternion.identity);
        obstacle.OnPositionChanged += () => UpdateObstaclePosition(obstacle, obstacleInstance);
        obstacle.OnDestroyed += () => DestroyObstacle(obstacle, obstacleInstance);
    }

    private void DestroyObstacle(Obstacle obstacle, GameObject obstacleInstance)
    {
        if (obstacleInstance != null && obstacle != null)
            Destroy(obstacleInstance);
    }

    private void UpdateObstaclePosition(Obstacle obstacle, GameObject obstacleInstance)
    {
        if (obstacle == null || obstacleInstance == null) return;
        obstacleInstance.transform.position = new Vector3(obstacle.posX, obstacle.posY, 0);
    }

    private void StartGame()
    {
        if (isGameOver)
        {
            SceneManager.LoadScene(0); // XALTYRA: This is a hotfix. It should be replaced with a proper game restart.
            return;
        }

        //Play Start game animation
        Game.Start();
        startGameUI.SetActive(false);
        startGameAnim.gameObject.SetActive(false);
    }

    private void Update()
    {
        Game.Update(Time.deltaTime);
        
        MoveGround();
        ReadInput();
        PlayerVisualJump();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(player.transform.position,
            new Vector3(config.trexConfig.width, config.trexConfig.height, 1));
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(player.transform.position,
            new Vector3(config.trexConfig.width, config.trexConfig.duckHeight, 1));
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(new Vector3(config.generatorConfig.spawnPosX, 0, 0), 0.3f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector3(config.generatorConfig.despawnPosX, 0, 0), 0.3f);
    }

    private void ReadInput()
    {
        if (!Game.IsGameStarted && (Input.GetKeyDown(KeyCode.W) || Input.GetMouseButton(0)))
        {
            startGameAnim.Play();
            //StartGame();
        }
        
        if(isGameOver && (Input.GetKeyDown(KeyCode.W) || Input.GetMouseButton(0)))
        {
            SceneManager.LoadScene(0);
        }

        if (Input.GetKey(KeyCode.W))
        {
            Game.OnJump();
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            Game.OnJumpReleased();
        }

        if (Input.GetKey(KeyCode.S))
        {
            Game.OnDuck();
        }
        
        if (Input.GetKeyUp(KeyCode.S))
        {
            Game.OnDuckReleased();
        }
    }

    private void MoveGround()
    {
        if(isGameOver) return;
        float speed = Game.GameSpeed / ground.transform.localScale.x;
        ground.material.mainTextureOffset += new Vector2(speed * Time.deltaTime, 0);
    }

    private void PlayerVisualJump()
    {
        player.transform.position = new Vector3(player.position.x, Game.GetTrexPosY(), player.position.z);
    }
}