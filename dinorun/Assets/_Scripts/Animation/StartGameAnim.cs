using System;
using UnityEngine;

public class StartGameAnim : MonoBehaviour
{
    [SerializeField] private Animator anim;

    public event Action OnAnimEnd;
    public void Play()
    {
        anim.Play("StartGame");
    }

    public void OnAnimationEnd()
    {
        OnAnimEnd?.Invoke();
    }
}