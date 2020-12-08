using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Game_Manager : MonoBehaviour
{
    public int remaining = 5;
    public int scoreText = 0;
    public TextMeshProUGUI heartText;
    public TextMeshProUGUI score;
    public void CollectedHeart()
    {
        remaining -= 1;
    }
    public void addScore(int score)
    {
        scoreText += score;
    }
    void Update()
    {
        heartText.SetText(remaining.ToString());
        score.SetText(scoreText.ToString());
    }
}
