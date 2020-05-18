using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class genMobsOnChunks : MonoBehaviour
{
    //public GameObject mobToSpawn;
    private int correctRandomMobNumber;
    public int numberRange = 10;
    public int numberOfMobsToSpawn = 0;
    public GameObject[] objectsToSpawn;
    int worldDimensions;
    
    private void Start()
    {
        worldDimensions = GetComponent<fixedWorldGen>().worldDimensions;
    }

    public bool shouldSpawnMob(int randomMobNumber)
    {
        correctRandomMobNumber = Random.Range(0, numberRange);

        if(randomMobNumber == correctRandomMobNumber)
        {
            return true;
        }
        return false;
    }

    public void spawnMob(Transform objectToSpawnOver)
    {
        //UnityEngine.Debug.Log(numberOfMobsToSpawn);
        //chunkDistanceCalc 123
        //spawned sheep(AI) don't have a body? and are disabled by default
        //Add them back in and focus on combat?? and having them drop things


        //foreach (GameObject objectIndex in objectsToSpawn)
        for (int mobIndex = 0; mobIndex < numberOfMobsToSpawn; mobIndex++)
        {
            //for (int mobIndex = 0; mobIndex <= numberOfMobsToSpawn; mobIndex++)
            foreach (GameObject objectIndex in objectsToSpawn)
            //int mobIndex = 0;
            {
                GameObject tempMob = Instantiate(objectIndex, objectToSpawnOver);
                tempMob.transform.position = new Vector3(
                    tempMob.transform.position.x + mobIndex + Random.Range(0, worldDimensions),
                    tempMob.transform.position.y,
                    tempMob.transform.position.z + mobIndex + Random.Range(0, worldDimensions));
                //tempMob.transform.SetParent(null);
                tempMob.transform.SetParent(objectToSpawnOver.transform.GetChild(1));
            }
        }
        /*for (int mobIndex = 0; mobIndex < numberOfMobsToSpawn; mobIndex++)
        {
            GameObject tempMob = Instantiate(mobToSpawn, objectToSpawnOver);
            tempMob.transform.position = new Vector3(tempMob.transform.position.x + 2.5f, tempMob.transform.position.y + 1, tempMob.transform.position.z + 2.5f);
            tempMob.transform.SetParent(null);
        }*/
        //tempMob.transform.parent = null;
        //Spawn the mob over top of the associated object. Most likely over a chunk
    }
}
