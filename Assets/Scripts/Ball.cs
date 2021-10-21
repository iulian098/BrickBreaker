using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public bool started;
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;
    public TrailRenderer trail;
    float speed;
    

    [SerializeField]
    Transform paddle;
    public bool sticked;

    void Start()
    {
        //Get rigidbody component
        if(!rb)
            rb = GetComponent <Rigidbody2D>();
        //Get spriteRenderer component
        if(!spriteRenderer)
            spriteRenderer = GetComponent<SpriteRenderer>();

        //Find player paddle
        paddle = LevelManager.paddle.transform;

        //Set ball skin
        spriteRenderer.sprite = LevelManager.data.balls[LevelManager.data.selectedBall].sprite;
        if (LevelManager.data.balls[LevelManager.data.selectedBall].material)
            spriteRenderer.material = LevelManager.data.balls[LevelManager.data.selectedBall].material;
        else
            spriteRenderer.material = LevelManager.data.defaultBallMaterial;

    }

    private void Update()
    {

        if (trail.startWidth != transform.localScale.x)
            trail.startWidth = transform.localScale.x;

        //Start moving ball
        if (Input.GetButtonDown("Jump") && (!started || sticked))
        {
            StartBall();
        }

        if (started)
        {
            //Keep constant speed
            rb.velocity = speed * rb.velocity.normalized;
        }
        else if(!started)
        {
            //Follow the paddle
            transform.position = paddle.position + new Vector3(0, 1.5f, 0);
        }

        //Check velocity
        CheckXYSpeed();
    }

    void CheckXYSpeed()
    {
        //Check y velocity
        if (rb.velocity.y >= -0.025f && rb.velocity.y <= 0.025f)
        {
            if (transform.position.y >= 0)
                rb.velocity = rb.velocity + new Vector2(0, 2.5f);
            else
                rb.velocity = rb.velocity - new Vector2(0, 2.5f);
        }

        //Check x velocity
        if (rb.velocity.x >= -0.025f && rb.velocity.x <= 0.025f)
        {
            if (transform.position.x <= LevelManager.instance.leftWall.transform.position.x + 1)
                rb.velocity = rb.velocity + new Vector2(2.5f, 0);
            else if(transform.position.x >= LevelManager.instance.rightWall.transform.position.x - 1)
                rb.velocity = rb.velocity - new Vector2(2.5f, 0);
        }
    }
    public void StartNewBall(Ball oldBall, Vector2 direction)
    {
        float randomX = Random.Range(-1f, 1f);
        sticked = false;
        started = true;

        //Remove from sticked balls list
        if (LevelManager.stickedBalls.Contains(this))
            LevelManager.stickedBalls.Remove(this);

        //Enable rigidbody simulation
        if (!rb.simulated)
            rb.simulated = true;

        //Need to change
        rb.velocity = rb.velocity + new Vector2(randomX, 0);
    }
    public void StartBall()
    {

        //Remove from sticked balls list
        if (LevelManager.stickedBalls.Contains(this))
            LevelManager.stickedBalls.Remove(this);

        //Set transform parent to null
        transform.SetParent(null);

        //Get the rigidbody component if it's not assigned
        if (!rb)
            rb = GetComponent<Rigidbody2D>();

        //Enable rigidbody simulation
        if (!rb.simulated)
            rb.simulated = true;



        if (!started || (started && sticked))
        {
            //Move ball up
            rb.velocity = Vector2.up * speed;
        }
        /*else if (started && sticked)
        {

            //Move ball up
            rb.velocity = Vector2.up * speed;
        }*/

        sticked = false;
        started = true;

    }

    public void UnstickBall()
    {
        rb.simulated = true;
        transform.SetParent(null);
        rb.velocity = Vector2.up * speed;
    }

    public void StopBall()
    {
        rb.velocity = Vector2.zero;
        rb.simulated = false;
    }

    public void SetBallSpeed(float ballSpeed)
    {
        speed = ballSpeed;
    }

    Vector3 randomDirection()
    {
        float xVal = Random.Range(-0.5f, 0.5f);

        //Calculate direction
        Vector3 dir = new Vector3(xVal, 1 - Mathf.Abs(xVal), 0);
        return dir;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Destroy brick when hit
        if(collision.gameObject.tag == "Brick")
        {
            EventManager.addScoreEvent.Invoke(collision.gameObject);

        }
        else if(collision.gameObject.tag == "ExplodingBrick")
        {
            EventManager.explodeEvent.Invoke(collision.gameObject);
            EventManager.addScoreEvent.Invoke(collision.gameObject);
        }
        else if(collision.gameObject.tag == "StrongBrick")
        {
            EventManager.brickDamageEvent.Invoke(collision.gameObject.GetComponent<StrongBrick>());
        }

        //The game is over, too bad for you
        else if(collision.gameObject.tag == "GameOver")
        {

            LevelManager.instance.balls.Remove(this);
            Destroy(gameObject);
            EventManager.gameOverEvent.Invoke();

        }
        //Invoke stickToPaddle event
        else if(collision.gameObject.tag == "Player")
        {

            EventManager.stickToPaddleEvent.Invoke(gameObject);

        }

        //Invode ballHitEvent event
        EventManager.ballHitEvent.Invoke(collision.GetContact(0).point);

        
    }
}
