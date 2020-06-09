using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dynamicChunkBuilder : MonoBehaviour
{
    public GameObject blockToSpawn;

    private Vector3 spawnLocation = new Vector3(0,0,0);
    private int chunkDimensions;
    void Start()
    {
        StartCoroutine(buildChunk());
    }

    private IEnumerator buildChunk()
    {
        int blockName = 0;
        yield return new WaitForFixedUpdate();

        chunkDimensions = transform.parent.GetComponent<fixedWorldGen>().worldDimensions;
        GameObject childBlockHolder = new GameObject();
        childBlockHolder.transform.parent = transform;
        childBlockHolder.transform.position = transform.position;
        childBlockHolder.SetActive(false);
        childBlockHolder.layer = 13;

        GameObject childSpawnedHolder = new GameObject();
        childSpawnedHolder.transform.parent = transform;
        childSpawnedHolder.layer = 13;
        childSpawnedHolder.SetActive(false);

        /*for (int zDimension = 0; zDimension < chunkDimensions; zDimension++)
        {
            spawnLocation.z = zDimension + transform.position.z;
            for (int yDimension = 0; yDimension > -chunkDimensions; yDimension--)
            {
                spawnLocation.y = yDimension + transform.position.y;
                for (int xDimension = 0; xDimension < chunkDimensions; xDimension++)
                {
                    spawnLocation.x = xDimension + transform.position.x;
                    //GameObject tempNewBlock = Instantiate(blockToSpawn, spawnLocation, transform.rotation, childBlockHolder.transform);
                    Instantiate(blockToSpawn, spawnLocation, transform.rotation, childBlockHolder.transform).SetActive(false);
                    //tempNewBlock.SetActive(false);
                    blockName++;
                }
            }
        }*/

        for (int yDimension = 0; yDimension > -chunkDimensions; yDimension--)
        {
            spawnLocation.y = yDimension + transform.position.y;
            for (int zDimension = 0; zDimension > -chunkDimensions; zDimension--)
            {
                spawnLocation.z = zDimension + transform.position.z;
                for (int xDimension = 0; xDimension < chunkDimensions; xDimension++)
                {
                    spawnLocation.x = xDimension + transform.position.x;
                    GameObject tempNewBlock = Instantiate(blockToSpawn, spawnLocation, transform.rotation, childBlockHolder.transform);
                    //Instantiate(blockToSpawn, spawnLocation, transform.rotation, childBlockHolder.transform).SetActive(false);
                    tempNewBlock.SetActive(false);
                    tempNewBlock.name = blockName + "";
                    blockName++;
                }
            }
        }
        GetComponent<chunkDistanceCalc>().enabled = true;
        transform.parent.GetComponent<fixedWorldGen>().buildNewListChunkImproved(gameObject);
    }
}
