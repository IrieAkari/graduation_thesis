using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAgentCondition : MonoBehaviour
{

    public GameObject Robot;
    public float Xr = -25;
    public float Zr = 0;

    public GameObject Player;
    public float Xp = 25;
    public float Zp = 0;

    public int ModelCodition;


    // Start is called before the first frame update
    void Start()
    {
        // Initializing the selected conditions.
        Robot.GetComponent<RunDesigned>().enabled = false;
        Robot.GetComponent<RunAIFOnly>().enabled = false;
        Robot.GetComponent<RunAIFWithComp>().enabled = false;


        // Getting condition information for the scene.
        ModelCodition = ExperimentCondition.Models[ExperimentCondition.NowScene];

        // Setting condition properly.
        switch (ModelCodition)
        {
            case 1:
                Robot.GetComponent<RunDesigned>().enabled = true;
                // Debug.Log("Designed Run.");
                break;
            case 2:
                Robot.GetComponent<RunAIFOnly>().enabled = true;
                // Debug.Log("AIF Run.");
                break;
            case 3:
                Robot.GetComponent<RunAIFWithComp>().enabled = true;
                // Debug.Log("Compassion Run.");
                break;

            default:
                // Debug.Log("NO MODEL");
                break;

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
