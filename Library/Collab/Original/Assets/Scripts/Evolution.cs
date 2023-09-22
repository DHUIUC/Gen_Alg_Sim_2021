using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evolution : MonoBehaviour
{
    Rigidbody rb;
    private Vector3 startPoint, goalPoint;

    public int populationSize = 20;
    public int numAlleles=40;       //should be divisible by 4
    public float randMax = 6.0f;

    private float[,] population;
    private float[] fitnessArr;
    private Vector3[] positions;
    private int[] parentIndices;

    public float bestFit;
    public int genCount;


    // Start is called before the first frame update
    void Start()
    {
        startPoint = this.transform.position;
        rb = GetComponent<Rigidbody>();

        goalPoint = GameObject.Find("goal").transform.position;

        beginGA(rb,startPoint);
    }

    //performs the GA algorithm
    void beginGA(Rigidbody rb, Vector3 startPoint)
    {
        //Initialize population
        population = new float[populationSize,numAlleles];

        for(int i = 0; i < populationSize; i++)
        {
            for(int j = 0; j < numAlleles; j++)
            {
                if((j+1)%4==0)
                    population[i,j] = Random.Range(0, randMax);
                else
                    population[i, j] = Random.Range(-randMax, randMax);
            }
        }

        //arbitrary number for while loop condition
        bestFit = 100.0f;
        genCount = 1;

        while (bestFit > 2.0f)
        {

            //gather positions of drone

            positions = new Vector3[populationSize];

            for (int i = 0; i < populationSize; i++)
            {

                for (int j = 0; j < numAlleles; j+=4)
                {
                    StartCoroutine(Move(rb, population[i, j], population[i, j + 1], population[i, j + 2], population[i, j + 3]));
                }

                positions[i] = rb.transform.position;
                rb.transform.position = startPoint;
            }


            //determine fitness based off positions

            fitnessArr = new float[populationSize];

            for(int i = 0; i < positions.Length; i++)
            {
                Vector3 distance = goalPoint - positions[i];

                float x = distance.x;
                float y = distance.y;
                float z = distance.z;

                fitnessArr[i] = x + y + z;
            }



            //update bestfit?

            bestFit = Mathf.Min(fitnessArr);

            //if bestFit>.5 then begin breeding

            if (bestFit > 0.5)
            {
                parentIndices = new int[populationSize / 2];
                findParents();

                crossOver();

                mutate();

                genCount++;

                //add graphical output for bestFit somewhere?
            }
        }

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

            for (int i = 0; i < populationSize; i++)
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
            parentIndices[counter] = mindex;
            counter++;
        }
    }

    void mutate()
    {
        for (int i = 0; i < populationSize; i++)
        {
            int chance = Random.Range(0, 100);

            if (chance < 10)
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

        //keep parents?

        for (int i = 0; i < populationSize; i++)
        {
            int parent1 = Random.Range(0, parentIndices.Length-1);
            int parent2 = Random.Range(0, parentIndices.Length - 1);

            if(parent2==parent1)
            {
                while (parent2 == parent1)
                {
                    parent2 = Random.Range(0, parentIndices.Length-1);
                }
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

    IEnumerator Move(Rigidbody rb, float x, float y, float z, float speed)
    {
        rb.velocity = rb.velocity - rb.velocity + new Vector3(x, y, z) * speed * Time.deltaTime);
        
        yield return new WaitForSeconds(.4f);
    }

    IEnumerator Simulate()
    {
        while (bestFit > 2.0f)
        {

            //gather positions of drone

            positions = new Vector3[populationSize];

            for (int i = 0; i < populationSize; i++)
            {

                for (int j = 0; j < numAlleles; j += 4)
                {
                    StartCoroutine(Move(rb, population[i, j], population[i, j + 1], population[i, j + 2], population[i, j + 3]));
                }

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

                fitnessArr[i] = x + y + z;
            }



            //update bestfit?

            bestFit = Mathf.Min(fitnessArr);

            //if bestFit>.5 then begin breeding

            if (bestFit > 0.5)
            {
                parentIndices = new int[populationSize / 2];
                findParents();

                crossOver();

                mutate();

                genCount++;

                //add graphical output for bestFit somewhere?
            }
        }
    }
}
