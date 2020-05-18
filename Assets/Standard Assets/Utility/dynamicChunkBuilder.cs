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
        /*chunkDimensions = transform.parent.GetComponent<fixedWorldGen>().worldDimensions;
        //GameObject childBlockHolder = Instantiate(new GameObject(), transform.position, transform.rotation, transform);
        GameObject childBlockHolder = new GameObject("blockHolder");
        childBlockHolder.transform.parent = transform;
        childBlockHolder.transform.position = transform.position;
        childBlockHolder.SetActive(false);

        //GameObject childBlockHolder = Instantiate(new GameObject(), transform);
        //childBlockHolder.name = "blockHolder";
        childBlockHolder.layer = 13;

        //GameObject childSpawnedHolder = Instantiate(new GameObject(), transform);
        GameObject childSpawnedHolder = new GameObject("spawnedHolder");
        childSpawnedHolder.transform.parent = transform;
        childSpawnedHolder.layer = 13;
        childSpawnedHolder.SetActive(false);
        //childSpawnedHolder.name = "spawnedHolder";

        for (int zDimension = 0; zDimension < chunkDimensions; zDimension++)
        {
            spawnLocation.z = zDimension + transform.position.z;
            for (int yDimension = 0; yDimension > -chunkDimensions; yDimension--)
            {
                spawnLocation.y = yDimension + transform.position.y;
                for (int xDimension = 0; xDimension < chunkDimensions; xDimension++)
                {
                    spawnLocation.x = xDimension + transform.position.x;
                    GameObject tempNewBlock = Instantiate(blockToSpawn, spawnLocation, transform.rotation, childBlockHolder.transform);
                    tempNewBlock.SetActive(false);
                }
            }
        }

        GetComponent<chunkDistanceCalc>().enabled = true;
        transform.parent.GetComponent<fixedWorldGen>().buildNewListChunkImproved(gameObject);*/
    }

    private IEnumerator buildChunk()
    {
        int blockName = 0;
        yield return new WaitForFixedUpdate();
        //yield return new WaitForEndOfFrame();
        //yield return new WaitForSeconds(0);

        chunkDimensions = transform.parent.GetComponent<fixedWorldGen>().worldDimensions;
        GameObject childBlockHolder = new GameObject("blockHolder");
        childBlockHolder.transform.parent = transform;
        childBlockHolder.transform.position = transform.position;
        childBlockHolder.SetActive(false);
        childBlockHolder.layer = 13;

        GameObject childSpawnedHolder = new GameObject("spawnedHolder");
        childSpawnedHolder.transform.parent = transform;
        childSpawnedHolder.layer = 13;
        childSpawnedHolder.SetActive(false);

        for (int zDimension = 0; zDimension < chunkDimensions; zDimension++)
        {
            //yield return new WaitForFixedUpdate();
            spawnLocation.z = zDimension + transform.position.z;
            for (int yDimension = 0; yDimension > -chunkDimensions; yDimension--)
            {
                //yield return new WaitForFixedUpdate();
                spawnLocation.y = yDimension + transform.position.y;
                for (int xDimension = 0; xDimension < chunkDimensions; xDimension++)
                {
                    //yield return new WaitForFixedUpdate();
                    spawnLocation.x = xDimension + transform.position.x;
                    GameObject tempNewBlock = Instantiate(blockToSpawn, spawnLocation, transform.rotation, childBlockHolder.transform);
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
