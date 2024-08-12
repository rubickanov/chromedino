# Копия игры Chrome Dino от Google.

**Версия движка 2022.3.39f1**

## Задачи
У меня была задача сделать игру-копию Chrome Dino. Я хотел попробовать сделать архитектуру не типичную для моих проектов. Первую похожую реализацию я увидел на канале [Life.EXE](https://www.youtube.com/watch?v=LgXplbMGR3c&list=PL2XQZYeh2Hh-1p7TqtOJhU-fiBXg_VaNX&index=2).

Я планировал сделать всю логику игры отдельно от Unity и не использовать ничего связанного с Unity.  

## Ядро проекта

Ядро проекта построено на 5 классах:
- `Game`
- `Trex`
- `Obstacle`
- `ObstacleGenerator`
- `Score`

Структура `GameConfig` включающая в себя еще структуры с конфигом для отдельно `Trex`'a, `Generator`'a и всех видов `Obstacle`. И `enum ObstacleType` для разделения всех препятствий (`ObstacleType.SmallCactus`, `ObstacleType.LargeCactus` etc.) 
Архитектура ядра построена таким образом, что Game считается главным классом, который инициализирует другие классы, выполняющие свою задачи. Класс `Game` будет инициализирован в `MonoBehavior` скрипте, который будет связывать проект Unity и ядро. В классах ядра также есть методы Update, куда будет передаваться `deltaTime`.

Я никогда не писал подобного рода код, но идея была в том, что ядро может использоваться также в других движках и GUI фреймворках.

## Реализация [^1]

Класс `DinoRunner` является связкой Unity и ядра, позволяющее управлять сценой и объектами. 



### 1. Движение
Логичным ходом было сделать не двигающегося динозавтра и камеру следующим за ним, а добиться того, чтобы земля под ним двигалась влево вместе с объектами, создавая эффект движения. Признаюсь, решение я подсмотрел на канале 
[Zigurous](https://www.youtube.com/watch?v=UPvW8kYqxZk&t=2240s). Земля это единственный объект, отрисованный не `SpriteRender`'ом, а `MeshRenderer`'ом. Сам спрайт земли является текстурой одного полигона. Чтобы добиться движения мы используем поле `mainTextureOffset` текстуры материала того самого полигона. 
```
float speed = Game.GameSpeed / ground.transform.localScale.x;
ground.material.mainTextureOffset += new Vector2(speed * Time.deltaTime, 0);
```
Сама логика скорости игры реализована в классе `Game`.
```
public void Update(float deltaTime)
{
    if (!isGameRunning) return;          
    GameSpeed += config.gameSpeedMultiplier * deltaTime;

    // Остальной код
}
```
Реализация движения объектов реализована в классе `ObstacleGenerator`, немного нарушаем правило **Single Responsibility**, но правила созданы, чтобы их нарушать. Каждому инициализированному `Obstacle` мы меняем свойство `PosX`, также используя `deltaTime` и `GameSpeed`. 
```
public void Update(float deltaTime)
{
  // Остальной код

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
### 2. Прыжок
Прыжок реализован в классе `Trex`. `DinoRunner` слушает ввод пользователя (W или Space) и в случае нажатия мы ставим флаг `wantToJump` класса `Trex` в значение `true`, что в свою очередь вызовет прыжок. Сам прыжок это плавное изменение координаты Y. Если пользователь отпустит кнопку, то `Trex` перейдет в состояние `FALLING`, что по сути такое же плавное изменение позиции, только с другим модификатором скорости, заданное в `TrexConfig`. Оба метода вызываются в `Update`. В классе `Trex` реализована простенькая `StateMachine`.

**Метод прыжка:**
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
**Метод падения:**
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

### 3. Генерация препятствий
За это отвечает класс `ObstacleGenerator`. Сама генерация происходит путем рандомного таймера. Высчитывается рандомное чисто между двумя границами минимального и максимального значения, через это время создается объект. Также этот таймер немного модифицируем полем `timeDecreaseFactor` и `GameSpeed`, чтобы объекты создавались быстрее по мере ускорения игры, чтобы на большой скорости не было 'пустынь', но все равно гарантируем себе, чтобы объекты создавались не быстрее, чем раз в 0.75 секунд.

**Метод генерации**
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
    // Остальной код
    }
}
```

**Метод подсчета рандомного таймера:**
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

### 4. Столкновения
Проверка столкновений также не использует никаких `Collider`'ов Unity. Дино, как и все препятствия в игре имеют поля `height` и `width`. Зная позиции, ширину и высотку объектов мы можем проверять их на столкновения. Метод реализован в классе `Game`. Снова проблема **Single Responsobility**.

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
Метод кажется громоздким из-за огромного условия оператора `If`.

### 5. Рекорд
Самый просто класс `Score`, который просто увеличивает поле score по мере игры. 
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

### 6. Скорость
Скоростью игры управляет класс `Game`. В ChromeDino по моим ощущением реализовано не плавное увелечение скорости, а резкий скачок в скорости, у игры также по ощущениям есть максимальная скорость. В моём проекте я это поменял и сделал плавное увелечением без максимально возможной скорости. 
```
public void Update(float deltaTime)
{
    if (!isGameRunning) return;
    
    GameSpeed += config.gameSpeedMultiplier * deltaTime;
    
    // Остальной код
}
```

### 7. Рестарт
Халтура. Рестарт я пытался реализовать без `SceneManager.LoadScene(0)`, но так как я столкнулся со сложностями и скорее всего из-за моего кода, я больше уделил внимания другим аспектам игры. Также из-за этого пострадал `Score`, так как он удаляется после рестарта, но я сохраняю лучший результат используя `PlayerPrefs`. **Постараюсь решить эту проблему позже.**

## Внутри Unity

Я также написал несколько `MonoBehavior` скриптов, для того чтобы слушать объекты ядра и обновлять сцену. 

Два скрипта `DinoAnimatedSprit` и `AnimatedSprite` также были "вдохновлены" 👹 решением с канала [Zigurous](https://www.youtube.com/watch?v=UPvW8kYqxZk&t=2240s). Позволяют создать анимацию используя два или несколько спрайтов, также анимация ускоряется вместе с `GameSpeed`.

`AudioManager` слушает ивенты `Trex` и `Game`, проигрывает 3 звука: прыжок, достижение рекорда кратное 100 и Game Over.

`ObstacleGizmosDrawer` для того, чтобы рисовать `Gizmos` на префабах препятствий используя поля `Config`'а.

`ScoreUI` обновляет текст с рекордом в правом верхнем углу экрана. ~Также сохраняет лучший результат в PlayerPrefs.~ 

`StartGameAnim` для проигрывания анимации в начале игры.

## Что можно улучшить

- [ ] Single Responsobility
- [ ] Логику прыжков
- [ ] Логику проверки столкновений
- [ ] Добавить рестарт, не используя SceneManager.LoadScene()

[^1]: **Так как проект не слишком сложный, мне кажется, будет легче рассмотреть как я реализовал каждый аспект игры, а не разбирать каждый класс отдельно. Это НЕ техническая документация, а мой рассказ о том, как я сделал этот проект. Сами скрипты небольшие и простые для понимания**
