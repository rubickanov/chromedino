using Rubickanov.Dino.Core;
using UnityEngine;
using Vector2 = Rubickanov.Dino.Core.Vector2;

public class DinoRunner : MonoBehaviour
{
    public Game game;

    [SerializeField] private MeshRenderer ground;
    [SerializeField] private Transform player;

    private void Start()
    {
        game = new Game();
        StartGame();
    }

    private void StartGame()
    {
        game.Start();
    }

    private void Update()
    {
        game.Update(Time.deltaTime);

        ground.material.mainTextureOffset = ConvertVector2(new Vector2(game.GameSpeed, 0));

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space))
        {
            game.Trex.wantToJump = true;
        }
        
        if(Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.Space))
        {
            game.Trex.wantToJump = false;
            game.Trex.jumpReleased = true;
        }
        
        player.transform.position = new Vector3(player.position.x, game.GetTrexPosY(), player.position.z);
        
        Debug.Log(game.Trex.state);
    }


    public UnityEngine.Vector2 ConvertVector2(Vector2 vector2)
    {
        return new UnityEngine.Vector2(vector2.X, vector2.Y);
    }
}