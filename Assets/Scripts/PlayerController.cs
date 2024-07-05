using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static bool magnetActive;

    static Collider2D coll;
    
    [SerializeField]
    float speed = 50;
    float xSize;
    public SpriteRenderer spriteRenderer;

    public Vector2 bounds;
    float targetXPosition;

    private void Start()
    {
        //Set speed
        speed = LevelManager.data.paddleSpeed;

        // Set selected paddle skin
        if(!spriteRenderer)
            spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = LevelManager.data.paddles[LevelManager.data.selectedPaddle].sprite;

        //Get screen bounds
        bounds = GameManager.GetBounds();
        

        //Get collider component
        coll = GetComponent<Collider2D>();
        

        //Get collider bounds
        xSize = coll.bounds.size.x / 2 + 0.5f;
    }

    void Update()
    {
        float h = 0;
        Vector3 pos = transform.position;

        //Get horizontal input
        switch (LevelManager.data.inputType) {
            case GameData.InputType.Touch:

                break;
            case GameData.InputType.Keyboard:
                if (Input.GetAxisRaw("Horizontal") < 0)
                    h = -1;
                else if (Input.GetAxisRaw("Horizontal") > 0)
                    h = 1;
                break;
            case GameData.InputType.Mouse:
                
                break;
            default:
                break;
        }

        if (LevelManager.data.inputType == GameData.InputType.Touch)
        {
            /*float dir = Input.acceleration.x;
            if (dir < -0.1f)
                h = -1;
            else if (dir > 0.1f)
                h = 1;

            Debug.Log("<color=yellow>" + dir + "</color>");*/
        }
        else
        {
            targetXPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
            pos.x = Mathf.Lerp(pos.x, targetXPosition, speed * Time.deltaTime);
        }


        //Move paddle
        pos.x += h * speed * Time.deltaTime;
        
        //Clamp position
        if (pos.x > bounds.x - xSize)
        {
            pos.x = bounds.x - xSize;
        }
        else if(pos.x < -bounds.x + xSize)
        {
            pos.x = -bounds.x + xSize;
        }

        //Set position
        transform.position = pos;
    }
}
