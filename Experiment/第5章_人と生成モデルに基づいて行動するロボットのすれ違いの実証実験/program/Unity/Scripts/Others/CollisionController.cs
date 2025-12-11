using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionController : MonoBehaviour
{
    [SerializeField]
    GameObject sceneManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Goal")
        {
            sceneManager.GetComponent<SceneController>().GoGoal();
        }
        else if(other.gameObject.tag == "Agent")
        {
            sceneManager.GetComponent<SceneController>().GoCollided();
        }
    }
}
