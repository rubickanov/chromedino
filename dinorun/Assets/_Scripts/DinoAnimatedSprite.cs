using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DinoAnimatedSprite : MonoBehaviour
{
    public Sprite idle;
    public Sprite[] runSprites;
    public Sprite[] dinoDuck;
    public Sprite jump;
    public Sprite dead;
    
    private SpriteRenderer spriteRenderer;
    private int frame;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        DinoRunner.Instance.Game.Trex.OnDead += PlayDead;
        DinoRunner.Instance.Game.Trex.OnJump += PlayJump;
        DinoRunner.Instance.Game.Trex.OnRun += PlayRun;
        DinoRunner.Instance.Game.Trex.OnDuck += PlayDuck;
        
        PlayIdle();
    }

    public void PlayDuck()
    {
        CancelInvoke(nameof(AnimateRun));
        Invoke(nameof(AnimateDuck), 0f);
    }

    private void PlayIdle()
    {
        spriteRenderer.sprite = idle;
    }
    
    private void PlayJump()
    {
        CancelInvoke();
        spriteRenderer.sprite = jump;
    }
    
    private void PlayDead()
    {
        CancelInvoke();
        spriteRenderer.sprite = dead;
    }

    private void PlayRun()
    {
        CancelInvoke(nameof(AnimateDuck));
        Invoke(nameof(AnimateRun), 0f);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void AnimateRun()
    {
        frame++;

        if (frame >= runSprites.Length) {
            frame = 0;
        }

        if (frame >= 0 && frame < runSprites.Length) {
            spriteRenderer.sprite = runSprites[frame];
        }
        
        Invoke(nameof(AnimateRun), 1f / DinoRunner.Instance.Game.GameSpeed);
    }
    
    private void AnimateDuck()
    {
        frame++;

        if (frame >= dinoDuck.Length) {
            frame = 0;
        }

        if (frame >= 0 && frame < dinoDuck.Length) {
            spriteRenderer.sprite = dinoDuck[frame];
        }
        
        Invoke(nameof(AnimateDuck), 1f / DinoRunner.Instance.Game.GameSpeed);
    }

}