using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrongBrick : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public int totalHealth = 4;
    int health;
    public List<Texture> cracks;

    // Start is called before the first frame update
    void Start()
    {
        //If spriteRenderer is not assignet find it
        if (!spriteRenderer)
            spriteRenderer = GetComponent<SpriteRenderer>();

        //Set health
        health = totalHealth;
    }

    public void GetDamage()
    {
        health--;

        //Change crack texture
        switch (health)
        {
            case 3:
                spriteRenderer.material.SetTexture("_Crack", cracks[0]);
            break;

            case 2:
                spriteRenderer.material.SetTexture("_Crack", cracks[1]);
            break;
            
            case 1:
                spriteRenderer.material.SetTexture("_Crack", cracks[2]);
            break;
        }

        //Check if life is below or equal with 0
        if(health <= 0)
        {
            EventManager.addScoreEvent.Invoke(this.gameObject);
        }
    }
}
