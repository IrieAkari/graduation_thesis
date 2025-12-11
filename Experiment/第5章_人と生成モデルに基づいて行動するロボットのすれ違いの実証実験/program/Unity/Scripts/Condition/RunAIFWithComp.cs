using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System.Linq;
using System;


public class RunAIFWithComp : MonoBehaviour
{
    [SerializeField]
    private int PreferSide = 0;     //  0:Left, 1:Right
    [SerializeField]
    float NearDistance = 5.0f;
    [SerializeField]
    float VelocityDumper = 1.1f;

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


    // Start is called before the first frame update
    void Start()
    {
        int height = 0;


        // Loading CSV.
        string file = "ActionProb2";

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


        unitW = Math.Abs(leftWall.transform.position.z - rightWall.transform.position.z) / 20;


        Debug.Log("Start.");

        dx = 0;
        dz = 0;

        array = new float[ROADw * ROADl * ROADw, 6];
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

        // Set initial target position
        TNT.position = posNT;
        Action = 1;


        NearDistance = 4.5f;
        VelocityDumper = 1.2f;

        Debug.Log("Initialized.");
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


        mm = posOpp.z - pos.z;      // Vertical posiotion.
        nn = posOpp.x - pos.x;      // Horizontal position.

        mINT = PositionConvertion(mm);

        nINT = (int)Mathf.Round(nn / unit);


        mmOr = pos.z - posOr.z;      // Vertical position.

        mOrINT = PositionConvertion(mmOr);

        mOrINTOpp = PositionConvertion(posOpp.z - posOr.z);


        // Caluculating from "now" position.

        if (mOrINTOpp - mOrINTOppcr != 0 || nINT - nINTcr != 0 || mOrINT - mOrINTcr != 0)
        {

            Debug.Log("mHuman: " + mOrINTOpp + " nH-R:" + nINT + ", mRobot:" + mOrINT);

            if (mOrINT == mOrINTOpp && Action != 1 && (mOrINT == 3 || mOrINT == 4))
            {
                Debug.Log("Go same way.");
            }
            else
            {
                Action = ChoiceAction(mOrINTOpp, nINT, mOrINT);
                Debug.Log("Choose action.");
            }

        }


        int mCrt = (int)Mathf.Round(pos.z);
        int nCrt = (int)Mathf.Round(pos.x);

        int mNew = (int)Mathf.Round(posCrt.z);
        int nNew = (int)Mathf.Round(posCrt.x);


        if (Vector3.Distance(pos, posCrt) < unit * 1.5)
        {
            MoveCurrentPos();
        }

        MoveTarget(Action);

        if (Mathf.Abs(this.transform.position.z - leftWall.transform.position.z) < NearDistance)
        {
            if (Action == 2)
            {
                rb.velocity = rb.velocity / VelocityDumper;
            }
        }
        else if (Mathf.Abs(this.transform.position.z - rightWall.transform.position.z) < NearDistance)
        {
            if (Action == 0)
            {
                rb.velocity = rb.velocity / VelocityDumper;
            }
        }


        mINTcr = mINT;
        nINTcr = nINT;

        mOrINTcr = mOrINT;
        mOrINTOppcr = mOrINTOpp;

        TNT.position = posNT;  // Set target position.

    }


    int PositionConvertion(float n)
    {
        n = n / unitW;
        int nCon;

        switch (PreferSide)
        {


            case 0: //  Left prefered
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
                break;

            case 1: //  Right prefered
                if (n <= 8)
                {
                    nCon = 5;
                }
                else if (8 < n && n <= 10)
                {
                    nCon = 4;
                }
                else if (10 < n && n < 12)
                {
                    nCon = 3;
                }
                else if (12 <= n)
                {
                    nCon = 2;
                }
                else
                {
                    Debug.Log("Convertion error");
                    nCon = 3;
                }
                break;

            default:
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
                break;
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
