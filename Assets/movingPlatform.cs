using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingPlatform : MonoBehaviour
{
    public float yVelocity = 0.01f;
    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Translate(new Vector3(0.0f, yVelocity));
        if (gameObject.transform.position.y >= 8.0f)
        {
            yVelocity *= -1.0f;
        }
        else if (gameObject.transform.position.y <= 0.0f)
        {
            yVelocity *= -1.0f;
        }
    }
}
