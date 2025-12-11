using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NowInformation : MonoBehaviour
{
    public TextMeshProUGUI information;

    // Start is called before the first frame update
    void Start()
    {
        information.text = "Secne " + (ExperimentCondition.NowScene + 1).ToString()
            + "\n" + "Model " + (ExperimentCondition.Models[ExperimentCondition.NowScene]).ToString() + "\n" + "Type " + (ExperimentCondition.SceneType[ExperimentCondition.NowScene]).ToString();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
