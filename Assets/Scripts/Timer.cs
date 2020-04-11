using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{ 
    private string welcomeMsg = "Welcome to Gravity game\n heres what to do..";

    public int spacePartsColl;

    public static Timer instance;

    private void Awake()
    {
        instance = this;
    }

    public void AdjustScore(int num)
    {
        spacePartsColl += num;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 100), welcomeMsg);
        GUI.Label(new Rect(10, 110, 500, 40),"Parts Collected = " + spacePartsColl);

    }

    void Start()
    {
        GameObject.FindGameObjectsWithTag("coin"); 
    }

    void Update()
    { 
      
    }

}
