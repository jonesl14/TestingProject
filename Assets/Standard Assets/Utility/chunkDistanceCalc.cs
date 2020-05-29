using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class chunkDistanceCalc : MonoBehaviour
{
    int viewDistanceControl = 30;
    private GameObject playerRef;
    public int worldDimensions, squaredWorldDimensions, cubedWorldDimensions;
    private bool showing = true, showingClose = false;
    public Dictionary<string, List<int>> outsideBrokenBlocks = new Dictionary<string, List<int>>();

    int numberOfBlocksToBreak = 0;

    protected int[] exposedBlocks;

    private void Awake()
    {
        worldDimensions = transform.parent.GetComponent<fixedWorldGen>().worldDimensions;
        numberOfBlocksToBreak = transform.parent.GetComponent<fixedWorldGen>().numOfBlocksToBreak;
        squaredWorldDimensions = worldDimensions * worldDimensions;
        cubedWorldDimensions = worldDimensions * worldDimensions * worldDimensions;

        exposedBlocks = new int[cubedWorldDimensions];

        for (int blockIndexSet = 0; blockIndexSet < exposedBlocks.Length; blockIndexSet++)
        {
            exposedBlocks[blockIndexSet] = 2;
        }
    }

    private void Start()
    {
        outsideBrokenBlocks.Add("top", new List<int>());
        outsideBrokenBlocks.Add("right", new List<int>());
        outsideBrokenBlocks.Add("bottom", new List<int>());
        outsideBrokenBlocks.Add("left", new List<int>());
        outsideBrokenBlocks.Add("front", new List<int>());
        outsideBrokenBlocks.Add("back", new List<int>());

        playerRef = GameObject.Find("FPSController");

        for (int epochCount = 0; epochCount < numberOfBlocksToBreak; epochCount++)
        {
            transform.GetChild(0).transform.GetChild(UnityEngine.Random.Range(0, cubedWorldDimensions)).GetComponent<BlockBreaking>().instaKillBlock();
        }
        StartCoroutine(determineBlockStates());

        StartCoroutine(checkIfShouldShow());

        StartCoroutine(checkSurroundingChunks());
    }

    private IEnumerator determineBlockStates()
    {
        yield return new WaitForEndOfFrame();
        //0 is an inactive block
        //1 is an active block with the active flag(has been exposed)
        //2 is an inactive block with the active flag(has not been exposed)
        int blockIndex = 0;
        foreach(Transform childrenBlocks in transform.GetChild(0))
        {
            if(exposedBlocks[blockIndex] == 0)
            {
                if (!(blockIndex % (worldDimensions * worldDimensions) >= worldDimensions))
                {
                    outsideBrokenBlocks["top"].Add(blockIndex);
                }
                if (!((blockIndex + 1) % worldDimensions != 0 || blockIndex == 0))
                {
                    outsideBrokenBlocks["right"].Add(blockIndex);
                }
                if (!(blockIndex % (worldDimensions * worldDimensions) < (worldDimensions * (worldDimensions - 1))))
                {
                    outsideBrokenBlocks["bottom"].Add(blockIndex);
                }
                if (!((blockIndex) % worldDimensions != 0 && (blockIndex - 1) >= 0))
                {
                    outsideBrokenBlocks["left"].Add(blockIndex);
                }
                if (!(blockIndex < (worldDimensions * worldDimensions * (worldDimensions - 1))))
                {
                    outsideBrokenBlocks["front"].Add(blockIndex);
                }
                if (!(blockIndex >= (worldDimensions * worldDimensions)))
                {
                    outsideBrokenBlocks["back"].Add(blockIndex);
                }
            }
            if(exposedBlocks[blockIndex] == 2 && childrenBlocks.transform.position.y == 0)
            {
                changeBlockState(blockIndex, 1);
            }
            blockIndex++;
        }
        if (transform.position.y == 0)
        {
            GameObject.Find("OriginPoint").GetComponent<genMobsOnChunks>().spawnMob(transform);
        }
    }
    private Vector3 posToCheck;
    private bool chunkTop, chunkRight, chunkBottom, chunkLeft, chunkFront, chunkBack, allChunks;
    private Transform surroundingChunkToCheck;


    private IEnumerator checkSurroundingChunks()
    {
        yield return new WaitForEndOfFrame();
        posToCheck = transform.position;
        //while (!allChunks)
        {
            if (!chunkTop)
            {
                if (transform.position.y == 0)
                {
                    chunkTop = true;//This may need to be changed at a later date
                }
                posToCheck.y += worldDimensions;
                if (GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 0) < transform.parent.transform.childCount
                    && GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 0) > 0)
                {
                    if ((surroundingChunkToCheck = transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 0)))
                    && surroundingChunkToCheck.gameObject.GetComponent<chunkDistanceCalc>().outsideBrokenBlocks.ContainsKey("top"))
                    {
                        foreach (var item in surroundingChunkToCheck.gameObject.GetComponent<chunkDistanceCalc>().outsideBrokenBlocks["top"])
                        {
                            //if (exposedBlocks[item] != 0)
                            {
                                changeBlockState(item, 1);
                            }
                        }
                        chunkTop = true;
                    }
                }
                posToCheck.y -= worldDimensions;
            }
            if (!chunkRight)
            {
                posToCheck.x += worldDimensions;
                if (GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 1) < transform.parent.transform.childCount)
                {
                    if ((surroundingChunkToCheck = transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 1)))
                    && surroundingChunkToCheck.gameObject.GetComponent<chunkDistanceCalc>().outsideBrokenBlocks.ContainsKey("right"))
                    {

                        foreach (var item in surroundingChunkToCheck.gameObject.GetComponent<chunkDistanceCalc>().outsideBrokenBlocks["right"])
                        {
                            //if (exposedBlocks[item] != 0)
                            {
                                changeBlockState(item, 1);
                            }
                        }
                        chunkRight = true;
                    }
                }
                posToCheck.x -= worldDimensions;
            }
            if (!chunkBottom)
            {
                posToCheck.y -= worldDimensions;
                if (GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 2) < transform.parent.transform.childCount)
                {
                    if ((surroundingChunkToCheck = transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 2)))
                    && surroundingChunkToCheck.gameObject.GetComponent<chunkDistanceCalc>().outsideBrokenBlocks.ContainsKey("bottom"))
                    {
                        foreach (var item in surroundingChunkToCheck.gameObject.GetComponent<chunkDistanceCalc>().outsideBrokenBlocks["bottom"])
                        {
                            //if (exposedBlocks[item] != 0)
                            {
                                changeBlockState(item, 1);
                            }
                        }
                        chunkBottom = true;
                    }
                }
                posToCheck.y += worldDimensions;
            }
            if (!chunkLeft)
            {
                posToCheck.x -= worldDimensions;
                if (GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 3) < transform.parent.transform.childCount)
                {
                    if ((surroundingChunkToCheck = transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 3)))
                    && surroundingChunkToCheck.gameObject.GetComponent<chunkDistanceCalc>().outsideBrokenBlocks.ContainsKey("left"))
                    {
                        foreach (var item in surroundingChunkToCheck.gameObject.GetComponent<chunkDistanceCalc>().outsideBrokenBlocks["left"])
                        {
                            //if (exposedBlocks[item] != 0)
                            {
                                changeBlockState(item, 1);
                            }
                        }
                        chunkLeft = true;
                    }
                }
                posToCheck.x += worldDimensions;
            }
            if (!chunkFront)
            {
                posToCheck.z += worldDimensions;
                if (GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 4) < transform.parent.transform.childCount)
                {
                    if ((surroundingChunkToCheck = transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 4)))
                    && surroundingChunkToCheck.gameObject.GetComponent<chunkDistanceCalc>().outsideBrokenBlocks.ContainsKey("front"))
                    {
                        foreach (var item in surroundingChunkToCheck.gameObject.GetComponent<chunkDistanceCalc>().outsideBrokenBlocks["front"])
                        {
                            //if (exposedBlocks[item] != 0)
                            {
                                changeBlockState(item, 1);
                            }
                        }
                        chunkFront = true;
                    }
                }
                posToCheck.z -= worldDimensions;
            }
            if (!chunkBack)
            {
                posToCheck.z -= worldDimensions;
                if (GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 5) < transform.parent.transform.childCount)
                {
                    if ((surroundingChunkToCheck = transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 5)))
                    && surroundingChunkToCheck.gameObject.GetComponent<chunkDistanceCalc>().outsideBrokenBlocks.ContainsKey("back"))
                    {
                        foreach (var item in surroundingChunkToCheck.gameObject.GetComponent<chunkDistanceCalc>().outsideBrokenBlocks["back"])
                        {
                            //if (exposedBlocks[item] != 0)
                            {
                                changeBlockState(item, 1);
                            }
                        }
                        chunkBack = true;
                    }
                }
                posToCheck.z += worldDimensions;
            }
        }
    }

    public int checkChild(int childNum)
    {
        if (childNum < exposedBlocks.Length)
        {
            return exposedBlocks[childNum];
        }
        else
        {
            return -1;
        }    
    }

    private void showAllActiveBlocks()
    {
        for (int childBlockIndex = 0; childBlockIndex < exposedBlocks.Length; childBlockIndex++)
        {
            if (exposedBlocks[childBlockIndex] == 1)// || exposedBlocks[childBlockIndex] == 2)
            {
                transform.GetChild(0).transform.GetChild(childBlockIndex).gameObject.SetActive(true);
            }
        }
        showingClose = true;
    }
    private void showActiveBlocks()
    {
        for(int childBlockIndex = 0; childBlockIndex < exposedBlocks.Length; childBlockIndex++)
        {
            if (exposedBlocks[childBlockIndex] == 1)
            {
                transform.GetChild(0).transform.GetChild(childBlockIndex).gameObject.SetActive(true);
            }
            else if(exposedBlocks[childBlockIndex] == 2)
            {
                transform.GetChild(0).transform.GetChild(childBlockIndex).gameObject.SetActive(false);
            }
        }
        showing = true;
        showingClose = false;
    }
    private void hideActiveBlocks()
    {
        for (int childBlockIndex = 0; childBlockIndex < exposedBlocks.Length; childBlockIndex++)
        {
            if (exposedBlocks[childBlockIndex] == 1)
            {
                transform.GetChild(0).transform.GetChild(childBlockIndex).gameObject.SetActive(false);
            }
        }
        showing = false;
        showingClose = false;
    }

    public void changeBlockState(int blockNum, int stateNum)
    {
        //0 is an inactive block
        //1 is an active block with the active flag(has been exposed)
        //2 is an inactive block with the active flag(has not been exposed)
        {
            
            exposedBlocks[blockNum] = stateNum;
        }
        if(stateNum == 0)
        {
            transform.GetChild(0).transform.GetChild(blockNum).gameObject.SetActive(false);
            breakBlockAndShowSurrounds(blockNum);
        }
        showing = false;
        showingClose = false;
    }

    private void breakBlockAndShowSurrounds(int blockChildNum)
    {
        GameObject foundChunk;

        //top
        if (blockChildNum % (worldDimensions * worldDimensions) >= worldDimensions)
        {
            if (checkChild(blockChildNum - worldDimensions) != 0)
            {
                changeBlockState(blockChildNum - worldDimensions, 1);
            }
        }
        else if (GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 0) < transform.parent.childCount && GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 0) > 0)
        {
            foundChunk = transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 0)).gameObject;
            if (foundChunk.GetComponent<chunkDistanceCalc>().checkChild(blockChildNum + (worldDimensions * (worldDimensions - 1))) != 0)
            {
                foundChunk.GetComponent<chunkDistanceCalc>().changeBlockState(blockChildNum + (worldDimensions * (worldDimensions - 1)), 1);
            }
        }

        //right
        if ((blockChildNum + 1) % worldDimensions != 0)// || blockChildNum == 0)
        {
            if (checkChild(blockChildNum + 1) != 0)
            {
                changeBlockState(blockChildNum + 1, 1);
            }
        }
        else if (GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 1) < transform.parent.childCount && GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 1) >= 0)
        {
            foundChunk = transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 1)).gameObject;

            if (foundChunk.transform.position.y == transform.position.y)
            {
                if (foundChunk.GetComponent<chunkDistanceCalc>().checkChild(blockChildNum - (worldDimensions - 1)) != 0)
                {
                    foundChunk.GetComponent<chunkDistanceCalc>().changeBlockState(blockChildNum - (worldDimensions - 1), 1);
                }
            }
        }

        //bottom
        if (blockChildNum % (worldDimensions * worldDimensions) < (worldDimensions * (worldDimensions - 1)))
        {
            if (checkChild(blockChildNum + worldDimensions) != 0)
            {
                changeBlockState(blockChildNum + worldDimensions, 1);
            }
        }
        else if(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 2) < transform.parent.childCount && GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 2) >= 0)
        {
            foundChunk = transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 2)).gameObject;
            if (foundChunk.GetComponent<chunkDistanceCalc>().checkChild(blockChildNum - (worldDimensions * (worldDimensions - 1))) != 0)
            {
                foundChunk.GetComponent<chunkDistanceCalc>().changeBlockState(blockChildNum - (worldDimensions * (worldDimensions - 1)), 1);
            }
        }

        //left
        if ((blockChildNum) % worldDimensions != 0 && (blockChildNum - 1) >= 0)
        {
            if (checkChild(blockChildNum - 1) != 0)
            {
                changeBlockState(blockChildNum - 1, 1);
            }
        }
        else if (GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 3) < transform.parent.childCount && GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 3) >= 0)
        {
            foundChunk = transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 3)).gameObject;
            if (foundChunk.transform.position.y == transform.position.y)
            {
                if (foundChunk.GetComponent<chunkDistanceCalc>().checkChild(blockChildNum + worldDimensions - 1) != 0)
                {
                    foundChunk.GetComponent<chunkDistanceCalc>().changeBlockState(blockChildNum + worldDimensions - 1, 1);
                }
            }
        }

        //front
        if (blockChildNum < (worldDimensions * worldDimensions * (worldDimensions - 1)))
        {
            if (checkChild(blockChildNum + (worldDimensions * worldDimensions)) != 0)
            {
                changeBlockState(blockChildNum + (worldDimensions * worldDimensions), 1);
            }
        }
        else if (GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 4) < transform.parent.childCount && GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 4) >= 0)
        {
            foundChunk = transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 4)).gameObject;
            if (foundChunk.transform.position.y == transform.position.y)
            {
                if (foundChunk.GetComponent<chunkDistanceCalc>().checkChild(blockChildNum - (worldDimensions * worldDimensions * (worldDimensions - 1))) != 0)
                {
                    foundChunk.GetComponent<chunkDistanceCalc>().changeBlockState(blockChildNum - (worldDimensions * worldDimensions * (worldDimensions - 1)), 1);
                }
            }
        }

        //back
        if (blockChildNum >= (worldDimensions * worldDimensions))
        {
            if (checkChild(blockChildNum - (worldDimensions * worldDimensions)) != 0)
            {
                changeBlockState(blockChildNum - (worldDimensions * worldDimensions), 1);
            }
        }
        else if (GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 5) < transform.parent.childCount && GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 5) >= 0)
        {
            foundChunk = transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 5)).gameObject;
            if (foundChunk.GetComponent<chunkDistanceCalc>().checkChild(blockChildNum + (worldDimensions * worldDimensions * (worldDimensions - 1))) != 0)
            {
                foundChunk.GetComponent<chunkDistanceCalc>().changeBlockState(blockChildNum + (worldDimensions * worldDimensions * (worldDimensions - 1)), 1);
            }
        }
    }

    private IEnumerator checkIfShouldShow()
    {
        while (true)
        {
            yield return new WaitForSeconds(.1f);
            float chunkDistance = Vector3.Distance(transform.position, playerRef.transform.position);

            if (chunkDistance < viewDistanceControl / 2)
            {
                if (!showingClose)
                {
                    transform.GetChild(0).gameObject.SetActive(true);
                    transform.GetChild(1).gameObject.SetActive(true);
                    showAllActiveBlocks();
                }
            }
            else if (chunkDistance < viewDistanceControl)
            {
                if (!showing && showingClose)
                {
                    transform.GetChild(1).gameObject.SetActive(true);
                    transform.GetChild(0).gameObject.SetActive(true);
                    showActiveBlocks();
                }
            }
            else
            {
                if (showing)
                {
                    transform.GetChild(1).gameObject.SetActive(false);
                    transform.GetChild(0).gameObject.SetActive(false);
                    hideActiveBlocks();
                }
            }
        }
    }
}
