using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController2D : MonoBehaviour
{

    public float startingPos;
    public float endingPos;
    public float speed;
    SpriteRenderer spriteRenderer;

    private bool currGoingRight = true;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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
    }
}
