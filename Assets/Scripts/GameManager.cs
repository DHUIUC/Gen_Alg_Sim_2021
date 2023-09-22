using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance = null;

    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance !=this)
        {
            Destroy(gameObject);
        }

        //add DontDestroyOnLoad if needed
    }


    public void returnToTitle()
    {
        Time.timeScale = 1f;
        //Load Scene Title, stop any simulation running if need be
    }

    public void Exit()
    {
        Application.Quit();
    }
}
