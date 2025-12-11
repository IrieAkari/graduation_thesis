using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;


public class ExperimentCondition : MonoBehaviour
{   
    int R = 3;
    int L = -3;
    int C = 0;

    int Rx = -30;
    int Px = 25;

    // 1:CC-LP, 2:LL-LP, 3:CL-LP, 4:RLandGoBack-LP,    5:CC-RP, 6:RR-RP, 7:CR-RP, 8:CLandGoBack-RP

    public static readonly int[] SceneType = { 1, 8, 3, 6, 4,
                                                5, 2, 6, 3, 8,
                                                    1, 7, 3, 5, 4,
                                                        7, 2, 6, 1, 8,
                                                             2, 7, 4, 5 }; // Type1

    //public static readonly int[] SceneType = { 5, 4, 7, 2, 8, 1,
    //                                            6, 2, 7, 4, 5, 3,
    //                                                7, 1, 8, 3, 6, 2,
    //                                                    5, 4, 6, 3, 8, 1}; // Type2



    public static readonly int[] Models = { 2, 3, 1, 1, 3, 2,
                                                1, 3, 2, 2, 3, 1,
                                                    3, 3, 1, 2, 3, 2,
                                                    1, 1, 2, 3, 2, 1}; // Type1

    //public static readonly int[] Models = { 1, 2, 3, 2, 1, 1,
    //                                          2, 3, 2, 1, 3, 3,
    //                                              1, 3, 2, 2, 3, 1,
    //                                                  2, 3, 1, 1, 3, 2}; // Type2


    public static readonly int AllScene = SceneType.Length;



    public static int[] RobotStartX = new int[SceneType.Length];
    public static int[] RobotStartZ = new int[SceneType.Length];
    public static int[] PlayerStartX = new int[SceneType.Length];
    public static int[] PlayerStartZ = new int[SceneType.Length];

    public static int[] PreferDirection = new int[SceneType.Length];

    public static int[] TakeRelax = {0, 0, 0, 0, 0,
                                        0, 0, 1, 0, 0,
                                            0, 0, 0, 0, 0,
                                                1, 0, 0, 0, 0,
                                                    0, 0, 0, 0};

    public static int NowScene;
    public static readonly string DataTitle = DateTime.Now.ToString("yyyyMMddHHmmss");

    // Start is called before the first frame update
    void Start()
    {

        for(int i = 0; i < AllScene; i++)
        {
            switch (SceneType[i])
            {
                case 1:
                    RobotStartX[i] = -35;
                    RobotStartZ[i] = 0;
                    PlayerStartX[i] = 15;
                    PlayerStartZ[i] = 0;
                    PreferDirection[i] = 0;
                    break;

                case 2:
                    RobotStartX[i] = -35;
                    RobotStartZ[i] = -3;
                    PlayerStartX[i] = 15;
                    PlayerStartZ[i] = -3;
                    PreferDirection[i] = 0;
                    break;

                case 3:
                    RobotStartX[i] = -35;
                    RobotStartZ[i] = 0;
                    PlayerStartX[i] = 15;
                    PlayerStartZ[i] = -3;
                    PreferDirection[i] = 0;
                    break;

                case 4:
                    RobotStartX[i] = -45;
                    RobotStartZ[i] = 0;
                    PlayerStartX[i] = 5;
                    PlayerStartZ[i] = 3;
                    PreferDirection[i] = 0;
                    break;

                case 5:
                    RobotStartX[i] = -35;
                    RobotStartZ[i] = 0;
                    PlayerStartX[i] = 15;
                    PlayerStartZ[i] = 0;
                    PreferDirection[i] = 1;
                    break;

                case 6:
                    RobotStartX[i] = -35;
                    RobotStartZ[i] = 3;
                    PlayerStartX[i] = 15;
                    PlayerStartZ[i] = 3;
                    PreferDirection[i] = 1;
                    break;

                case 7:
                    RobotStartX[i] = -35;
                    RobotStartZ[i] = 0;
                    PlayerStartX[i] = 15;
                    PlayerStartZ[i] = 3;
                    PreferDirection[i] = 1;
                    break;

                case 8:
                    RobotStartX[i] = -45;
                    RobotStartZ[i] = 0;
                    PlayerStartX[i] = 5;
                    PlayerStartZ[i] = -3;
                    PreferDirection[i] = 1;
                    break;

                default:
                    RobotStartX[i] = -35;
                    RobotStartZ[i] = 0;
                    PlayerStartX[i] = 15;
                    PlayerStartZ[i] = 0;
                    PreferDirection[i] = 0;
                    break;
            }
        }

        NowScene = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
