﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Evolution : MonoBehaviour
{
    public GameObject drone;
    Rigidbody rb;
    private Vector3 startPoint, goalPoint;

    //initialize these in Start()?
    public int populationSize = 20;
    public int numAlleles = 40;       //should be divisible by 4
    public float randMax = 6.0f;

    private float[,] population;
    private float[] fitnessArr;
    private Vector3[] positions;
    private int[] parentIndices;

    public float bestFit;
    public int genCount;

    public int mutationChance;
    public int numParentsToKeep;
    public float goalNum;

    //private DataDisplay data;

    // Start is called before the first frame update
    void Start()
    {
        startPoint = drone.transform.position;

        rb = GetComponent<Rigidbody>();

        goalPoint = GameObject.Find("goal").transform.position;

       

        beginGA(rb, startPoint);

        //data = new DataDisplay();
    }

    //performs the GA algorithm
    void beginGA(Rigidbody rb, Vector3 startPoint)
    {
        //Initialize population
        population = new float[populationSize, numAlleles];

        for (int i = 0; i < populationSize; i++)
        {
            for (int j = 0; j < numAlleles; j++)
            {
                if ((j + 1) % 4 == 0)
                    population[i, j] = Random.Range(0, randMax);
                else
                    population[i, j] = Random.Range(-randMax, randMax);
            }
        }

        //arbitrary number for while loop condition
        bestFit = 100.0f;
        genCount = 1;

        StartCoroutine(Simulate(rb));
    }

    /**
     * This process uses elitism to select only the best parents.
     * This is alright for the given scenario, as we want to find the optimal
     * solution where the drone gets closest to the goal. 
     * 
     * May need to change to tournament selection?
     */
    void findParents()
    {
        bool[] skipList = new bool[populationSize]; //proportion of the population to keep

        int counter = 0;

        while (counter < populationSize / 2)
        {

            float min = -1.0f;
            int mindex = -1;

            for (int i = 0; i < fitnessArr.Length; i++)
            {
                if (!skipList[i] && min < 0)
                {
                    min = fitnessArr[i];
                    mindex = i;
                }
                else if (!skipList[i])
                {
                    if (min > fitnessArr[i])
                    {
                        min = fitnessArr[i];
                        mindex = i;
                    }
                }
            }

            skipList[mindex] = true;
            parentIndices[counter++] = mindex;

        }

        //Save the successful run? emulate it?
    }

    void mutate()
    {
        for (int i = 0; i < populationSize; i++)
        {
            int chance = Random.Range(0, 100);

            if (chance < mutationChance)
            {
                int randex = Random.Range(0, numAlleles - 1);

                if (population[i, randex] < 0)
                    population[i, randex] = -1.0f * Mathf.Abs(Mathf.Abs(population[i, randex]) - 6);
                else
                    population[i, randex] = Mathf.Abs(Mathf.Abs(population[i, randex]) - 6);

            }
        }
    }

    void crossOver()
    {
        float[,] newPopulation = new float[populationSize, numAlleles];

        //keeps specified number of parents. Works with 0

        for (int i=0;i<numParentsToKeep;i++)
        {
            for(int j=0; j < numAlleles; j++)
            {
                newPopulation[i, j] = population[parentIndices[i], j];
            }
        }

        for (int i = numParentsToKeep; i < populationSize; i++)
        {
            int parent1 = Random.Range(0, parentIndices.Length - 1);
            int parent2 = parent1;
            while (parent2 == parent1)
            {
                parent2 = Random.Range(0, parentIndices.Length - 1);
            }

            int crossover = Random.Range(1, numAlleles - 2);

            for (int j = 0; j < crossover; j++)
            {
                newPopulation[i, j] = population[parent1, j];
            }

            for (int j = crossover; j < numAlleles; j++)
            {
                newPopulation[i, j] = population[parent2, j];
            }

        }

        population = newPopulation;
    }
    

    /*
     * 
     */
    IEnumerator Simulate(Rigidbody rb)
    {

        while (bestFit > goalNum)
        {

            //gather positions of drone

            positions = new Vector3[populationSize];

            for (int i = 0; i < populationSize; i++)
            {

                for (int j = 0; j < numAlleles; j += 4)
                {
                   float x = population[i, j];
                   float y = population[i, j + 1];
                   float z = population[i, j + 2];
                   float speed = population[i, j + 3];

                    rb.velocity = rb.velocity - rb.velocity + new Vector3(x, y, z * speed);
                    yield return new WaitForSeconds(.3f);
                }

                rb.velocity = Vector3.zero;
                positions[i] = rb.transform.position;
                rb.transform.position = startPoint;
            }

            //determine fitness based off positions

            fitnessArr = new float[populationSize];

            for (int i = 0; i < positions.Length; i++)
            {
                Vector3 distance = goalPoint - positions[i];

                float x = distance.x;
                float y = distance.y;
                float z = distance.z;

                fitnessArr[i] = Mathf.Abs(x) + Mathf.Abs(y) + Mathf.Abs(z);
                //fitnessArr[i] = x + y + z;
            }



            //update bestfit?

            bestFit = Mathf.Min(fitnessArr);

            //if bestFit>.5 then begin breeding

            if (bestFit > goalNum)
            {
                parentIndices = new int[populationSize / 2];
                findParents();

                crossOver();

                mutate();

                genCount++;

                DataDisplay.Change(bestFit);
            }
        }

        DataDisplay.Change(bestFit);

        int index = -1;
        bool flag = true;

        for(int i=0;i<fitnessArr.Length&&flag; i++)
        {
            if (fitnessArr[i] == bestFit)
            {
                index = i;
                flag = false;
            }
        }

        float[] solution = new float[numAlleles];

        for(int i =0; i < numAlleles; i++)
        { solution[i] = population[index, i]; }

        //call movement method that allows for simulation of solution
        Optimal.AllowMovement(solution);
    }
}
