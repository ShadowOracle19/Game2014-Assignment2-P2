using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlitchHeartBehaviour : MonoBehaviour
{
    public Game_Manager manager;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            manager.CollectedHeart();
            Destroy(gameObject);
        }
    }
}
