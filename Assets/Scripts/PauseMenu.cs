using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                resumeSimulation();
            }
            else
            {
                pauseSimulation();
            }
        }
    }

    public void pauseSimulation()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void resumeSimulation()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;        //CHANGE TO WHATEVER THE PUB VAR FOR CURRENT SCENE SPEED WAS/IS
    }
}
