using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManipulation : MonoBehaviour
{
    public float time =1;
    public float timeMax = 16f;
    //add timeMin if you want slower than 1x speed. 

    public Text text;

    // Start is called before the first frame update
    void Start()
    {
        time = 1f;
        setText();
    }

    public void setText()
    {
        text.text = time.ToString() + "x";
    }
    
    public void speedUp(){

        if (time < timeMax && Time.timeScale > 0)
        {
            time = time * 2;
            Time.timeScale = time;
            setText();
        }
    }

    public void slowDown()
    {
        //potentially change to timeMin instead of 1
        if (time > 1 && Time.timeScale > 0)
        {
            time = time / 2;
            Time.timeScale = time;
            setText();
        }
    }
}
