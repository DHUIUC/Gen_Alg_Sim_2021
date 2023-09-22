using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataDisplay : MonoBehaviour
{

    public Text curGen;
    public Text curBF;

    private float curBestFit;

    private int genNum;
    public static bool needChange = false;
    public static float newBestFit;

    // Start is called before the first frame update
    void Start()
    {
        curBestFit = 100;
        genNum = 1;
    }


    private void Update()
    {
        if (needChange)
            setData();
    }

    //Only need bestfit, Current Generation can only increment by 1
    public void setData()
    {

        if (newBestFit < curBestFit)
        {
            curBF.text = newBestFit.ToString();
            curBF.color = Color.green;
            curBestFit = newBestFit;
        }
        else
        {
            curBF.text = newBestFit.ToString();
            curBF.color = Color.red;
            curBestFit = newBestFit;
        }

        genNum++;
        curGen.text = genNum.ToString();

        needChange = false;
    }
 
    public static void Change(float bestFit)
    {
        needChange = true;
        newBestFit = bestFit;
    }

}
