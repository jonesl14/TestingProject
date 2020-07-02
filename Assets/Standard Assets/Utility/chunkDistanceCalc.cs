using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Break blocks once all the chunks have been created. Pick random chunk and random block num then break around that. Repeat
/// </summary>
public class chunkDistanceCalc : MonoBehaviour
{
    int viewDistanceControl = 50;
    public GameObject playerRef, fpsControllerRef;
    public int worldDimensions, squaredWorldDimensions, cubedWorldDimensions;
    private bool showing = true, showingClose = false;
    //public Dictionary<string, List<int>> outsideBrokenBlocks = new Dictionary<string, List<int>>();
    public List<int> topBrokenBlocks, rightBrokenBlocks, bottomBrokenBlocks, leftBrokenBlocks, frontBrokenBlocks, backBrokenBlocks;

    int numberOfBlocksToBreak = 0;

    protected int[] exposedBlocks;

    Vector3 centreChunkPos;

    private void Awake()
    {
        viewDistanceControl = GetComponentInParent<fixedWorldGen>().viewDistance;
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
        /*outsideBrokenBlocks.Add("top", new List<int>());//top
        outsideBrokenBlocks.Add("right", new List<int>());//right
        outsideBrokenBlocks.Add("bottom", new List<int>());//bottom
        outsideBrokenBlocks.Add("left", new List<int>());//left
        outsideBrokenBlocks.Add("front", new List<int>());//front
        outsideBrokenBlocks.Add("back", new List<int>());//back*/
        //worldDimensions /2.0f - .5
        Vector3 colliderCenter = new Vector3((worldDimensions / 2.0f) - .5f, -((worldDimensions / 2.0f) - .5f), -((worldDimensions / 2.0f) - .5f));
        GetComponent<BoxCollider>().center = colliderCenter;
        GetComponent<BoxCollider>().size = new Vector3(worldDimensions, worldDimensions, worldDimensions);

        centreChunkPos = transform.position;
        centreChunkPos.x += (worldDimensions / 2.0f) - .5f;
        centreChunkPos.y -= (worldDimensions / 2.0f) - .5f;
        centreChunkPos.z -= (worldDimensions / 2.0f) - .5f;

        playerRef = GetComponentInParent<fixedWorldGen>().playerRef;// GameObject.Find("FPSController");
        fpsControllerRef = GetComponentInParent<fixedWorldGen>().fpsControllerRef;


        /*for (int epochCount = 0; epochCount < numberOfBlocksToBreak; epochCount++)
        {
            transform.GetChild(0).transform.GetChild(UnityEngine.Random.Range(0, cubedWorldDimensions)).GetComponent<BlockBreaking>().instaKillBlock();
        }*/
        StartCoroutine(determineBlockStates());



        //StartCoroutine(checkIfShouldShow());
        //StartCoroutine(checkIfShouldShowRotation());
        StartCoroutine(checkIFShouldShowAngle());

        StartCoroutine(checkSurroundingChunks());

        /*for (int childBlockIndex = 0; childBlockIndex < exposedBlocks.Length; childBlockIndex++)
        {
            if (exposedBlocks[childBlockIndex] == 1)// || exposedBlocks[childBlockIndex] == 2)
            {
                transform.GetChild(0).transform.GetChild(childBlockIndex).gameObject.SetActive(true);
            }
            else
            {
                transform.GetChild(0).transform.GetChild(childBlockIndex).gameObject.SetActive(false);
            }
        }*/

        if (transform.position.y > 0)
        {
            StartCoroutine(settleBlocks());
        }
    }

