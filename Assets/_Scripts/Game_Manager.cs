using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Game_Manager : MonoBehaviour
{
    public int remaining = 5;
    public TextMeshProUGUI heartText;
    public void CollectedHeart()
    {
        remaining -= 1;
    }
    void Update()
    {
        heartText.SetText(remaining.ToString());
    }
}
