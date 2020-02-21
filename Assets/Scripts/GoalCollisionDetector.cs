using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalCollisionDetector : MonoBehaviour
{

    public string nextSceneName;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("trigger!");
        Application.LoadLevel(nextSceneName);
    }
}
