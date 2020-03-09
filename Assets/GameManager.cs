using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private string welcomeMsg = "Welcome to Gravity game\n heres what to do..";

    private int spacePartsColl;

    public static GameManager instance;

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
        GUI.Label(new Rect(10, 110, 500, 40), "Parts Collected = " + spacePartsColl);

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
