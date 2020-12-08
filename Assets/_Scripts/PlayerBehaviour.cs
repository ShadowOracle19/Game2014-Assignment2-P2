using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public enum ImpulseSounds
{
    JUMP,
    HIT1,
    DIE,
    SPAWN
}

public class PlayerBehaviour : MonoBehaviour
{
    [Header("Controls")]
    public Joystick joystick;
    public float joystickHorizontalSensitivity;
    public float joystickVerticalSensitivity;
    public float horizontalForce;
    public float verticalForce;

    [Header("Platform Detection")]
    public bool isGrounded;
    public bool isJumping;
    public bool isCrouching;
    public Transform spawnPoint;
    public Transform lookAheadPoint;
    public Transform lookInFrontPoint;
    public LayerMask collisionGroundLayer;
    public LayerMask collisionWallLayer;
    public RampDirection rampDirection;
    public bool onRamp;
    public float rampForceVector;

    [Header("Player Abilities")] 
    public int health;
    public int lives;
    public BarController healthBar;
    public Animator livesHUD;

    

    public Rigidbody2D m_rigidBody2D;
    private SpriteRenderer m_spriteRenderer;
    private Animator m_animator;
    private RaycastHit2D groundHit;

    [Header("Dust Trail")]
    public ParticleSystem dustTrail;
    public Color dustTrailColor;

    [Header("Impulse Sounds")]
    public AudioSource[] sounds;

    [Header("Camera")]
    public CinemachineVirtualCamera vCam1;
    public CinemachineBasicMultiChannelPerlin perlin;
    public float shakeIntensity;
    public float maxShakeTime;
    public float shakeTimer;
    public bool isCameraShaking = false;

    // Start is called before the first frame update
    void Start()
    {
        health = 100;
        lives = 3;

        m_rigidBody2D = GetComponent<Rigidbody2D>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_animator = GetComponent<Animator>();
        dustTrail = GetComponentInChildren<ParticleSystem>();
        
        sounds = GetComponents<AudioSource>();

        
        perlin = vCam1.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        shakeTimer = maxShakeTime;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _LookInFront();
        _LookAhead();
        _Move();

        if(isCameraShaking)
        {
            shakeTimer -= Time.deltaTime;
            if(shakeTimer <= 0.0f)//timed out
            {
                perlin.m_AmplitudeGain = 0.0f;
                shakeTimer = maxShakeTime;
                isCameraShaking = false;
            }
        }
    }

    private void _LookInFront()
    {
        if (!isGrounded)
        {
            rampDirection = RampDirection.NONE;
            return;
        }

        var wallHit = Physics2D.Linecast(transform.position, lookInFrontPoint.position, collisionWallLayer);
        if (wallHit && isOnSlope())
        {
            rampDirection = RampDirection.UP;
        }
        else if (!wallHit && isOnSlope())
        {
            rampDirection = RampDirection.DOWN;
        }

        Debug.DrawLine(transform.position, lookInFrontPoint.position, Color.red);
    }

    private void _LookAhead()
    {
        groundHit = Physics2D.Linecast(transform.position, lookAheadPoint.position, collisionGroundLayer);

        isGrounded = (groundHit) ? true : false;

        Debug.DrawLine(transform.position, lookAheadPoint.position, Color.green);
    }

    private bool isOnSlope()
    {
        if (!isGrounded)
        {
            onRamp = false;
            return false;
        }

        if (groundHit.normal != Vector2.up)
        {
            onRamp = true;
            return true;
        }

        onRamp = false;
        return false;
    }

    void _Move()
    {
        if (isGrounded)
        {
            if (!isJumping && !isCrouching)
            {
                if (joystick.Horizontal > joystickHorizontalSensitivity)
                {
                    // move right
                    m_rigidBody2D.AddForce(Vector2.right * horizontalForce * Time.deltaTime);
                    transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    if (onRamp && rampDirection == RampDirection.UP)
                    {
                        m_rigidBody2D.AddForce(Vector2.up * horizontalForce * rampForceVector * Time.deltaTime);
                    }
                    else if (onRamp && rampDirection == RampDirection.DOWN)
                    {
                        m_rigidBody2D.AddForce(Vector2.down * horizontalForce * rampForceVector * Time.deltaTime);
                    }

                    CreateDustTrail();
                    m_animator.SetInteger("AnimState", (int)PlayerAnimationType.RUN);
                }
                else if (joystick.Horizontal < -joystickHorizontalSensitivity)
                {
                    // move left
                    m_rigidBody2D.AddForce(Vector2.left * horizontalForce * Time.deltaTime);
                    transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                    if (onRamp && rampDirection == RampDirection.UP)
                    {
                        m_rigidBody2D.AddForce(Vector2.up * horizontalForce * rampForceVector * Time.deltaTime);
                    }
                    else if (onRamp && rampDirection == RampDirection.DOWN)
                    {
                        m_rigidBody2D.AddForce(Vector2.down * horizontalForce * rampForceVector * Time.deltaTime);
                    }

                    m_animator.SetInteger("AnimState", (int)PlayerAnimationType.RUN);
                }
                else
                {
                    m_animator.SetInteger("AnimState", (int)PlayerAnimationType.IDLE);
                }
            }

            if ((joystick.Vertical > joystickVerticalSensitivity) && (!isJumping))
            {
                // jump
                m_rigidBody2D.AddForce(Vector2.up * verticalForce);
                m_animator.SetInteger("AnimState", (int) PlayerAnimationType.JUMP);
                isJumping = true;

                sounds[(int)ImpulseSounds.JUMP].Play();
                CreateDustTrail();
            }
            else
            {
                isJumping = false;
            }

            if ((joystick.Vertical < -joystickVerticalSensitivity) && (!isCrouching))
            {
                m_animator.SetInteger("AnimState", (int)PlayerAnimationType.CROUCH);
                isCrouching = true;
            }
            else
            {
                isCrouching = false;
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // respawn
        if (other.gameObject.CompareTag("DeathPlane"))
        {
            LoseLife();
        }

        if (other.gameObject.CompareTag("Bullet"))
        {
            TakeDamage(10);
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            //damage delay
            if(Time.frameCount % 20 == 0)
            {
                TakeDamage(5);

            }
        }
    }

    public void LoseLife()
    {
        lives -= 1;

        sounds[(int)ImpulseSounds.DIE].Play();
        ShakeCamera();
        livesHUD.SetInteger("LivesState", lives);

        if (lives > 0)
        {
            health = 100;
            healthBar.SetValue(health);
            transform.position = spawnPoint.position;
            sounds[(int)ImpulseSounds.SPAWN].Play();
        }
        else
        {
            SceneManager.LoadScene("End");
        }
        
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        healthBar.SetValue(health);

        PlayRandomHitSound();
        ShakeCamera();

        if (health <= 0)
        {
            LoseLife();
        }
    }

    private void CreateDustTrail()
    {
        dustTrail.GetComponent<Renderer>().material.SetColor("_Color", dustTrailColor);
        dustTrail.Play();
    }

    private void PlayRandomHitSound()
    {
        var randomHitSound = UnityEngine.Random.Range(1, 3);
        sounds[1].Play();
    }

    private void ShakeCamera()
    {
        perlin.m_AmplitudeGain = shakeIntensity;
        isCameraShaking = true;
    }
}
