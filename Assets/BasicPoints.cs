using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPoints : MonoBehaviour
{
    public Game_Manager manager;
    public AudioSource audio;

    bool doOnce = true;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.tag == "Player")
        {
            
            if (doOnce)
            {
                playAudio();
                manager.addScore(1);
                doOnce = false;
                Destroy(gameObject);

            }
        }
    }
    public void playAudio()
    {
        audio.transform.SetParent(null);
        audio.Play();
    }
}
