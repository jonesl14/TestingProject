using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class findPlayerPos : MonoBehaviour
{
    public GameObject originPointChunkHolder;
    public Vector3 chunkRoundedPlayerPos;
    private Vector3 playerViewRange;

    private int chunkViewRange = 3, chunkDepth = 3;

    private List<GameObject> visibleChunks = new List<GameObject>();

    private void Start()
    {
        //StartCoroutine(checkSurroundingChunks());
        //StartCoroutine(checkSurroundingChunkDistance());
    }


    //private void Update()
    //{
    private IEnumerator checkSurroundingChunks()
    {
        while (true)
        {
            //yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(1);

            chunkRoundedPlayerPos.x = 5 * (int)Mathf.Round(transform.position.x / 5);
            chunkRoundedPlayerPos.y = 5 * (int)Mathf.Round(transform.position.y / 5);
            chunkRoundedPlayerPos.z = 5 * (int)Mathf.Round(transform.position.z / 5);

            if (originPointChunkHolder.transform.Find(chunkRoundedPlayerPos + ""))
            {
                originPointChunkHolder.transform.Find(chunkRoundedPlayerPos + "").GetComponent<chunkDistanceCalc>().showAllBlocks();
            }
            for (int viewIndex = 0; viewIndex < chunkViewRange; viewIndex++)
            {
                playerViewRange.x = chunkRoundedPlayerPos.x;
                playerViewRange.y = chunkRoundedPlayerPos.y;
                playerViewRange.z = chunkRoundedPlayerPos.z;

                for (int depthIndexFlip = 1; depthIndexFlip > -2; depthIndexFlip -= 2)
                {
                    //UnityEngine.Debug.Log(depthIndexFlip);
                    for (int depthIndex = 0; depthIndex < chunkDepth; depthIndex++)
                    {
                        playerViewRange.y = chunkRoundedPlayerPos.y - ((5 * depthIndex) * depthIndexFlip);

                        playerViewRange.x = chunkRoundedPlayerPos.x + (5 * viewIndex);
                        if (originPointChunkHolder.transform.Find(playerViewRange + ""))
                        {//Checks 1 block to the right
                            originPointChunkHolder.transform.Find(playerViewRange + "").GetComponent<chunkDistanceCalc>().showAllBlocks();
                            visibleChunks.Add(originPointChunkHolder.transform.Find(playerViewRange + "").gameObject);
                        }
                        playerViewRange.z = chunkRoundedPlayerPos.z + (5 * viewIndex);
                        if (originPointChunkHolder.transform.Find(playerViewRange + ""))
                        {//checks 1 block to the right and 1 block forward
                            originPointChunkHolder.transform.Find(playerViewRange + "").GetComponent<chunkDistanceCalc>().showAllBlocks();
                            visibleChunks.Add(originPointChunkHolder.transform.Find(playerViewRange + "").gameObject);
                        }
                        playerViewRange.z = chunkRoundedPlayerPos.z;

                        playerViewRange.z = chunkRoundedPlayerPos.z - (5 * viewIndex);
                        if (originPointChunkHolder.transform.Find(playerViewRange + ""))
                        {//checks 1 block to the right and 1 block backward
                            originPointChunkHolder.transform.Find(playerViewRange + "").GetComponent<chunkDistanceCalc>().showAllBlocks();
                            visibleChunks.Add(originPointChunkHolder.transform.Find(playerViewRange + "").gameObject);
                        }
                        playerViewRange.x = chunkRoundedPlayerPos.x;
                        playerViewRange.z = chunkRoundedPlayerPos.z;


                        playerViewRange.x = chunkRoundedPlayerPos.x - (5 * viewIndex);
                        if (originPointChunkHolder.transform.Find(playerViewRange + ""))
                        {//checks 1 block to the left
                            originPointChunkHolder.transform.Find(playerViewRange + "").GetComponent<chunkDistanceCalc>().showAllBlocks();
                            visibleChunks.Add(originPointChunkHolder.transform.Find(playerViewRange + "").gameObject);
                        }
                        playerViewRange.z = chunkRoundedPlayerPos.z + (5 * viewIndex);
                        if (originPointChunkHolder.transform.Find(playerViewRange + ""))
                        {//checks 1 block to the left and 1 block forward
                            originPointChunkHolder.transform.Find(playerViewRange + "").GetComponent<chunkDistanceCalc>().showAllBlocks();
                            visibleChunks.Add(originPointChunkHolder.transform.Find(playerViewRange + "").gameObject);
                        }
                        playerViewRange.z = chunkRoundedPlayerPos.z;

                        playerViewRange.z = chunkRoundedPlayerPos.z - (5 * viewIndex);
                        if (originPointChunkHolder.transform.Find(playerViewRange + ""))
                        {//checks 1 block to the left and 1 block backward
                            originPointChunkHolder.transform.Find(playerViewRange + "").GetComponent<chunkDistanceCalc>().showAllBlocks();
                            visibleChunks.Add(originPointChunkHolder.transform.Find(playerViewRange + "").gameObject);
                        }
                        playerViewRange.x = chunkRoundedPlayerPos.x;
                        playerViewRange.z = chunkRoundedPlayerPos.z;
                    }
                    playerViewRange.y = chunkRoundedPlayerPos.y;
                }

                playerViewRange.y = chunkRoundedPlayerPos.y;

                /*playerViewRange.y = chunkRoundedPlayerPos.y + (5 * viewIndex);
                if (originPointChunkHolder.transform.Find(playerViewRange + ""))
                {
                    originPointChunkHolder.transform.Find(playerViewRange + "").GetComponent<chunkDistanceCalc>().showAllBlocks();
                }
                //playerViewRange.y = chunkRoundedPlayerPos.y - (5 * viewIndex);
                //playerViewRange.x = chunkRoundedPlayerPos.x;// + (5 * viewIndex);
                playerViewRange.y = chunkRoundedPlayerPos.y;// + (5 * viewIndex);
                //playerViewRange.z = chunkRoundedPlayerPos.z;// + (5 * viewIndex);

                playerViewRange.y = chunkRoundedPlayerPos.y - (5 * viewIndex);
                if (originPointChunkHolder.transform.Find(playerViewRange + ""))
                {
                    originPointChunkHolder.transform.Find(playerViewRange + "").GetComponent<chunkDistanceCalc>().showAllBlocks();
                }
                //playerViewRange.y = chunkRoundedPlayerPos.y + (5 * viewIndex);
                //playerViewRange.x = chunkRoundedPlayerPos.x;// + (5 * viewIndex);
                playerViewRange.y = chunkRoundedPlayerPos.y;// + (5 * viewIndex);
                //playerViewRange.z = chunkRoundedPlayerPos.z;// + (5 * viewIndex);*/


                playerViewRange.z = chunkRoundedPlayerPos.z + (5 * viewIndex);
                if (originPointChunkHolder.transform.Find(playerViewRange + ""))
                {
                    originPointChunkHolder.transform.Find(playerViewRange + "").GetComponent<chunkDistanceCalc>().showAllBlocks();
                    visibleChunks.Add(originPointChunkHolder.transform.Find(playerViewRange + "").gameObject);
                }
                //playerViewRange.z = chunkRoundedPlayerPos.z - (5 * viewIndex);
                //playerViewRange.x = chunkRoundedPlayerPos.x;// + (5 * viewIndex);
                //playerViewRange.y = chunkRoundedPlayerPos.y;// + (5 * viewIndex);
                playerViewRange.z = chunkRoundedPlayerPos.z;// + (5 * viewIndex);

                playerViewRange.z = chunkRoundedPlayerPos.z - (5 * viewIndex);
                if (originPointChunkHolder.transform.Find(playerViewRange + ""))
                {
                    originPointChunkHolder.transform.Find(playerViewRange + "").GetComponent<chunkDistanceCalc>().showAllBlocks();
                    visibleChunks.Add(originPointChunkHolder.transform.Find(playerViewRange + "").gameObject);
                }
                //playerViewRange.z = chunkRoundedPlayerPos.z + (5 * viewIndex);
                //playerViewRange.x = chunkRoundedPlayerPos.x;// + (5 * viewIndex);
                //playerViewRange.y = chunkRoundedPlayerPos.y;// + (5 * viewIndex);
                playerViewRange.z = chunkRoundedPlayerPos.z;// + (5 * viewIndex);
            }
            //checkOnChunks();
        }
    }

    float visibleDistance = 30;

    private IEnumerator checkSurroundingChunkDistance()
    {
        while (true)
        {
            //yield return new WaitForFixedUpdate();
            //yield return new WaitForSeconds(.5f);
            foreach (Transform chunks in originPointChunkHolder.transform)
            {
                yield return new WaitForFixedUpdate();
                if (Vector3.Distance(transform.position, chunks.transform.position) < visibleDistance)
                {
                    //UnityEngine.Debug.Log("far " + Vector3.Distance(transform.position, chunks.transform.position));
                    chunks.transform.GetChild(0).gameObject.SetActive(true);
                    chunks.GetComponent<chunkDistanceCalc>().showAllBlocks();
                }
                else
                {
                    //UnityEngine.Debug.Log("near " + Vector3.Distance(transform.position, chunks.transform.position));
                    chunks.transform.GetChild(0).gameObject.SetActive(false);
                    chunks.GetComponent<chunkDistanceCalc>().hideAllBlocks();
                }
            }
        }
    }

    private void checkOnChunks()
    {
        foreach(Transform spawnedChunks in originPointChunkHolder.transform)
        {
            if(visibleChunks.Contains(spawnedChunks.gameObject))
            {
                spawnedChunks.transform.GetChild(0).gameObject.SetActive(true);
                spawnedChunks.GetComponent<chunkDistanceCalc>().showAllBlocks();
                UnityEngine.Debug.Log("contains " + spawnedChunks.name);
            }
            else
            {
                visibleChunks.Remove(spawnedChunks.gameObject);
                spawnedChunks.GetComponent<chunkDistanceCalc>().hideAllBlocks();
                UnityEngine.Debug.Log("doesn't contain " + spawnedChunks.name);
            }
        }
    }

}
