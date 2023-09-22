using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Optimal : MonoBehaviour
{
    public GameObject drone;
    Rigidbody rb;
    private Vector3 startPoint;

    public static float[] moveSet;
    public static bool allowed;
    private bool isMoving;

    // Start is called before the first frame update
    void Start()
    {
        startPoint = drone.transform.position;
        rb = GetComponent<Rigidbody>();

        allowed = false;
        isMoving=false;
    }

    // Update is called once per frame
    void Update()
    {
        if (allowed&&!isMoving)
        {
            if (Input.GetKeyDown(KeyCode.Space))      //May need to change for keyboard compatabilities
            {
                isMoving = true;
                StartCoroutine(ShowPathing());
            }
        }
    }

    public static void AllowMovement(float[] pathing)
    {
        moveSet = pathing;
        allowed = true;
    }

    IEnumerator ShowPathing()
    {
        for (int i = 0; i < moveSet.Length; i += 4)
        {
            float x = moveSet[i];
            float y = moveSet[i+1];
            float z = moveSet[i+2];
            float speed = moveSet[i+3];

            rb.velocity = rb.velocity - rb.velocity + new Vector3(x, y, z * speed);
            yield return new WaitForSeconds(.3f);
        }

        rb.velocity = Vector3.zero;
        rb.transform.position = startPoint;
        isMoving = false;
    }
}
