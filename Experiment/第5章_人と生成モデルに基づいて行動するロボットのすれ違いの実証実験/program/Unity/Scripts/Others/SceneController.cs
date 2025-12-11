using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//’Ç‰Á

public class SceneController : MonoBehaviour
{
    [SerializeField]
    SceneObject m_nextScene;
    [SerializeField]
    SceneObject goalScene;
    [SerializeField]
    SceneObject errorScene;
    [SerializeField]
    SceneObject collidedScene;
    [SerializeField]
    SceneObject masterScene;

    [SerializeField]
    Color fadeColor = Color.black;
    [SerializeField]
    float fadeSpeed = 1.0f;
    [SerializeField]
    float waitTime = 1.0f;
    

    void Start()
    {

    }

    void Update()
    {
        if(OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.LTouch))
        {
            if(OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.LTouch))
            {
                if (OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch))
                {
                    if(OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.LTouch))
                    {
                        if(OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
                        {
                            if(OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch))
                            {
                                GoMaster();
                            }
                        }
                    }
                }
            }
        }

        if(Input.GetKey(KeyCode.Z))
        {
            if (Input.GetKey(KeyCode.X))
            {
                if (Input.GetKey(KeyCode.C))
                {
                    if (Input.GetKey(KeyCode.V))
                    {
                        GoMaster();
                    }
                }
            }

        }

    }

    public void GoNext()
    {
        Initiate.Fade(m_nextScene, fadeColor, fadeSpeed);
    }

    public void GoGoal()
    {
        Initiate.Fade(goalScene, fadeColor, fadeSpeed);
    }

    public void GoError()
    {
        Initiate.Fade(errorScene, fadeColor, fadeSpeed);
    }

    public void GoCollided()
    {
        Initiate.Fade(collidedScene, fadeColor, fadeSpeed);
    }

    public void GoMaster()
    {
        Initiate.Fade(masterScene, fadeColor, fadeSpeed);
    }

}
