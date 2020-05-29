using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class genMobsOnChunks : MonoBehaviour
{
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
        for (int mobIndex = 0; mobIndex < numberOfMobsToSpawn; mobIndex++)
        {
            foreach (GameObject objectIndex in objectsToSpawn)
            {
                GameObject tempMob = Instantiate(objectIndex, objectToSpawnOver);
                tempMob.transform.position = new Vector3(
                    tempMob.transform.position.x + mobIndex + Random.Range(0, worldDimensions),
                    tempMob.transform.position.y,
                    tempMob.transform.position.z + mobIndex + Random.Range(0, worldDimensions));
                tempMob.transform.SetParent(objectToSpawnOver.transform.GetChild(1));
            }
        }
    }
}
