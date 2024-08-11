using System;

namespace Rubickanov.Dino.Core
{
    public class Trex
    {
        public float height;
        public float width;

        private float currentPosY = 0;
        private float jumpTimer = 0f;
        private TrexConfig config;
        private State state = State.IDLE;
        private State lastState;

        public bool wantToJump;
        public bool jumpReleased;
        public bool wantToDuck;
        public float CurrentPosY => currentPosY;

        public event Action OnDuck;

        public event Action OnJump;
        public event Action OnDead;
        public event Action OnRun;

        public Trex(TrexConfig config)
        {
            this.config = config;
            height = config.height;
            width = config.width;
        }

        public void Start()
        {
            state = State.RUNNING;
        }

        public enum State
        {
            IDLE,
            RUNNING,
            DUCK,
            JUMPING,
            FALLING,
            DEAD
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

                case State.DEAD:
                    break;

                case State.IDLE:
                    break;

                case State.DUCK:
                    HandleDuck();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleDuck()
        {
            if (lastState != State.DUCK)
            {
                OnDuck?.Invoke();
                lastState = State.DUCK;
                height = config.duckHeight;
            }

            if (wantToJump && jumpReleased)
            {
                state = State.JUMPING;
                jumpReleased = false;
                height = config.height;
            }

            if (!wantToDuck)
            {
                state = State.RUNNING;
                height = config.height;
            }
        }

        private void HandleRunning()
        {
            if (lastState != State.RUNNING)
            {
                OnRun?.Invoke();
                lastState = State.RUNNING;
            }

            if (wantToJump && jumpReleased)
            {
                state = State.JUMPING;
                jumpReleased = false;
            }

            if (wantToDuck)
            {
                state = State.DUCK;
            }
        }

        private void HandleJump(float deltaTime)
        {
            if (lastState != State.JUMPING)
            {
                OnJump?.Invoke();
                lastState = State.JUMPING;
            }

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

        public void Die()
        {
            state = State.DEAD;
            OnDead?.Invoke();
        }
    }
}