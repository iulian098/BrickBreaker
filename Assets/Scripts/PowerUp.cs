using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public PowerUpType _type;

    public Sprite[] sprites;
    public SpriteRenderer spriteRenderer;
    private void Start()
    {
        if(!spriteRenderer)
            spriteRenderer = GetComponent<SpriteRenderer>();

        int rand = Random.Range(0, sprites.Length);
        _type = (PowerUpType)rand;
        spriteRenderer.sprite = sprites[rand];

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            EventManager.powerPickupEvent.Invoke(_type);
            Destroy(gameObject);
        }
    }
}
