using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System.Linq;
using System;


public class RunDesigned : MonoBehaviour
{

    private TextAsset _csvFile;

    private List<string[]> _csvData = new List<string[]>();

    public GameObject leftWall;
    public GameObject rightWall;

    public GameObject NextTarget;
    public GameObject OppAgent;
    public GameObject Origin;

    Transform TStart;
    Vector3 posStart;

    Transform TNT;
    Transform T;
    Transform TOpp;
    Transform TOr;

    Vector3 posNT;
    Vector3 pos;
    Vector3 posOpp;
    Vector3 posOr;

    Rigidbody rb;

    int ROADw;
    int ROADl;

    float mm;
    float nn;
    int mINT;
    int nINT;

    int mINTcr;
    int nINTcr;


    float mmOr;
    float nnOr;
    int mOrINT;
    int nOrINT;

    float mmOrOpp;
    float nnOrOpp;
    int mOrINTOpp;
    int nOrINTOpp;


    int mOrINTcr;
    int nOrINTcr;

    int mOrINTOppcr;
    int nOrINTOppcr;

    int Action;

    float[,] array;


    int t;
    bool cross;


    float dx;
    float dz;

    float dn;

    Vector3 dist;
    Vector3 posCrt;


    float unit;
    float unitW;

    int UpB;


    // Start is called before the first frame update
    void Start()
    {
        int height = 0;

        string file = "ActionProb";

        _csvFile = Resources.Load(file) as TextAsset;  
        StringReader reader = new StringReader(_csvFile.text);

        while (reader.Peek() != -1)
        {
            string line = reader.ReadLine();
            _csvData.Add(line.Split(','));
            height++;
        }

        Debug.Log(height);

        TStart = this.transform;
        posStart = TStart.position;

        posCrt = posStart;

        rb = this.transform.GetComponent<Rigidbody>();

        TOr = Origin.transform;
        posOr = TOr.position;

        t = 1;
        cross = true;

        ROADw = 6;
        ROADl = 8;
        t = 1;
        cross = true;

        unit = 6;

        unitW = Math.Abs(leftWall.transform.position.z - rightWall.transform.position.z)/20;


        Debug.Log("Start.");

        dx = 0;
        dz = 0;

        array = new float[ROADw*ROADl*ROADw , 6];
        for (int k = 0; k < height; k++)
        {
            array[k, 0] = Convert.ToSingle(_csvData[k][1]);     
            array[k, 1] = Convert.ToSingle(_csvData[k][2]);
            array[k, 2] = Convert.ToSingle(_csvData[k][3]);

            array[k, 3] = Convert.ToSingle(_csvData[k][4]);  
            array[k, 4] = Convert.ToSingle(_csvData[k][5]);
            array[k, 5] = Convert.ToSingle(_csvData[k][6]);    
        }

        // Getting transform information.
        TNT = NextTarget.transform;
        T = this.transform;

        // Getting position information.
        posNT = TNT.position;
        pos = T.position;

        posNT.x = posStart.x + 10f;
        posNT.z = posStart.z;
        TNT.position = posNT;  // Setting initialn target position.
        Action = 1;


        Debug.Log("Initialized.");

        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
        UpB = UnityEngine.Random.Range(4, 9);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Getting transform information.
        TNT = NextTarget.transform;
        T = this.transform;
        TOpp = OppAgent.transform;

        // Getting position information.
        posNT = TNT.position;
        pos = T.position;
        posOpp = TOpp.position;

        float dis = Vector3.Distance(pos, posNT);


        mm = posOpp.z - pos.z;      // Vertical position
        nn = posOpp.x - pos.x;      // Horizontal position

        mINT = PositionConvertion(mm);

        nINT = (int)Mathf.Round(nn / unit);


        mmOr = pos.z - posOr.z;      // Vertical position

        mOrINT = PositionConvertion(mmOr);
        mOrINTOpp = PositionConvertion(posOpp.z - posOr.z);

        // Decelerating for designed run.
        if (0 < nn && nn < 18 && Math.Abs(mm) < 4)
        {
            rb.velocity = rb.velocity / 1.05f;
        }


        int mCrt = (int)Mathf.Round(pos.z);
        int nCrt = (int)Mathf.Round(pos.x);

        int mNew = (int)Mathf.Round(posCrt.z);
        int nNew = (int)Mathf.Round(posCrt.x);


        if (Vector3.Distance(pos, posCrt) < unit*1.5)
        {
            MoveCurrentPos();
        }


        // Only go straight.
        MoveTarget(1);


        mINTcr = mINT;
        nINTcr = nINT;

        mOrINTcr = mOrINT;
        mOrINTOppcr = mOrINTOpp;


        TNT.position = posNT;  // Setting target position.
    }

    int PositionConvertion(float n)
    {
        int nCon;

        if (n <= 8)
        {
            nCon = 2;
        }
        else if (8 < n && n <= 10)
        {
            nCon = 3;
        }
        else if (10 < n && n < 12)
        {
            nCon = 4;
        }
        else if (12 <= n)
        {
            nCon = 5;
        }
        else
        {
            Debug.Log("Convertion error");
            nCon = 3;
        }

        return nCon;
    }

    int ChoiceAction(int mINT, int nINT, int mOr)
    {

        int possibleA = 0;
        int mDif = mINT + ROADw - 1;

        int mc = mINT;
        int nc = nINT;
        int moc = mOr;

        int n = (mc - 1) + (nc - 1) * ROADw + (moc - 1) * ROADw * ROADl;

        if (nINT <= 0 || nINT > ROADl)
        {
            possibleA = 1;
            Debug.Log("Far away.");
        }
        else
        {
            Debug.Log("array: " + array[n, 0] + " / " + array[n, 1] + " / " + array[n, 2]);


            List<float> list = new List<float>() { array[n, 3], array[n, 4], array[n, 5] };

            Debug.Log(list[0] + " / " + list[1] + " / " + list[2]);

            if ((Mathf.Abs(list[0] - list[2]) < 0.0001f) && (list[0] > list[1]))
            {
                Debug.Log("Same Prob.");
                // Select when exactly same probability.
                UnityEngine.Random.InitState(DateTime.Now.Millisecond);
                possibleA = UnityEngine.Random.Range(0, 2);
                if (possibleA == 1)
                {
                    possibleA = 2;
                }
            }
            else
            {
                Debug.Log("One way.");
                float maxValue = list.Max();
                possibleA = list.IndexOf(maxValue);
            }
        }

        switch (possibleA)
        {
            case 0:
                Debug.Log("Left.");
                break;
            case 1:
                Debug.Log("Straight.");
                break;
            case 2:
                Debug.Log("Right.");
                break;

            default:
                Debug.Log("Error");
                break;
        }


        return possibleA;
    }

    void MoveCurrentPos()
    {
        posCrt = pos;
    }

    void MoveTarget(int Action)
    {
        switch (Action)
        {
            case 0:
                posNT.x = pos.x + unit / 2;
                posNT.z = pos.z + unit;
                // Debug.Log("Left.");
                break;
            case 1:
                posNT.x = pos.x + unit;
                posNT.z = pos.z;
                // Debug.Log("Straight.");
                break;
            case 2:
                posNT.x = pos.x + unit / 2;
                posNT.z = pos.z - unit;
                // Debug.Log("Right.");
                break;

            default:
                // Debug.Log("Error");
                break;
        }
    }

    void ExceptionMove()
    {
        posNT.x = pos.x + unit;
        posNT.z = pos.z;
        Debug.Log("Exception.");
    }
}
