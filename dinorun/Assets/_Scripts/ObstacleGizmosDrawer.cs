using System;
using Rubickanov.Dino.Core;
using UnityEngine;

public class ObstacleGizmosDrawer : MonoBehaviour
{
    [SerializeField] private DinoRunner dinoRunner;
    [SerializeField] private ObstacleType obstacleType;

    private void Start()
    {
        dinoRunner = FindObjectOfType<DinoRunner>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        switch (obstacleType)
        {
            case ObstacleType.LargeCactus:
                Gizmos.DrawWireCube(transform.position,
                    new Vector3(dinoRunner.config.generatorConfig.largeCactusConfig.width,
                        dinoRunner.config.generatorConfig.largeCactusConfig.height, 1));
                break;

            case ObstacleType.SmallCactus:
                Gizmos.DrawWireCube(transform.position,
                    new Vector3(dinoRunner.config.generatorConfig.smallCactusConfig.width,
                        dinoRunner.config.generatorConfig.smallCactusConfig.height, 1));
                break;

            case ObstacleType.TwoLargeCactus:
                Gizmos.DrawWireCube(transform.position,
                    new Vector3(dinoRunner.config.generatorConfig.twoLargeCactusConfig.width,
                        dinoRunner.config.generatorConfig.twoLargeCactusConfig.height, 1));
                break;

            case ObstacleType.TwoSmallCactus:
                Gizmos.DrawWireCube(transform.position,
                    new Vector3(dinoRunner.config.generatorConfig.twoSmallCactusConfig.width,
                        dinoRunner.config.generatorConfig.twoSmallCactusConfig.height, 1));
                break;

            case ObstacleType.ThreeSmallCactus:
                Gizmos.DrawWireCube(transform.position,
                    new Vector3(dinoRunner.config.generatorConfig.threeSmallCactusConfig.width,
                        dinoRunner.config.generatorConfig.threeSmallCactusConfig.height, 1));
                break;

            case ObstacleType.MixCactus:
                Gizmos.DrawWireCube(transform.position,
                    new Vector3(dinoRunner.config.generatorConfig.mixCactusConfig.width,
                        dinoRunner.config.generatorConfig.mixCactusConfig.height, 1));
                break;

            case ObstacleType.Bird:
                Gizmos.DrawWireCube(transform.position,
                    new Vector3(dinoRunner.config.generatorConfig.birdConfig.width,
                        dinoRunner.config.generatorConfig.birdConfig.height, 1));
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}