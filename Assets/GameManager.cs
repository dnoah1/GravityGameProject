﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
 
public class GameManager : MonoBehaviour
{
    private string welcomeMsg = "Welcome to Gravity game\n heres what to do..";

    public int spacePartsColl;
    public int totalSpaceParts;
    private GUIStyle guiStyle = new GUIStyle();
    private int numLives;
    public static GameManager instance;
    public GUISkin guiSkin;
    //private mainCharacter;


    public Sprite[] heartSprites;
    public Image heart;
    public Text lifeText;

    private void Awake()
    {
        instance = this;
    }

    public bool canWin()
    {
        //Debug.Log(totalSpaceParts);
        //Debug.Log(spacePartsColl);
        Debug.Log(totalSpaceParts == spacePartsColl);
        return (totalSpaceParts == spacePartsColl);
    }

    public void AdjustScore(int num)
    {
        spacePartsColl += num;
    }

    void OnGUI()
    {
        //guiStyle.fontSize = 50;
        //GUI.color = Color.white;
        //guiStyle.normal.textColor = Color.white;
        //GUI.skin = guiSkin ;
        //GUI.Label(new Rect(1150, 690, 0, 0), "Parts Collected: " + spacePartsColl +
        //    "\nParts needed: " + totalSpaceParts, guiStyle);

        lifeText.text = "Parts Collected: " + spacePartsColl +
            "\nParts needed: " + totalSpaceParts;
    }
    // Start is called before the first frame update
    void Start()
    {
        numLives = 5;
        heart.sprite = heartSprites[0];
    }

    // Update is called once per frame
    void Update()
    {
        heart.sprite = heartSprites[numLives];
    }

    public void DecreaseLives()
    {
        numLives--;
    }
}
