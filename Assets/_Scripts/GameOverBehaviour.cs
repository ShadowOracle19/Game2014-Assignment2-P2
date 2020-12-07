/**
 * File Name: GameOverBehaviour.cs
 * Authors name: Lucas Coates
 * Student Number: 101172500
 * Date Last Modifier: 11/19/2020
 * Program Description: Game over button behaviour
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverBehaviour : MonoBehaviour
{
    public AudioSource audio;
    public void onButtonClick()
    {
        audio.Play();
        SceneManager.LoadScene("GameOver");
    }
}
