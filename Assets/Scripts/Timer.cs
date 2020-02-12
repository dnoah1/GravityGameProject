using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{ 
private string welcomeMsg = "Welcome to Gravity game\n heres what to do..";
private float timeLeft = 60f;
private int score = 0;
private GameObject[] coins;

    // Start is called before the first frame update
    void Start()
    {
    // finds the prefabs for Coin tagged with coins
    coins = GameObject.FindGameObjectsWithTag("coins"); 
    }

    private void OnGUI()
    {
    GUI.Label(new Rect(10, 10, 500, 40), welcomeMsg);
    GUI.Label(new Rect(10, 110, 500, 40), TimeLeft());

    }

    private string ScoreString()
    {
        return "Score" + score;
    }

    private string TimeLeft()
    {
        return "Time: " + (int)timeLeft + "secs";
    }

    // need to get a coinScore script system
    public void addScore (int CoinScore)
    {
        Debug.Log("Coin score" + CoinScore);
        score += CoinScore;
        Debug.Log("score" + score);


    }
    // Update is called once per frame
    void Update()
    {
        checkWin();
        timeLeft -= Time.deltaTime;
    }

    void checkWin()
    {
        bool completed = true;
        foreach (GameObject temp in coins)
        {
            //if (!temp.GetComponent<Coin>().poopedOn)
            //{
            //    completed = false;
            //}

        }
        if (completed)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
