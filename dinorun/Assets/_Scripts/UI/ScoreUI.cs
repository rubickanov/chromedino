using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoreText;
    private float bestScore;
    
    private void Start()
    {
        DinoRunner.Instance.Game.Score.OnScoreChanged += UpdateScore;
        DinoRunner.Instance.Game.Score.OnHighScoreChanged += UpdateBestScore;

        bestScore = PlayerPrefs.GetFloat("BestScore");
        
        UpdateScore(0);
    }

    private void UpdateBestScore(float bestScore)
    {
        this.bestScore = bestScore;
    }

    private void UpdateScore(float score)
    {
        scoreText.text = $"HI {score:00000} {bestScore:00000}";
    }
    
    private void OnDestroy()
    {
        PlayerPrefs.SetFloat("BestScore", bestScore);
        PlayerPrefs.Save();
    }
}
