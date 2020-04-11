using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GoalCollisionDetector : MonoBehaviour
{

    public GameManager gameManager;
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

        if (gameManager == null)
        {
            audio.PlayOneShot(goalSound);
            Debug.Log("triggerr");
            SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
        }
        else if (gameManager.canWin())
        {
            audio.PlayOneShot(goalSound);
            Debug.Log("trigger!");
            SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
        }
       
    }
}
