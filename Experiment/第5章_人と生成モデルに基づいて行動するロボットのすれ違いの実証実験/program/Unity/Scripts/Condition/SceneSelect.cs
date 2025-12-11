using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SceneSelect : MonoBehaviour
{
    public TMP_Dropdown ddtmp;

    // Start is called before the first frame update
    void Start()
    {
        List<string> optionlist = new List<string>();

        for (int i = 0; i < ExperimentCondition.AllScene; i++)
        {
            optionlist.Add("Scene " + (i+1).ToString());
        }

        ddtmp.ClearOptions();
        ddtmp.AddOptions(optionlist);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SceneJump()
    {
        ExperimentCondition.NowScene = ddtmp.value;
    }
}
