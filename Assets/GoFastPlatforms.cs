using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoFastPlatforms : MonoBehaviour
{
    public PlayerBehaviour playerBehaviour;
    public float verticalForce = 1500;
    private void OnCollisionEnter(Collision collision)
    {
        
    }
    private void OnCollisionExit(Collision collision)
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        playerBehaviour.m_rigidBody2D.AddForce(Vector2.right * verticalForce);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        //playerBehaviour.horizontalForce /= 2.0f;
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        playerBehaviour.m_rigidBody2D.AddForce(Vector2.right * verticalForce);
    }
}
