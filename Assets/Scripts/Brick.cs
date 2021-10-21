using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{

    public int health;

    bool randomColorSkin;
    public SpriteRenderer spriteRenderer;

    public List<Sprite> cracks;

    void Start()
    {
        if (randomColorSkin)
        {
            Color col = LevelManager.data.brickColors[Random.Range(0, LevelManager.data.brickColors.Length)];
            spriteRenderer.color = col;
        }
    }

    /*void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Ball")
            health--;
    }*/
}
