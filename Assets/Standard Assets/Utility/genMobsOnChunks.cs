using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class genMobsOnChunks : MonoBehaviour
{
    private int correctRandomMobNumber;
    public int numberRange = 10;
    public int numberOfMobsToSpawn = 0;
    public GameObject[] objectsToSpawn;
    int worldDimensions, squaredWorldDimensions, cubedWorldDimensions;
    
    private void Start()
    {
        worldDimensions = GetComponent<fixedWorldGen>().worldDimensions;
        squaredWorldDimensions = worldDimensions * worldDimensions;
        cubedWorldDimensions = worldDimensions * worldDimensions * worldDimensions;
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
        ///Restrict the x any z axis' so that mobs won't spawn outside of their chunk
        for (int mobIndex = 0; mobIndex < numberOfMobsToSpawn; mobIndex++)
        {
            foreach (GameObject objectIndex in objectsToSpawn)
            {
                //GameObject tempMob = Instantiate(objectIndex, objectToSpawnOver);
                //float tempSpawnedX = tempMob.transform.position.x + mobIndex + Random.Range(0, worldDimensions);
                //float tempSpawnedZ = tempMob.transform.position.z - mobIndex - Random.Range(0, worldDimensions);

                float tempSpawnedX = mobIndex + Random.Range(0, worldDimensions) + objectToSpawnOver.position.x;
                float tempSpawnedZ = mobIndex - Random.Range(0, worldDimensions) + objectToSpawnOver.position.z;
                //UnityEngine.Debug.Log(tempSpawnedX + "\t\t" + tempSpawnedZ);
                
                float tempSpawnedY = findCorrectHeight((int)(tempSpawnedX + ((tempSpawnedZ * -1) * worldDimensions)), objectToSpawnOver);
                //UnityEngine.Debug.Log(objectIndex + "\t\t" + tempSpawnedY);

                //UnityEngine.Debug.Log(findCorrectHeight((int)(tempSpawnedX + tempSpawnedZ)));
                //if (tempSpawnedY != -999)
                if(tempSpawnedY < 2)
                {
                    GameObject tempMob = Instantiate(objectIndex, objectToSpawnOver);

                    tempMob.transform.position = new Vector3(
                        //tempMob.transform.position.x + mobIndex + Random.Range(0, worldDimensions),
                        tempSpawnedX,
                        //tempMob.transform.position.y,
                        -tempSpawnedY+2,
                        //tempMob.transform.position.z - mobIndex - Random.Range(0, worldDimensions));
                        tempSpawnedZ);
                    tempMob.transform.SetParent(objectToSpawnOver.transform.GetChild(1));
                }
                //UnityEngine.Debug.Log(tempMob.transform.position);
            }
        }
    }

    private int findCorrectHeight(int foundBlockPos, Transform objectToSpawnOver)
    {
        //UnityEngine.Debug.Log(foundBlockPos);
        for(int depth = 0; depth < worldDimensions; depth++)
        {
            if(foundBlockPos + (depth * squaredWorldDimensions) < cubedWorldDimensions
                && foundBlockPos + (depth * squaredWorldDimensions) >= 0
                && objectToSpawnOver.GetComponent<chunkDistanceCalc>().checkChild(foundBlockPos + (depth * squaredWorldDimensions)) == 1)
            {
                return depth;
            }
        }
        return 999;
    }
}
