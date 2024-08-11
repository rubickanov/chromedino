using System;

namespace Rubickanov.Dino.Core
{
    public class Trex
    {
        private float initPosY;
        public float currentPosY;
        private float maxPosY;

        private float jumpMultiplier = 8f;
        private float gravityMultiplier = 6f;

        public bool wantToJump;
        public bool shouldFall;
        public bool jumpReleased;

        private float maxJumpTime = 0.5f;
        private float minJumpTime = 0.2f;
        public float jumpTimer = 0f;

        public State state;

        public Trex()
        {
            initPosY = 0;
            currentPosY = initPosY;
            maxPosY = 1.5f;
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

            if (jumpTimer >= maxJumpTime || !wantToJump && jumpTimer >= minJumpTime)
            {
                state = State.FALLING;
            }

            if (currentPosY <= maxPosY)
            {
                currentPosY += jumpMultiplier * deltaTime;
            }
        }

        private void HandleFalling(float deltaTime)
        {
            if (currentPosY > initPosY)
            {
                currentPosY -= gravityMultiplier * deltaTime;
            }

            if (currentPosY <= initPosY)
            {
                currentPosY = initPosY;
                jumpTimer = 0;

                state = State.RUNNING;
            }
        }
    }

    public struct Vector2
    {
        public Vector2(float x = 0, float y = 0)
        {
            X = x;
            Y = y;
        }

        public float X;
        public float Y;
    }
}