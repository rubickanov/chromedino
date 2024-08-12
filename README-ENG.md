# Chrome Dino Game Clone from Google

[![ru](https://img.shields.io/badge/lang-ru-red.svg)](https://github.com/rubickanov/chromedino/blob/main/README.md)

*This doc was translated by GPT-4o*.

**Engine Version 2022.3.39f1**

## Tasks
I had a task to create a game clone of Chrome Dino. I wanted to experiment with an architecture that is not typical for my projects. The first similar implementation I saw was on the [Life.EXE](https://www.youtube.com/watch?v=LgXplbMGR3c&list=PL2XQZYeh2Hh-1p7TqtOJhU-fiBXg_VaNX&index=2) channel.

I planned to implement the entire game logic separately from Unity and not use anything related to Unity.

## Project Core

The project core is built on 5 classes:
- `Game`
- `Trex`
- `Obstacle`
- `ObstacleGenerator`
- `Score`

The `GameConfig` structure includes other configurations for `Trex`, `Generator`, and all types of `Obstacle`. There's also an `enum ObstacleType` for categorizing all obstacles (`ObstacleType.SmallCactus`, `ObstacleType.LargeCactus`, etc.).
The core architecture is designed such that `Game` is considered the main class that initializes other classes to perform their tasks. The `Game` class is initialized in a `MonoBehavior` script, which links the Unity project and the core. The core classes also have `Update` methods, where `deltaTime` is passed.

I've never written this kind of code before, but the idea was that the core could also be used in other engines and GUI frameworks.

## Implementation [^1]

The `DinoRunner` class serves as the link between Unity and the core, allowing you to control the scene and objects.

### 1. Movement
A logical approach was not to have a moving dinosaur and a camera following it, but to make the ground beneath it move to the left along with the objects, creating a sense of movement. I admit, I borrowed this idea from the [Zigurous](https://www.youtube.com/watch?v=UPvW8kYqxZk&t=2240s) channel. The ground is the only object not rendered by `SpriteRender` but by `MeshRenderer`. The ground sprite is a texture on a single polygon. To achieve movement, we use the `mainTextureOffset` field of the polygon's material texture.
```
float speed = Game.GameSpeed / ground.transform.localScale.x;
ground.material.mainTextureOffset += new Vector2(speed * Time.deltaTime, 0);
```
The game speed logic is implemented in the `Game` class.
```
public void Update(float deltaTime)
{
    if (!isGameRunning) return;          
    GameSpeed += config.gameSpeedMultiplier * deltaTime;

    // Remaining code
}
```
The logic for moving objects is implemented in the `ObstacleGenerator` class, slightly violating the **Single Responsibility** principle, but rules are meant to be broken. For each initialized `Obstacle`, we change the `PosX` property, also using `deltaTime` and `GameSpeed`.
```
public void Update(float deltaTime)
{
  // Remaining code

  for (int i = CurrentObstacles.Count - 1; i >= 0; i--)
  {
      CurrentObstacles[i].PosX -= game.GameSpeed * deltaTime;
      if (CurrentObstacles[i].PosX < config.despawnPosX)
      {
          CurrentObstacles[i].Destroy();
          CurrentObstacles.RemoveAt(i);
    }
  }
}
```

### 2. Jumping
Jumping is implemented in the `Trex` class. `DinoRunner` listens for user input (W or Space) and if pressed, it sets the `wantToJump` flag of the `Trex` class to `true`, which triggers the jump. The jump itself is a smooth change in the Y coordinate. If the user releases the button, the `Trex` transitions to the `FALLING` state, which is essentially the same smooth position change but with a different speed modifier, specified in `TrexConfig`. Both methods are called in `Update`. The `Trex` class also implements a simple `StateMachine`.

**Jump Method:**
```
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
```

**Falling Method:**
```
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
```

### 3. Obstacle Generation
This is handled by the `ObstacleGenerator` class. The generation itself occurs through a random timer. A random number is calculated between two bounds, and after this time, an object is created. This timer is also slightly modified by the `timeDecreaseFactor` field and `GameSpeed` to make objects appear faster as the game speeds up, ensuring that no 'deserts' form at high speeds. However, we still guarantee that objects are not generated more frequently than once every 0.75 seconds.

**Generation Method**
```
public void Update(float deltaTime)
{
    timeSinceLastObstacle += deltaTime;

    if (timeSinceLastObstacle >= timeToNextObstacle)
    {
        GenerateObstacle();
        timeSinceLastObstacle = 0f;
        timeToNextObstacle = GetRandomTimeToNextObstacle();
    }
    // Remaining code
    }
}
```

**Random Timer Calculation Method:**
```
private float GetRandomTimeToNextObstacle()
{
    float time = (float)random.NextDouble() * (config.maxTimeBetweenObstacles - config.minTimeBetweenObstacles) +
               config.minTimeBetweenObstacles;

    time -= game.GameSpeed * config.timeDecreaseFactor;

    time = Math.Max(time, 0.75f);

    return time;
}
```

### 4. Collisions
Collision detection also does not use any Unity `Colliders`. The Dino, like all obstacles in the game, has `height` and `width` properties. Knowing the positions, widths, and heights of the objects, we can check them for collisions. The method is implemented in the `Game` class, once again challenging **Single Responsibility**.

```
public bool CheckCollision()
{
    foreach (var Obstacle in ObstacleGenerator.CurrentObstacles)
    {
      if (Obstacle.PosX <= config.trexConfig.initPosX + Trex.width &&
          Obstacle.PosX + Obstacle.config.width >= config.trexConfig.initPosX &&
          Trex.CurrentPosY <= Obstacle.config.height + Obstacle.PosY &&
          Trex.CurrentPosY + Trex.height >= Obstacle.PosY)
          {
              return true;
          }
    }

    return false;
}
```
The method might seem bulky due to the large `If` condition.

### 5. Score
The simplest class is `Score`, which simply increments the `score` field as the game progresses.
```
public void Update(float deltaTime, float gameSpeed)
{
    score += config.scoreMultiplier * gameSpeed * deltaTime;
    OnScoreChanged?.Invoke(score);

    float currentScoreEvent = Mathf.Floor(score / 100);
    if (currentScoreEvent > lastScoreEvent)
    {
        OnHundredScore?.Invoke();
        lastScoreEvent = currentScoreEvent;
    }
}
```

### 6. Speed
The game speed is controlled by the `Game` class. In Chrome Dino, it seems that the speed increases in sudden jumps rather than smoothly, and the game seems to have a maximum speed. In my project, I changed this to a smooth increase without a maximum speed limit.
```
public void Update(float deltaTime)
{
    if (!isGameRunning) return;
    
    GameSpeed += config.gameSpeedMultiplier * deltaTime;
    
    // Remaining code
}
```

### 7. Restart
A bit of a hack job. I tried to implement a restart without using `SceneManager.LoadScene(0)`, but as I encountered difficulties (probably due to my code), I focused more on other aspects of the game. As a result, the `Score` also suffers as it resets after restarting, but I save the best result using `PlayerPrefs`. **I plan to resolve this issue later.**

## Inside Unity

I also wrote several `MonoBehavior` scripts to listen to core objects and update the scene.

Two scripts, `DinoAnimatedSprite` and `AnimatedSprite`, were also "inspired" 👹 by the solution from the [Zigurous](https://www.youtube.com/watch?v=UPvW8kYqxZk&t=2240s) channel. They allow creating animations using two or more sprites, and the animation speed increases along with `GameSpeed`.

`AudioManager` listens to events from `Trex` and `Game`, playing 3 sounds: jump, score milestone (multiples of 100), and Game Over.

`ObstacleGizmosDrawer` is used to draw `Gizmos` on obstacle prefabs using `Config` fields.

`ScoreUI` updates the score text in the upper right corner of the screen. ~It also saves the best score in PlayerPrefs.~



`StartGameAnim` is for playing the start game animation.

## What Can Be Improved

- [ ] Single Responsibility
- [ ] Jump logic
- [ ] Collision detection logic
- [ ] Add restart without using SceneManager.LoadScene()

[^1]: **Since the project is not too complex, I think it would be easier to review how I implemented each aspect of the game rather than dissecting each class separately. This is NOT a technical document, but my story of how I created this project. The scripts themselves are small and easy to understand.**