    public void breakBlocks()
    {
        for (int epochCount = 0; epochCount < numberOfBlocksToBreak; epochCount++)
        {
            transform.GetChild(0).transform.GetChild(UnityEngine.Random.Range(0, cubedWorldDimensions-1)).GetComponent<BlockBreaking>().instaKillBlock();
        }
        if(transform.position.y < 0)
            smoothBlocks();
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
                    //outsideBrokenBlocks["top"].Add(blockIndex);
                    topBrokenBlocks.Add(blockIndex);
                }
                if (!((blockIndex + 1) % worldDimensions != 0 || blockIndex == 0))
                {
                    //outsideBrokenBlocks["right"].Add(blockIndex);
                    rightBrokenBlocks.Add(blockIndex);
                }
                if (!(blockIndex % (worldDimensions * worldDimensions) < (worldDimensions * (worldDimensions - 1))))
                {
                    //outsideBrokenBlocks["bottom"].Add(blockIndex);
                    bottomBrokenBlocks.Add(blockIndex);
                }
                if (!((blockIndex) % worldDimensions != 0 && (blockIndex - 1) >= 0))
                {
                    //outsideBrokenBlocks["left"].Add(blockIndex);
                    leftBrokenBlocks.Add(blockIndex);
                }
                if (!(blockIndex < (worldDimensions * worldDimensions * (worldDimensions - 1))))
                {
                    //outsideBrokenBlocks["front"].Add(blockIndex);
                    frontBrokenBlocks.Add(blockIndex);
                }
                if (!(blockIndex >= (worldDimensions * worldDimensions)))
                {
                    //outsideBrokenBlocks["back"].Add(blockIndex);
                    backBrokenBlocks.Add(blockIndex);
                }
            }
            if(exposedBlocks[blockIndex] == 2 && childrenBlocks.transform.position.y == 1)
            {
                changeBlockState(blockIndex, 1);
            }
            blockIndex++;
        }
        /*if (transform.position.y == 1)
        {
            GameObject.Find("OriginPoint").GetComponent<genMobsOnChunks>().spawnMob(transform);
        }*/
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
                    if ((surroundingChunkToCheck = transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 0))))
                    //&& surroundingChunkToCheck.gameObject.GetComponent<chunkDistanceCalc>().outsideBrokenBlocks.ContainsKey("top"))
                    {
                        //foreach (var item in surroundingChunkToCheck.gameObject.GetComponent<chunkDistanceCalc>().outsideBrokenBlocks["top"])
                        foreach(var item in topBrokenBlocks)
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
                    if ((surroundingChunkToCheck = transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 1))))
                    //&& surroundingChunkToCheck.gameObject.GetComponent<chunkDistanceCalc>().outsideBrokenBlocks.ContainsKey("right"))
                    {
                        //foreach (var item in surroundingChunkToCheck.gameObject.GetComponent<chunkDistanceCalc>().outsideBrokenBlocks["right"])
                        foreach (var item in rightBrokenBlocks)
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
                    if ((surroundingChunkToCheck = transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 2))))
                    //&& surroundingChunkToCheck.gameObject.GetComponent<chunkDistanceCalc>().outsideBrokenBlocks.ContainsKey("bottom"))
                    {
                        //foreach (var item in surroundingChunkToCheck.gameObject.GetComponent<chunkDistanceCalc>().outsideBrokenBlocks["bottom"])
                        foreach (var item in bottomBrokenBlocks)
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
                    if ((surroundingChunkToCheck = transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 3))))
                    //&& surroundingChunkToCheck.gameObject.GetComponent<chunkDistanceCalc>().outsideBrokenBlocks.ContainsKey("left"))
                    {
                        //foreach (var item in surroundingChunkToCheck.gameObject.GetComponent<chunkDistanceCalc>().outsideBrokenBlocks["left"])
                        foreach (var item in leftBrokenBlocks)
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
                    if ((surroundingChunkToCheck = transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 4))))
                    //&& surroundingChunkToCheck.gameObject.GetComponent<chunkDistanceCalc>().outsideBrokenBlocks.ContainsKey("front"))
                    {
                        //foreach (var item in surroundingChunkToCheck.gameObject.GetComponent<chunkDistanceCalc>().outsideBrokenBlocks["front"])
                        foreach (var item in frontBrokenBlocks)
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
                    if ((surroundingChunkToCheck = transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 5))))
                    //&& surroundingChunkToCheck.gameObject.GetComponent<chunkDistanceCalc>().outsideBrokenBlocks.ContainsKey("back"))
                    {
                        //foreach (var item in surroundingChunkToCheck.gameObject.GetComponent<chunkDistanceCalc>().outsideBrokenBlocks["back"])
                        foreach (var item in backBrokenBlocks)
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
        /*for (int childBlockIndex = 0; childBlockIndex < exposedBlocks.Length; childBlockIndex++)
        {
            if (exposedBlocks[childBlockIndex] == 1)// || exposedBlocks[childBlockIndex] == 2)
            {
                transform.GetChild(0).transform.GetChild(childBlockIndex).gameObject.SetActive(true);
            }
        }*/
        foreach(Transform childTransform in transform)
        {
            childTransform.gameObject.SetActive(true);
        }
        //transform.GetChild(1).gameObject.SetActive(true);
        //transform.GetChild(0).gameObject.SetActive(true);
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
        foreach (Transform childTransform in transform)
        {
            childTransform.gameObject.SetActive(true);
        }
        //transform.GetChild(1).gameObject.SetActive(true);
        //transform.GetChild(0).gameObject.SetActive(true);
        showing = true;
        showingClose = false;
    }
    private void hideActiveBlocks()
    {
        /*for (int childBlockIndex = 0; childBlockIndex < exposedBlocks.Length; childBlockIndex++)
        {
            if (exposedBlocks[childBlockIndex] == 1)
            {
                transform.GetChild(0).transform.GetChild(childBlockIndex).gameObject.SetActive(false);
            }
        }*/
        foreach (Transform childTransform in transform)
        {
            childTransform.gameObject.SetActive(false);
        }
        //transform.GetChild(1).gameObject.SetActive(false);
        //transform.GetChild(0).gameObject.SetActive(false);
        showing = false;
        showingClose = false;
    }

    
    public void changeBlockState(int blockNum, int stateNum)
    {
        bool anyBroken = false;
        //0 is an inactive block
        //1 is an active block with the active flag(has been exposed)
        //2 is an inactive block with the active flag(has not been exposed)
        {
            
            exposedBlocks[blockNum] = stateNum;
            //transform.GetChild(0).transform.GetChild(blockNum).GetComponent<BlockBreaking>().blockStatus = stateNum;
        }
        if(stateNum == 1 && transform.childCount == 2)
        {
            //UnityEngine.Debug.Log(transform.childCount);
            transform.GetChild(0).transform.GetChild(blockNum).gameObject.SetActive(true);
        }
        if(stateNum == 0)
        {
            //UnityEngine.Debug.Log("1\t\t"+blockNum);

            transform.GetChild(0).transform.GetChild(blockNum).gameObject.SetActive(false);
            //breakBlockAndShowSurrounds(blockNum);
            breakBlockAndShowSurroundsNew(blockNum);

            foreach(int blockIndex in exposedBlocks)
            {
                if(blockIndex != 0)
                {
                    anyBroken = true;
                }
            }
            if(!anyBroken)
            {
                //StopAllCoroutines();
                Destroy(transform.GetChild(0).gameObject);
                //hideActiveBlocks();

                UnityEngine.Debug.Log(transform.position + " all blocks broken");
            }
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

    private void breakBlockAndShowSurroundsNew(int blockChildNum)
    {
        //UnityEngine.Debug.Log("4\t\t" + blockChildNum);
        GameObject foundChunk;

        //top
        if(blockChildNum >= squaredWorldDimensions)
        {
            //UnityEngine.Debug.Log("top1 " + blockChildNum);
            if (checkChild(blockChildNum - squaredWorldDimensions) != 0)
            {
                changeBlockState(blockChildNum - squaredWorldDimensions, 1);
            }
        }
        else if (GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 0) < transform.parent.childCount && GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 0) >= 0)
        {
            //UnityEngine.Debug.Log("top2 " + blockChildNum);
            foundChunk = transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 0)).gameObject;
            if (foundChunk.GetComponent<chunkDistanceCalc>().checkChild(blockChildNum + (squaredWorldDimensions * (worldDimensions - 1))) != 0)
            {
                foundChunk.GetComponent<chunkDistanceCalc>().changeBlockState(blockChildNum + (squaredWorldDimensions * (worldDimensions - 1)), 1);
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
        // > squaredWorldDimensions * (worldDimension -1) && < cubedWorldDimension
        if (blockChildNum < (squaredWorldDimensions * (worldDimensions-1)))
        {
            if (checkChild(blockChildNum + squaredWorldDimensions) != 0)
            {
                changeBlockState(blockChildNum + squaredWorldDimensions, 1);
            }
        }
        else if (GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 2) < transform.parent.childCount && GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 2) >= 0)
        {
            foundChunk = transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 2)).gameObject;
            if (foundChunk.GetComponent<chunkDistanceCalc>().checkChild(blockChildNum - (squaredWorldDimensions * (worldDimensions - 1))) != 0)
            {
                foundChunk.GetComponent<chunkDistanceCalc>().changeBlockState(blockChildNum - (squaredWorldDimensions * (worldDimensions - 1)), 1);
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
        if ((blockChildNum % squaredWorldDimensions) >= worldDimensions)
        {
            if (checkChild(blockChildNum - worldDimensions) != 0)
            {
                changeBlockState(blockChildNum - worldDimensions, 1);
            }
        }
        else if (GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 4) < transform.parent.childCount && GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 4) >= 0)
        {
            foundChunk = transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 4)).gameObject;
            if (foundChunk.transform.position.y == transform.position.y)
            {
                //if (foundChunk.GetComponent<chunkDistanceCalc>().checkChild(blockChildNum - (squaredWorldDimensions-1)) != 0)
                if (foundChunk.GetComponent<chunkDistanceCalc>().checkChild(blockChildNum + (worldDimensions * (worldDimensions - 1))) != 0)
                {
                    //foundChunk.GetComponent<chunkDistanceCalc>().changeBlockState(blockChildNum - (squaredWorldDimensions - 1), 1);
                    foundChunk.GetComponent<chunkDistanceCalc>().changeBlockState(blockChildNum + (worldDimensions * (worldDimensions - 1)), 1);
                }
            }
        }

        //back
        if ((blockChildNum % squaredWorldDimensions) < (squaredWorldDimensions - worldDimensions))// || (blockChildNum - worldDimensions) % squaredWorldDimensions == 0)
        {
            if (checkChild(blockChildNum + worldDimensions) != 0)
            {
                changeBlockState(blockChildNum + worldDimensions, 1);
            }
        }
        else if (GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 5) < transform.parent.childCount && GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 5) >= 0)
        {
            //blockChildNum - squaredWorldDimensions - squaredWorldDimesions-worldDimensions
            foundChunk = transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 5)).gameObject;
            if (foundChunk.transform.position.y == transform.position.y)
            {
                if (foundChunk.GetComponent<chunkDistanceCalc>().checkChild(blockChildNum - (worldDimensions * (worldDimensions - 1))) != 0)
                {
                    foundChunk.GetComponent<chunkDistanceCalc>().changeBlockState(blockChildNum - (worldDimensions * (worldDimensions - 1)), 1);
                }
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
                    //transform.GetChild(0).gameObject.SetActive(true);
                    //transform.GetChild(1).gameObject.SetActive(true);
                    showAllActiveBlocks();
                }
            }
            else if (chunkDistance < viewDistanceControl)
            {
                if (!showing && showingClose)
                {
                    //transform.GetChild(1).gameObject.SetActive(true);
                    //transform.GetChild(0).gameObject.SetActive(true);
                    showActiveBlocks();
                }
            }
            else
            {
                if (showing)
                {
                    //transform.GetChild(1).gameObject.SetActive(false);
                    //transform.GetChild(0).gameObject.SetActive(false);
                    hideActiveBlocks();
                }
            }
        }
    }
    Vector3 targetDirTop, targetDirBottom;
    float angle;
    float chunkDistance;
    private IEnumerator checkIFShouldShowAngle()
    {
        //Vector3 topOfCentreChunk = centreChunkPos;
        //topOfCentreChunk.y = centreChunkPos.y + worldDimensions;
        Collider thisCollider = GetComponent<Collider>();
        //Vector3 bottomOfCentreChunk = centreChunkPos;
        //bottomOfCentreChunk.y = centreChunkPos.y - worldDimensions;
        Vector3 edgeOfChunk;// = GetComponent<Collider>().ClosestPoint(fpsControllerRef.transform.position);
        int layerMask = 1 << 10;
        while (true)
        {
            //viewDistanceControl = GetComponentInParent<fixedWorldGen>().viewDistance;
            yield return new WaitForSeconds(.2f);
            //yield return new WaitForFixedUpdate();

            chunkDistance = Vector3.Distance(transform.position, fpsControllerRef.transform.GetChild(1).transform.position);

            edgeOfChunk = thisCollider.ClosestPoint(fpsControllerRef.transform.GetChild(1).transform.position);
            //GameObject.Find("tempShow").transform.position = edgeOfChunk;

            //if (chunkDistance >= viewDistanceControl)
            {
                targetDirTop = fpsControllerRef.transform.GetChild(1).transform.position - edgeOfChunk;
                angle = Vector3.Angle(fpsControllerRef.transform.GetChild(1).transform.forward, targetDirTop);
                //UnityEngine.Debug.Log(angle);

                if ((angle) >= Camera.main.fieldOfView || angle == 0)
                {
                    float rayDistance = Vector3.Distance(fpsControllerRef.transform.GetChild(1).transform.position, edgeOfChunk);

                    //if (rayDistance >= viewDistanceControl)
                    {
                        if (Physics.Raycast(edgeOfChunk, targetDirTop.normalized, rayDistance, layerMask))
                        {
                            if (showingClose)
                            {
                                hideActiveBlocks();
                            }
                        }
                        else
                        {
                            if (!showingClose)
                            {
                                showAllActiveBlocks();
                            }
                        }
                    }
                    /*else if (rayDistance < viewDistanceControl)
                    {
                        if (!showingClose)
                        {
                            showAllActiveBlocks();
                        }
                    }*/
                }
                else
                {
                    if(chunkDistance < (worldDimensions + (worldDimensions/2)))
                    {
                        showAllActiveBlocks();
                    }
                    else if (showingClose)
                    {
                        hideActiveBlocks();
                    }
                }
            }
            /*else
            {
                if (!showingClose)
                {
                    showAllActiveBlocks();
                }
            }*/
        }
    }

    //private void settleBlocks()
    bool firstSettle = true;
    private IEnumerator settleBlocks()
    {
        //yield return new WaitForSeconds(1);
        yield return new WaitForFixedUpdate();
        bool blockMoved = true;
        int blocksMoved = 0;

        while (blockMoved)
        {
            //yield return new WaitForFixedUpdate();
            //yield return new WaitForSeconds(.5f);
            blockMoved = false;
            //foreach (int blockNum in exposedBlocks)
            for(int blockNum = 0; blockNum < exposedBlocks.Length; blockNum++)
            {
                int blockBelow = blockNum + squaredWorldDimensions;
                if (blockBelow < exposedBlocks.Length && exposedBlocks[blockBelow] == 0 && exposedBlocks[blockNum] == 1)
                {
                    //UnityEngine.Debug.Log("settled " + blockNum);
                    changeBlockState(blockNum, 0);
                    changeBlockState(blockBelow, 1);
                    blocksMoved++;
                    //exposedBlocks[blockNum] = 0;
                    //exposedBlocks[blockBelow] = 1;
                    blockMoved = true;
                }
            }
        }
        
        if(!firstSettle && transform.position.y > 0)
        {
            //yield return new WaitForSeconds(2);
            //UnityEngine.Debug.Log("settle");
            //GetComponentInParent<genMobsOnChunks>().spawnMob(transform);
            GameObject.Find("OriginPoint").GetComponent<genMobsOnChunks>().spawnMob(transform);
        }
        //UnityEngine.Debug.Log(blocksMoved);
        if (firstSettle)
        {
            //yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(1);
            firstSettle = false;
            smoothBlocks();
        }
        /*else
        {
            if (transform.position.y >0)
            {
                //GameObject.Find("OriginPoint").GetComponent<genMobsOnChunks>().spawnMob(transform);
            }
        }*/
    }

    //private IEnumerator smoothBlocks()
    private void smoothBlocks()
    {
        for (int blockNum = 0; blockNum < exposedBlocks.Length; blockNum++)
        {
            if (exposedBlocks[blockNum] == 1)
            {
                int blockLeft = blockNum - 1;
                int blockRight = blockNum + 1;

                int blockUp = blockNum - worldDimensions;
                int blockDown = blockNum + worldDimensions;

                int blockUpLeft = blockNum - (worldDimensions + 1);
                int blockUpRight = blockNum - (worldDimensions - 1);

                int blockDownLeft = blockNum + (worldDimensions - 1);
                int blockDownRight = blockNum + (worldDimensions + 1);

                int blockDownFront2 = blockNum + ((squaredWorldDimensions * 2) - worldDimensions);
                int blockDownBack2 = blockNum + ((squaredWorldDimensions * 2) + worldDimensions);

                if (blockLeft >= 0 && blockRight < exposedBlocks.Length && exposedBlocks[blockLeft] == 0 && exposedBlocks[blockRight] == 0)
                {
                    changeBlockState(blockNum, 0);
                }
                else if (blockUp >= 0 && blockDown < exposedBlocks.Length && exposedBlocks[blockUp] == 0 && exposedBlocks[blockDown] == 0)
                {
                    changeBlockState(blockNum, 0);
                }
                else if (blockUpLeft >= 0 && blockDownRight < exposedBlocks.Length && exposedBlocks[blockUpLeft] == 0 && exposedBlocks[blockDownRight] == 0)
                {
                    changeBlockState(blockNum, 0);
                }
                else if (blockUpRight >= 0 && blockDownLeft < exposedBlocks.Length && exposedBlocks[blockUpRight] == 0 && exposedBlocks[blockDownLeft] == 0)
                {
                    changeBlockState(blockNum, 0);
                }

                else if ((blockDownLeft + (worldDimensions-1)) < exposedBlocks.Length && exposedBlocks[blockDownLeft + (worldDimensions - 1)] == 0)
                {
                    changeBlockState(blockNum, 0);
                }
                else if ((blockDownRight + (worldDimensions + 1)) < exposedBlocks.Length && exposedBlocks[blockDownRight + (worldDimensions + 1)] == 0)
                {
                    changeBlockState(blockNum, 0);
                }
                else if (blockDownFront2 < exposedBlocks.Length && exposedBlocks[blockDownFront2] == 0)
                {
                    changeBlockState(blockNum, 0);
                }
                else if (blockDownBack2 < exposedBlocks.Length && exposedBlocks[blockDownBack2] == 0)
                {
                    changeBlockState(blockNum, 0);
                }
            }
        }
        if (transform.position.y > 0)
        {
            StartCoroutine(settleBlocks());
        }
    }

    //private IEnumerator checkVisibleByRay()
    void Updated()
    {   
        //yield return new WaitForFixedUpdate();
        RaycastHit hit;
        
        /*Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out hit))
        {
            Debug.DrawLine(ray.origin, hit.point);
        }*/

        ///Using FPSController's raycastRef
        Vector3 rayDirection = (fpsControllerRef.transform.GetChild(1).position - centreChunkPos).normalized;
        float rayDistance = Vector3.Distance(fpsControllerRef.transform.GetChild(1).position, centreChunkPos);

        ///Using FirstPersonCharacter's raycastRef2
        //Vector3 rayDirection = (playerRef.transform.GetChild(3).position - centreChunkPos).normalized;
        //float rayDistance = Vector3.Distance(playerRef.transform.GetChild(3).position, centreChunkPos);

        /*if(rayDistance >= viewDistanceControl * 3)
        {
            if(showingClose)
            hideActiveBlocks();
        }*/
        if (rayDistance >= viewDistanceControl)// && rayDistance < viewDistanceControl*3)
        {
            //10 = blocks   longer view distance but flashing chunks that appear and dissapear  The chunk is hiding its blocks then re-showing them causing the flashing
            //15 = chunks   short view distance but no flashing chunks
            int layerMask = 1 << 15;

            //if (Physics.Raycast(centreChunkPos, rayDirection, out hit, rayDistance, layerMask))
            if (Physics.Raycast(centreChunkPos, rayDirection, rayDistance, layerMask))
            {
                //Debug.DrawRay(centreChunkPos, rayDirection * hit.distance, Color.yellow);
                if (showingClose)
                {
                    hideActiveBlocks();
                }
            }
            else
            {
                //Debug.DrawRay(centreChunkPos, rayDirection * 1000, Color.white);
                if (!showingClose)
                {
                    showAllActiveBlocks();
                }
            }
        }
        else if(rayDistance < viewDistanceControl)
        {
            if (!showingClose)
            {
                showAllActiveBlocks();
            }
        }
    }

    private IEnumerator checkIfShouldShowRotation()
    {
        while (true)
        {
            yield return new WaitForSeconds(.5f);
            float chunkDistance = Vector3.Distance(transform.position, playerRef.transform.position);

            if (chunkDistance < viewDistanceControl)
            {
                //Looking North-East
                if (fpsControllerRef.transform.localEulerAngles.y >= 0 && fpsControllerRef.transform.localEulerAngles.y < 90)
                {
                    if (transform.position.z + worldDimensions < playerRef.transform.position.z && transform.position.x + worldDimensions < playerRef.transform.position.x)
                    {
                        //if (showingClose)
                        {
                            hideActiveBlocks();
                        }
                    }
                    else
                    {
                        //if (!showingClose)
                        {
                            showAllActiveBlocks();
                        }
                        //showAllActiveBlocks();
                    }
                }
                //Looking South-East
                else if (fpsControllerRef.transform.localEulerAngles.y >= 90 && fpsControllerRef.transform.localEulerAngles.y < 180)
                {
                    if (transform.position.z > playerRef.transform.position.z && transform.position.x + worldDimensions < playerRef.transform.position.x)
                    {
                        hideActiveBlocks();
                    }
                    else
                    {
                        showAllActiveBlocks();
                    }
                }
                //Looking South-West
                else if (fpsControllerRef.transform.localEulerAngles.y >= 180 && fpsControllerRef.transform.localEulerAngles.y < 270)
                {
                    if (transform.position.z > playerRef.transform.position.z && transform.position.x > playerRef.transform.position.x)
                    {
                        hideActiveBlocks();
                    }
                    else
                    {
                        showAllActiveBlocks();
                    }
                }
                //Looking North-West
                else if (fpsControllerRef.transform.localEulerAngles.y >= 270 && fpsControllerRef.transform.localEulerAngles.y <= 360)
                {
                    if (transform.position.z + worldDimensions < playerRef.transform.position.z && transform.position.x - worldDimensions > playerRef.transform.position.x)
                    {
                        hideActiveBlocks();
                    }
                    else
                    {
                        showAllActiveBlocks();
                    }
                }
            }
            else
            {
                if (showingClose)
                {
                    hideActiveBlocks();
                }
            }
        }
    }
}
