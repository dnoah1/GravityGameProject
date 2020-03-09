using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController2D : MonoBehaviour
{

    public float startingPos;
    public float endingPos;
    public float speed;
    SpriteRenderer spriteRenderer;
    public bool canMove = true;
    Rigidbody2D rb2d = null;
    public Sprite[] movingFrames;
    public Sprite[] idleFrames;

    int currentFrame = 0;
    float animationTimer = 0;
    public float animationFPS = 5;



    private bool currGoingRight = true;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb2d = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        if (currGoingRight)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);
        }


        if (transform.position.x >= endingPos)
        {
            currGoingRight = false;
            spriteRenderer.flipX = true;
        }

        if(transform.position.x < startingPos)
        {
            currGoingRight = true;
            spriteRenderer.flipX = false;

        }

        PlayBackAnimation(movingFrames);
    }


    void PlayBackAnimation(Sprite[] anim)
    {
        animationTimer -= Time.deltaTime;
        if (animationTimer <= 0 && anim.Length > 0)
        {
            animationTimer = 1f / animationFPS;
            currentFrame++;
            if (currentFrame >= anim.Length)
            {
                currentFrame = 0;
            }
            spriteRenderer.sprite = anim[currentFrame];
        }
    }
}
