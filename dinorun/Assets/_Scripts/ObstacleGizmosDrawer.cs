using System;
using UnityEngine;

namespace _Scripts
{
    public class ObstacleGizmosDrawer : MonoBehaviour
    {
        [SerializeField] private DinoRunner dinoRunner;
        [SerializeField] private ObstacleGenerator.Obstacles obstacleType;

        private void Start()
        {
            dinoRunner = FindObjectOfType<DinoRunner>();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            switch (obstacleType)
            {
                case ObstacleGenerator.Obstacles.LargeCactus:
                    Gizmos.DrawWireCube(transform.position,
                        new Vector3(dinoRunner.config.generatorConfig.largeCactusConfig.width,
                            dinoRunner.config.generatorConfig.largeCactusConfig.height, 1));
                    break;
                case ObstacleGenerator.Obstacles.SmallCactus:
                    Gizmos.DrawWireCube(transform.position,
                        new Vector3(dinoRunner.config.generatorConfig.smallCactusConfig.width,
                            dinoRunner.config.generatorConfig.smallCactusConfig.height, 1));
                    break;
                case ObstacleGenerator.Obstacles.TwoLargeCactus:
                    Gizmos.DrawWireCube(transform.position,
                        new Vector3(dinoRunner.config.generatorConfig.twoLargeCactusConfig.width,
                            dinoRunner.config.generatorConfig.twoLargeCactusConfig.height, 1));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}