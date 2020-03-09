using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalCollisionDetector : MonoBehaviour
{


    public AudioClip goalSound;
    AudioSource audio;

    public string nextSceneName;
    // Start is called before the first frame update
    void Start()
    {
        audio = gameObject.AddComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        audio.PlayOneShot(goalSound);
        Debug.Log("trigger!");
        Application.LoadLevel(nextSceneName);
    }
}
