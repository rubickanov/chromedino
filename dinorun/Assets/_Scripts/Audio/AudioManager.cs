using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource jump;
    [SerializeField] private AudioSource dead;
    [SerializeField] private AudioSource score;

    
    private void Start()
    {
        DinoRunner.Instance.Game.OnGameOver += PlayDeadSound;
        DinoRunner.Instance.Game.Trex.OnJump += PlayJumpSound;
        DinoRunner.Instance.Game.Score.OnHundredScore += PlayScoreSound;
    }

    private void PlayScoreSound()
    {
        score.Play();
    }

    private void PlayJumpSound()
    {
        jump.Play();
    }

    private void PlayDeadSound()
    {
        dead.Play();
    }
}
