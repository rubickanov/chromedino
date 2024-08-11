using System;

namespace Rubickanov.Dino.Core
{
    public class Trex
    {
        private float currentPosY = 0;
        private float jumpTimer = 0f;
        private TrexConfig config;
        private State state;

        public bool wantToJump;
        public bool jumpReleased;
        public float CurrentPosY => currentPosY;
        public Trex(TrexConfig config)
        {
            this.config = config;
        }

        public void Start()
        {
            state = State.RUNNING;
        }

        public enum State
        {
            //IDLE,
            RUNNING,
            JUMPING,
            FALLING,
            //DEAD
        }

        public void Update(float deltaTime)
        {
            switch (state)
            {
                case State.RUNNING:
                    HandleRunning();
                    break;
                
                case State.JUMPING:
                    HandleJump(deltaTime);
                    break;
                
                case State.FALLING:
                    HandleFalling(deltaTime);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleRunning()
        {
            if (wantToJump && jumpReleased)
            {
                state = State.JUMPING;
                jumpReleased = false;
            }
        }

        private void HandleJump(float deltaTime)
        {
            jumpTimer += deltaTime;

            if (jumpTimer >= config.maxJumpTime || !wantToJump && jumpTimer >= config.minJumpTime)
            {
                state = State.FALLING;
            }

            if (currentPosY <= config.maxPosY)
            {
                currentPosY += config.jumpMultiplier * deltaTime;
            }
        }

        private void HandleFalling(float deltaTime)
        {
            if (currentPosY > config.initPosY)
            {
                currentPosY -= config.gravityMultiplier * deltaTime;
            }

            if (currentPosY <= config.initPosY)
            {
                currentPosY = config.initPosY;
                jumpTimer = 0;

                state = State.RUNNING;
            }
        }
    }
}