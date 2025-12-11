using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayController : MonoBehaviour
{
    [SerializeField]
    SceneObject[] PlayScene = new SceneObject[8];

    [SerializeField]
    SceneObject RelaxScene;
    [SerializeField]
    SceneObject EndScene;

    [SerializeField]
    TextMeshProUGUI information;

    [SerializeField]
    Color fadeColor = Color.black;
    [SerializeField]
    float fadeSpeed = 1.0f;
    [SerializeField]
    float waitTime = 1.0f;

    [SerializeField]
    GameObject BlackOut;

    private int sceneType;

    int LRcheck = 0;
    int Relaxcheck = 0;

    private void Awake()
    {
        if (ExperimentCondition.NowScene >= ExperimentCondition.AllScene)   // ExperimentCondition.NowScene >= ExperimentCondition.AllScene
        {
            BlackOut.SetActive(true);
            Invoke("GoEnd",2f);
        }
        else
        {
            BlackOut.SetActive(false);
        }
    }
    void Start()
    {
        sceneType = ExperimentCondition.SceneType[ExperimentCondition.NowScene];
        // Get Conditions at the scene.
        LRcheck = ExperimentCondition.PreferDirection[ExperimentCondition.NowScene];
        Relaxcheck = ExperimentCondition.TakeRelax[ExperimentCondition.NowScene];

        information.text = "éüÇÃÉVÅ[ÉìÅF" + (ExperimentCondition.NowScene + 1).ToString();
    }

    void Update()
    {
       
    }

    public void GoPlay()
    {
        switch (sceneType)
        {
            case 1:
                Initiate.Fade(PlayScene[0], fadeColor, fadeSpeed);
                break;
            case 2:
                Initiate.Fade(PlayScene[1], fadeColor, fadeSpeed);
                break;
            case 3:
                Initiate.Fade(PlayScene[2], fadeColor, fadeSpeed);
                break;
            case 4:
                Initiate.Fade(PlayScene[3], fadeColor, fadeSpeed);
                break;
            case 5:
                Initiate.Fade(PlayScene[4], fadeColor, fadeSpeed);
                break;
            case 6:
                Initiate.Fade(PlayScene[5], fadeColor, fadeSpeed);
                break;
            case 7:
                Initiate.Fade(PlayScene[6], fadeColor, fadeSpeed);
                break;
            case 8:
                Initiate.Fade(PlayScene[7], fadeColor, fadeSpeed);
                break;
            default:
                break;
        }

    }


    public void GoRelax()
    {
        Initiate.Fade(RelaxScene, fadeColor, fadeSpeed);
    }

    public void GoEnd()
    {
        Initiate.Fade(EndScene, fadeColor, fadeSpeed);
    }
}
