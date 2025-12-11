using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Walking
{
    public float time;
    public Vector3 Ppos;
    public Vector3 Pfwd;
    public Vector3 Rpos;
    public Vector3 Rfwd;


    public Walking(float time, Vector3 Ppos, Vector3 Pfwd, Vector3 Rpos, Vector3 Rfwd)
    {
        this.time = time;
        this.Ppos = Ppos;
        this.Pfwd = Pfwd;
        this.Rpos = Rpos;
        this.Rfwd = Rfwd;
    }
}

public class WalkingHistory : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    [SerializeField]
    private GameObject robot;

    [SerializeField]
    private GameObject LogObj;
    private Log Log;
    private List<Walking> walkings = new List<Walking>();

    private float StartTime;

    // Start is called before the first frame update
    void Start()
    {
        StartTime = Time.time;

        Log = LogObj.GetComponent<Log>();
        StartCoroutine(nameof(AddWalkingHistory));
    }

    void FixedUpdate()
    {

    }

    private IEnumerator AddWalkingHistory()
    {
        WaitForSeconds cachedWait = new WaitForSeconds(0.5f);
        while (true)
        {
            walkings.Add(new Walking(
                Time.time - StartTime,
                player.transform.position,
                player.transform.Find("PlayerCameraRoot").transform.forward,
                robot.transform.position,
                robot.transform.forward
            ));
        
            yield return cachedWait;
        }
    }

    private void OnDestroy()
    {
        MakeSavePath();
    }
    private void Save(string fileName, List<Walking> history)
    {
        // Outputting log.
        Log.Output(fileName, WalkingListToCSV(history));
    }

    private List<string> WalkingListToCSV(List<Walking> history)
    {
        List<string> str = new List<string>() { "time,Ppos.x,Ppos.y,Ppos.z,Pfwd.x,Pfwd.y,Pfwd.z,Rpos.x,Rpos.y,Rpos.z,Rfwd.x,Rfwd.y,Rfwd.z" };
        for (int i = 0; i < history.Count; i++)
        {
            str.Add(string.Join(",", new List<string>(){
                history[i].time.ToString(),
                history[i].Ppos.x.ToString(),
                history[i].Ppos.y.ToString(),
                history[i].Ppos.z.ToString(),
                history[i].Pfwd.x.ToString(),
                history[i].Pfwd.y.ToString(),
                history[i].Pfwd.z.ToString(),
                history[i].Rpos.x.ToString(),
                history[i].Rpos.y.ToString(),
                history[i].Rpos.z.ToString(),
                history[i].Rfwd.x.ToString(),
                history[i].Rfwd.y.ToString(),
                history[i].Rfwd.z.ToString()
            }));
        }
        return str;
    }

    private void MakeSavePath()
    {
        var fileName = "PathData_" + ExperimentCondition.DataTitle + "_Scene" + (ExperimentCondition.NowScene+1).ToString() + ".csv";
        Save(fileName, walkings);
    }
}