namespace Rubickanov.Dino.Core
{
    public class Game
    {
        public Trex Trex;

        private float gameSpeed = 1f;
        private float gameSpeedMultiplier = 0.2f;
        public float GameSpeed => gameSpeed;

        public Game()
        {
            Trex = new Trex();
        }

        public void Start()
        {
            Trex.Start();
        }

        public void Update(float deltaTime)
        {
            gameSpeed += deltaTime * gameSpeedMultiplier;

            Trex.Update(deltaTime);
        }

        public float GetTrexPosY()
        {
            return Trex.currentPosY;
        }
    }
}