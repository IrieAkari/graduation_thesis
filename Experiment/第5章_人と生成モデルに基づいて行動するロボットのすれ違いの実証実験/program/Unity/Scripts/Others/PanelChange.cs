using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelChange : MonoBehaviour
{
    int PN;     // Number of panels.
    int Pt;
    public GameObject[] Panel;

    void Start()
    {
        PN = Panel.Length;
        Pt = 0;

        for (int i = 0; i<PN; i++)
        {
            Panel[i].SetActive(false);
        }

        Panel[0].SetActive(true);
    }

    public void NextView()
    {
        Pt++;
        if (Pt >= PN)
        {
            Panel[Pt-1].SetActive(false);
            Panel[0].SetActive(true);
            Pt = 0;
        }
        else
        {
            Panel[Pt-1].SetActive(false);
            Panel[Pt].SetActive(true);
        }
    }

    public void BackView()
    {
        Pt--;
        if (Pt < 0)
        {
            Panel[Pt + 1].SetActive(false);
            Panel[PN].SetActive(true);
            Pt = PN;
        }
        else
        {
            Panel[Pt + 1].SetActive(false);
            Panel[Pt].SetActive(true);
        }
    }
}