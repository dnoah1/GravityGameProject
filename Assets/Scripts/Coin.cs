using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    SpriteRenderer sr;
    public int coinScore;
    public GameObject scoreKeeper;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>(); 
    }
    
    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log(col.gameObject.name + " : " + gameObject.name + " : " + Time.time);
        if (col.CompareTag("coin"))
        {
            
            Debug.Log("collided with coin");
            coinScore++;
            scoreKeeper.GetComponent<Timer>().addScore(coinScore);

        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
}

