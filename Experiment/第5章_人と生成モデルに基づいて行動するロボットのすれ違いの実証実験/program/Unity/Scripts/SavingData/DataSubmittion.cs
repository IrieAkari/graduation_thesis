using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class DataSubmittion : MonoBehaviour
{

    [SerializeField]
    private GameObject LogObj;
    private Log Log;

    private StreamWriter sw;
    public Slider[] Eval;

    List<string> Answers = new List<string>();

    int nowscene;

    int Model;

    int sceneType;

    float Rx;
    float Rz;

    float Px;
    float Pz;

    // Start is called before the first frame update
    void Start()
    {
        nowscene = ExperimentCondition.NowScene;

        Model = ExperimentCondition.Models[ExperimentCondition.NowScene];

        sceneType = ExperimentCondition.SceneType[ExperimentCondition.NowScene];

        Rx = ExperimentCondition.RobotStartX[ExperimentCondition.NowScene];
        Rz = ExperimentCondition.RobotStartZ[ExperimentCondition.NowScene];

        Px = ExperimentCondition.PlayerStartX[ExperimentCondition.NowScene];
        Pz = ExperimentCondition.PlayerStartZ[ExperimentCondition.NowScene];
        Log = LogObj.GetComponent<Log>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        AddSaveEval();
        AddSceneNumber();
    }

    private void AddSceneNumber()
    {       
        ExperimentCondition.NowScene = ExperimentCondition.NowScene + 1;
    }

    public void SaveData(int NowScene, int Model, int SceneType, float Rx, float Rz, float Px, float Pz, 
                                                    float Q1, float Q2, float Q3, float Q4, float Q5, float Q6)
    {
        Answers.Add(string.Join(",", new List<string>()
        {
            (NowScene+1).ToString(),  Model.ToString(), SceneType.ToString(),
                Rx.ToString(),  Rz.ToString(),  Px.ToString(),  Pz.ToString(),
                 Q1.ToString(),  Q2.ToString(),  Q3.ToString(),  Q4.ToString(),  Q5.ToString(), Q6.ToString()
        }));


    }

    private void Save(string fileName, List<string> Answers)
    {
        Log.Output(fileName, Answers);
    }

    private void AddSaveEval()
    {
        float Q1 = Eval[0].value;
        float Q2 = Eval[1].value;
        float Q3 = Eval[2].value;
        float Q4 = Eval[3].value;
        float Q5 = Eval[4].value;
        float Q6 = Eval[5].value;

        SaveData(nowscene, Model, sceneType, Rx, Rz, Px, Pz,
         Q1, Q2, Q3, Q4, Q5, Q6);

        var fileName = "Answers_" + ExperimentCondition.DataTitle + ".csv";
        Save(fileName, Answers);
    }
}
