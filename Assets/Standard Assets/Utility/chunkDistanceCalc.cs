using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class chunkDistanceCalc : MonoBehaviour
{
    private GameObject playerRef;
    private int worldDimensions, squaredWorldDimensions, cubedWorldDimensions;
    //private Vector3 chunkPos;
    private bool showing = true, showingClose = false;
    //private bool currentChunk = false;
    //public List<int> brokenBlocks = new List<int>();
    public Dictionary<string, List<int>> outsideBrokenBlocks = new Dictionary<string, List<int>>();

    //private bool fixedSurroundingChunks = false;

    protected int[] exposedBlocks;// = new int[125];

    private void Awake()
    {
        worldDimensions = transform.parent.GetComponent<fixedWorldGen>().worldDimensions;
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
        /*worldDimensions = transform.parent.GetComponent<fixedWorldGen>().worldDimensions;
        squaredWorldDimensions = worldDimensions * worldDimensions;
        cubedWorldDimensions = worldDimensions * worldDimensions * worldDimensions;*/

        //exposedBlocks = new int[cubedWorldDimensions];

        /*for (int blockIndexSet = 0; blockIndexSet < exposedBlocks.Length; blockIndexSet++)
        {
            exposedBlocks[blockIndexSet] = 0;
        }*/

        for (int epochCount = 0; epochCount < 10; epochCount++)
        {
            //transform.GetChild(0).transform.GetChild(UnityEngine.Random.Range(0, cubedWorldDimensions)).GetComponent<BlockBreaking>().instaKillBlock();
            //transform.GetChild(0).transform.GetChild(UnityEngine.Random.Range(0, cubedWorldDimensions)).GetComponent<BlockBreaking>().killBlock();
        }


        //determineBlockStates();
        StartCoroutine(determineBlockStates());


        StartCoroutine(checkIfShouldShow());


        //checkSurroundingChunks();
        StartCoroutine(checkSurroundingChunks());

        //StartCoroutine(showChunks());
        //showActiveBlocks();
    }

    private IEnumerator determineBlockStates()
    //private void determineBlockStates()
    {
        yield return new WaitForEndOfFrame();
        //0 is an inactive block
        //1 is an active block with the active flag(has been exposed)
        //2 is an inactive block with the active flag(has not been exposed)
        int blockIndex = 0;
        foreach(Transform childrenBlocks in transform.GetChild(0))
        {
            /*if (exposedBlocks[blockIndex] != 1)
            {
                if (childrenBlocks.gameObject.activeSelf)
                {
                    if (childrenBlocks.GetComponent<BlockBreaking>().activeBlock)
                    {
                        exposedBlocks[blockIndex] = 1;
                        //UnityEngine.Debug.Log("exposed block " + blockIndex);
                    }
                    else
                    {
                        UnityEngine.Debug.Log("illegal block state " + blockIndex);
                    }
                }
                else
                {
                    if (childrenBlocks.GetComponent<BlockBreaking>().activeBlock && (exposedBlocks[blockIndex] != 1 || exposedBlocks[blockIndex] != 0))
                    {
                        exposedBlocks[blockIndex] = 2;
                        //UnityEngine.Debug.Log("unexposed block " + blockIndex);
                    }
                    else
                    {
                        exposedBlocks[blockIndex] = 0;
                        //UnityEngine.Debug.Log(blockIndex);

                        if ((blockIndex + 1) < cubedWorldDimensions && exposedBlocks[blockIndex + 1] != 0)//right
                        {
                            exposedBlocks[blockIndex + 1] = 1;
                        }

                        if ((blockIndex - 1) > 0 && exposedBlocks[blockIndex - 1] != 0)//left
                        {
                            exposedBlocks[blockIndex - 1] = 1;
                        }

                        if ((blockIndex + worldDimensions) < cubedWorldDimensions && exposedBlocks[blockIndex + worldDimensions] != 0)//down
                        {
                            exposedBlocks[blockIndex + worldDimensions] = 1;
                        }

                        if ((blockIndex - worldDimensions) > 0 && exposedBlocks[blockIndex - worldDimensions] != 0)//up
                        { 
                            exposedBlocks[blockIndex -worldDimensions] = 1;
                        }

                        if ((blockIndex + squaredWorldDimensions) < cubedWorldDimensions && exposedBlocks[blockIndex + squaredWorldDimensions] != 0)//back
                        {
                            exposedBlocks[blockIndex + squaredWorldDimensions] = 1;
                        }

                        if ((blockIndex - squaredWorldDimensions) > 0 && exposedBlocks[blockIndex - squaredWorldDimensions] != 0)//forward
                        {
                            exposedBlocks[blockIndex - squaredWorldDimensions] = 1;
                        }
                    }
                }
            }*/

            if(exposedBlocks[blockIndex] == 0)
            {
                if (!(blockIndex % (worldDimensions * worldDimensions) >= worldDimensions))
                {
                    outsideBrokenBlocks["top"].Add(blockIndex);
                    //UnityEngine.Debug.Log("top" + blockIndex);
                }
                if (!((blockIndex + 1) % worldDimensions != 0 || blockIndex == 0))
                {
                    outsideBrokenBlocks["right"].Add(blockIndex);
                    //UnityEngine.Debug.Log("right" + blockIndex);
                }
                if (!(blockIndex % (worldDimensions * worldDimensions) < (worldDimensions * (worldDimensions - 1))))
                {
                    outsideBrokenBlocks["bottom"].Add(blockIndex);
                    //UnityEngine.Debug.Log("bottom" + blockIndex);
                }
                if (!((blockIndex) % worldDimensions != 0 && (blockIndex - 1) >= 0))
                {
                    outsideBrokenBlocks["left"].Add(blockIndex);
                    //UnityEngine.Debug.Log("left" + blockIndex);
                }
                if (!(blockIndex < (worldDimensions * worldDimensions * (worldDimensions - 1))))
                {
                    outsideBrokenBlocks["front"].Add(blockIndex);
                    //UnityEngine.Debug.Log("front" + blockIndex);
                }
                if (!(blockIndex >= (worldDimensions * worldDimensions)))
                {
                    outsideBrokenBlocks["back"].Add(blockIndex);
                    //UnityEngine.Debug.Log("back" + blockIndex);
                }
                //0 - worldDimensions
                //0 - worldDimensions^2
                //(worldDimensions * x) + 1
                /*
                 * top (blockChildNum % (worldDimensions * worldDimensions) >= worldDimensions)
                 * 
                 * right ((blockChildNum + 1) % worldDimensions != 0 || blockChildNum == 0)
                 * 
                 * bottom (blockChildNum % (worldDimensions * worldDimensions) < (worldDimensions * (worldDimensions - 1)))
                 * 
                 * left ((blockChildNum) % worldDimensions != 0 && (blockChildNum - 1) >= 0)
                 * 
                 * front (blockChildNum < (worldDimensions * worldDimensions * (worldDimensions - 1)))
                 * 
                 * back (blockChildNum >= (worldDimensions * worldDimensions))
                 */
                
            }

            //if(exposedBlocks[blockIndex] != 0 && exposedBlocks[blockIndex] != 2)// && childrenBlocks.transform.position.y == 0)
            if(exposedBlocks[blockIndex] == 2 && childrenBlocks.transform.position.y == 0)
            {
                //exposedBlocks[blockIndex] = 1;

                changeBlockState(blockIndex, 1);
            }

            

            //UnityEngine.Debug.Log(childrenBlocks.name + "__" + exposedBlocks[blockIndex]);

            blockIndex++;
        }
        /*if (transform.position.y == 0)
        {
            GameObject.Find("OriginPoint").GetComponent<genMobsOnChunks>().spawnMob(transform);
        }*/
    }
    private Vector3 posToCheck;
    private bool chunkTop, chunkRight, chunkBottom, chunkLeft, chunkFront, chunkBack, allChunks;
    private Transform surroundingChunkToCheck;


    private IEnumerator checkSurroundingChunks()
    //private void checkSurroundingChunks()
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
                if ((surroundingChunkToCheck = transform.parent.Find(posToCheck + "")) && surroundingChunkToCheck.gameObject.GetComponent<chunkDistanceCalc>().outsideBrokenBlocks.ContainsKey("top"))
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
                posToCheck.y -= worldDimensions;
            }
            if (!chunkRight)
            {
                posToCheck.x += worldDimensions;
                if ((surroundingChunkToCheck = transform.parent.Find(posToCheck + "")) && surroundingChunkToCheck.gameObject.GetComponent<chunkDistanceCalc>().outsideBrokenBlocks.ContainsKey("right"))
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
                posToCheck.x -= worldDimensions;
            }
            if (!chunkBottom)
            {
                posToCheck.y -= worldDimensions;
                if ((surroundingChunkToCheck = transform.parent.Find(posToCheck + "")) && surroundingChunkToCheck.gameObject.GetComponent<chunkDistanceCalc>().outsideBrokenBlocks.ContainsKey("bottom"))
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
                posToCheck.y += worldDimensions;
            }
            if (!chunkLeft)
            {
                posToCheck.x -= worldDimensions;
                if ((surroundingChunkToCheck = transform.parent.Find(posToCheck + "")) && surroundingChunkToCheck.gameObject.GetComponent<chunkDistanceCalc>().outsideBrokenBlocks.ContainsKey("left"))
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
                posToCheck.x += worldDimensions;
            }
            if (!chunkFront)
            {
                posToCheck.z += worldDimensions;
                if ((surroundingChunkToCheck = transform.parent.Find(posToCheck + "")) && surroundingChunkToCheck.gameObject.GetComponent<chunkDistanceCalc>().outsideBrokenBlocks.ContainsKey("front"))
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
                posToCheck.z -= worldDimensions;
            }
            if (!chunkBack)
            {
                posToCheck.z -= worldDimensions;
                if ((surroundingChunkToCheck = transform.parent.Find(posToCheck + "")) && surroundingChunkToCheck.gameObject.GetComponent<chunkDistanceCalc>().outsideBrokenBlocks.ContainsKey("back"))
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
                posToCheck.z += worldDimensions;
            }
            /*else
            {
                allChunks = true;
            }*/
            //yield return new WaitForSeconds(1);
        }
        //yield return new WaitForSeconds(1);
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
        //if (exposedBlocks[blockNum] != 0)
        {
            
            exposedBlocks[blockNum] = stateNum;
            //checkAroundBlocks(blockNum);
            //transform.parent.GetComponent<fixedWorldGen>().breakChunkBlockRecursive(transform.position, blockNum); DO NOT USE
        }
        //if(stateNum == 1)
        {
            /************************Temporary workaround************************/
            //display any new blocks that are now set to 1(should show)
            //showing = false;
            //showingClose = false;
        }
        if(stateNum == 0)
        {
            //exposedBlocks[blockNum] = 0;
            //checkAroundBlocks(blockNum);
            //UnityEngine.Debug.Log(blockNum);
            transform.GetChild(0).transform.GetChild(blockNum).gameObject.SetActive(false);
            breakBlockAndShowSurrounds(blockNum);
            //UnityEngine.Debug.Log(transform.GetChild(0).transform.GetChild(blockNum).gameObject.activeSelf);
        }

        showing = false;
        showingClose = false;
    }

    private void breakBlockAndShowSurrounds(int blockChildNum)
    {
        GameObject foundChunk;

        //UnityEngine.Debug.Log(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 0) + " top");
        //top
        if (blockChildNum % (worldDimensions * worldDimensions) >= worldDimensions)
        {
            //blockChildNum - worldDimensions
            if (checkChild(blockChildNum - worldDimensions) != 0)
            {
                changeBlockState(blockChildNum - worldDimensions, 1);
            }
        }
        //else if (foundChunk = transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 0)).gameObject)
        //else if (transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 0)))
        else if (GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 0) < transform.parent.childCount && GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 0) > 0)
        {
            UnityEngine.Debug.Log("broke top of " + transform.GetSiblingIndex());
            foundChunk = transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 0)).gameObject;
            UnityEngine.Debug.Log("found " + foundChunk.transform.GetSiblingIndex());
            //blockChildNum + (worldDimensions * (worldDimensions - 1))
            if (foundChunk.GetComponent<chunkDistanceCalc>().checkChild(blockChildNum + (worldDimensions * (worldDimensions - 1))) != 0)
            {
                foundChunk.GetComponent<chunkDistanceCalc>().changeBlockState(blockChildNum + (worldDimensions * (worldDimensions - 1)), 1);
            }
        }

        //UnityEngine.Debug.Log(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 1) + " right");
        //right
        if ((blockChildNum + 1) % worldDimensions != 0)// || blockChildNum == 0)
        {
            //blockChildNum + 1
            if (checkChild(blockChildNum + 1) != 0)
            {
                changeBlockState(blockChildNum + 1, 1);
            }
        }
        //else if (foundChunk = retrieveChunkByPosition(chunkPos, 1))
        //else if (foundChunk = transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 1)).gameObject)
        //else if(transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 1)))
        else if (GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 1) < transform.parent.childCount && GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 1) >= 0)
        {
            UnityEngine.Debug.Log("broke right of " + transform.GetSiblingIndex());
            foundChunk = transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 1)).gameObject;
            UnityEngine.Debug.Log("found " + foundChunk.transform.GetSiblingIndex());

            if (foundChunk.transform.position.y == transform.position.y)
            {
                //blockChildNum - (worldDimensions - 1)
                if (foundChunk.GetComponent<chunkDistanceCalc>().checkChild(blockChildNum - (worldDimensions - 1)) != 0)
                {
                    foundChunk.GetComponent<chunkDistanceCalc>().changeBlockState(blockChildNum - (worldDimensions - 1), 1);
                }
            }
        }

        //UnityEngine.Debug.Log(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 2) + " bottom");
        //bottom
        if (blockChildNum % (worldDimensions * worldDimensions) < (worldDimensions * (worldDimensions - 1)))
        {
            //blockChildNum + worldDimensions
            if (checkChild(blockChildNum + worldDimensions) != 0)
            {
                changeBlockState(blockChildNum + worldDimensions, 1);
            }
        }
        //else if (foundChunk = retrieveChunkByPosition(chunkPos, 2))
        //else if (foundChunk = transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 2)).gameObject)
        //else if (transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 2)))
        else if(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 2) < transform.parent.childCount && GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 2) >= 0)
        {
            UnityEngine.Debug.Log("broke bottom of " + transform.GetSiblingIndex());
            foundChunk = transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 2)).gameObject;
            UnityEngine.Debug.Log("found " + foundChunk.transform.GetSiblingIndex());
            //blockChildNum - (worldDimensions * (worldDimensions - 1))
            if (foundChunk.GetComponent<chunkDistanceCalc>().checkChild(blockChildNum - (worldDimensions * (worldDimensions - 1))) != 0)
            {
                foundChunk.GetComponent<chunkDistanceCalc>().changeBlockState(blockChildNum - (worldDimensions * (worldDimensions - 1)), 1);
            }
        }

        //UnityEngine.Debug.Log(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 3) + " left");
        //left
        if ((blockChildNum) % worldDimensions != 0 && (blockChildNum - 1) >= 0)
        {
            //blockChildNum - 1
            if (checkChild(blockChildNum - 1) != 0)
            {
                changeBlockState(blockChildNum - 1, 1);
            }
        }
        //else if (foundChunk = retrieveChunkByPosition(chunkPos, 3))
        //else if (foundChunk = transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 3)).gameObject)
        //else if (transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 3)))
        else if (GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 3) < transform.parent.childCount && GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 3) >= 0)
        {
            UnityEngine.Debug.Log("broke left of " + transform.GetSiblingIndex());
            foundChunk = transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 3)).gameObject;
            UnityEngine.Debug.Log("found " + foundChunk.transform.GetSiblingIndex());
            if (foundChunk.transform.position.y == transform.position.y)
            {
                //blockChildNum + worldDimensions - 1
                if (foundChunk.GetComponent<chunkDistanceCalc>().checkChild(blockChildNum + worldDimensions - 1) != 0)
                {
                    foundChunk.GetComponent<chunkDistanceCalc>().changeBlockState(blockChildNum + worldDimensions - 1, 1);
                }
            }
        }

        //UnityEngine.Debug.Log(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 4) + " front");
        //front
        if (blockChildNum < (worldDimensions * worldDimensions * (worldDimensions - 1)))
        {
            //blockChildNum + (worldDimensions * worldDimensions)
            if (checkChild(blockChildNum + (worldDimensions * worldDimensions)) != 0)
            {
                changeBlockState(blockChildNum + (worldDimensions * worldDimensions), 1);
            }
        }
        //else if (foundChunk = retrieveChunkByPosition(chunkPos, 4))
        //else if (foundChunk = transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 4)).gameObject)
        //else if (transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 4)))
        else if (GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 4) < transform.parent.childCount && GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 4) >= 0)
        {
            UnityEngine.Debug.Log("broke front of " + transform.GetSiblingIndex());
            foundChunk = transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 4)).gameObject;
            UnityEngine.Debug.Log("found " + foundChunk.transform.GetSiblingIndex());
            if (foundChunk.transform.position.y == transform.position.y)
            {
                //blockChildNum - (worldDimensions * worldDimensions * (worldDimensions - 1))
                if (foundChunk.GetComponent<chunkDistanceCalc>().checkChild(blockChildNum - (worldDimensions * worldDimensions * (worldDimensions - 1))) != 0)
                {
                    foundChunk.GetComponent<chunkDistanceCalc>().changeBlockState(blockChildNum - (worldDimensions * worldDimensions * (worldDimensions - 1)), 1);
                }
            }
        }

        //UnityEngine.Debug.Log(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 5) + " back");
        //back
        if (blockChildNum >= (worldDimensions * worldDimensions))
        {
            //blockChildNum - (worldDimensions * worldDimensions)
            if (checkChild(blockChildNum - (worldDimensions * worldDimensions)) != 0)
            {
                changeBlockState(blockChildNum - (worldDimensions * worldDimensions), 1);
            }
        }
        //else if (foundChunk = retrieveChunkByPosition(chunkPos, 5))
        //else if (foundChunk = transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 5)).gameObject)
        //else if (transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 5)))
        else if (GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 5) < transform.parent.childCount && GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 5) >= 0)
        {
            UnityEngine.Debug.Log("broke back of " + transform.GetSiblingIndex());
            foundChunk = transform.parent.transform.GetChild(GetComponentInParent<fixedWorldGen>().findChunkLoopNum(transform.GetSiblingIndex(), 5)).gameObject;
            UnityEngine.Debug.Log("found " + foundChunk.transform.GetSiblingIndex());
            if (foundChunk.transform.position.y == transform.position.y)
            {
                //blockChildNum + (worldDimensions * worldDimensions * (worldDimensions - 1))
                if (foundChunk.GetComponent<chunkDistanceCalc>().checkChild(blockChildNum + (worldDimensions * worldDimensions * (worldDimensions - 1))) != 0)
                {
                    foundChunk.GetComponent<chunkDistanceCalc>().changeBlockState(blockChildNum + (worldDimensions * worldDimensions * (worldDimensions - 1)), 1);
                }
            }
        }
    }

    private void checkAroundBlocks(int blockChildNum)
    {
        if ((blockChildNum % (worldDimensions * worldDimensions) >= worldDimensions) && (exposedBlocks[blockChildNum - worldDimensions] != 0))
        {
            exposedBlocks[blockChildNum - worldDimensions] = 1;
        }
        if ((blockChildNum + 1) % worldDimensions != 0 && (exposedBlocks[blockChildNum + 1] != 0))
        {
            exposedBlocks[blockChildNum + 1] = 1;
        }
        if ((blockChildNum % (worldDimensions * worldDimensions) < (worldDimensions * (worldDimensions - 1))) && (exposedBlocks[blockChildNum + worldDimensions] != 0))
        {
            exposedBlocks[blockChildNum + worldDimensions] = 1;
        }
        if (((blockChildNum) % worldDimensions != 0 && (blockChildNum - 1) >= 0) && (exposedBlocks[blockChildNum - 1] != 0))
        {
            exposedBlocks[blockChildNum - 1] = 1;
        }
        if ((blockChildNum < (worldDimensions * worldDimensions * (worldDimensions - 1))) && (exposedBlocks[blockChildNum + (worldDimensions * worldDimensions)] != 0))
        {
            exposedBlocks[blockChildNum + (worldDimensions * worldDimensions)] = 1;
        }
        if ((blockChildNum >= (worldDimensions * worldDimensions)) && (exposedBlocks[blockChildNum - (worldDimensions * worldDimensions)] != 0))
        {
            exposedBlocks[blockChildNum - (worldDimensions * worldDimensions)] = 1;
        }
        //if (blockChildNum % (worldDimensions* worldDimensions) >= worldDimensions)
        //if (blockChildNum + 1) % worldDimensions != 0
        //if (blockChildNum % (worldDimensions * worldDimensions) < (worldDimensions * (worldDimensions - 1)))
        //if ((blockChildNum) % worldDimensions != 0 && (blockChildNum - 1) >= 0)
        //if (blockChildNum < (worldDimensions * worldDimensions * (worldDimensions - 1)))
        //if (blockChildNum >= (worldDimensions * worldDimensions))
    }

    int viewDistanceControl = 30;

    private IEnumerator checkIfShouldShow()
    {
        while (true)
        {
            //yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(.1f);
            float chunkDistance = Vector3.Distance(transform.position, playerRef.transform.position);

            //UnityEngine.Debug.Log("near " + chunkDistance);

            if(chunkDistance < viewDistanceControl/2)
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
                    //UnityEngine.Debug.Log("near " + chunkDistance);
                    transform.GetChild(1).gameObject.SetActive(true);
                    transform.GetChild(0).gameObject.SetActive(true);
                    showActiveBlocks();
                    //yield return new WaitForSeconds(3);
                }
            }
            else// if (chunkDistance >= viewDistanceControl && chunkDistance < (viewDistanceControl * 2))
            {
                //UnityEngine.Debug.Log("far " + chunkDistance);
                if (showing)
                {
                    transform.GetChild(1).gameObject.SetActive(false);
                    transform.GetChild(0).gameObject.SetActive(false);
                    hideActiveBlocks();
                    //yield return new WaitForSeconds(1);
                }
            }
            //else if(chunkDistance >= (viewDistanceControl * 2))
            /*else
            {
                //UnityEngine.Debug.Log((chunkDistance % viewDistanceControl));
                if (showing)
                {
                    transform.GetChild(1).gameObject.SetActive(false);
                    transform.GetChild(0).gameObject.SetActive(false);
                    hideActiveBlocks();
                    //yield return new WaitForSeconds((chunkDistance % viewDistanceControl));
                }
                //yield return new WaitForSeconds(5);
            }*/

        }
    }

    /*private IEnumerator showChunks()
    {
        while(true)
        {
            yield return new WaitForSeconds(.1f);
            
            float chunkDistance = Vector3.Distance(transform.position, playerRef.transform.position);

            if (!fixedSurroundingChunks)
            {
                fixedSurroundingChunks = true;
                minChunkDistance = smallerChunkDistance;
                //GameObject.Find("OriginPoint").GetComponent<fixedWorldGen>().findAllSurroundingChunks(transform.position);
            }
            if (chunkDistance <= minChunkDistance)
            {
                showActiveBlocks();
                if (fixedSurroundingChunks)
                {
                    transform.GetChild(0).gameObject.SetActive(true);
                }

                
                {
                    transform.GetChild(1).gameObject.SetActive(true);
                }
                
            }
            else if(chunkDistance > minChunkDistance && chunkDistance <= maxChunkDistance)
            {
                hideActiveBlocks();
                transform.GetChild(0).gameObject.SetActive(false);
                {
                    transform.GetChild(1).gameObject.SetActive(false);
                }
            }
            else if(chunkDistance > maxChunkDistance)
            {
                yield return new WaitForSeconds(5);
            }
        }
    }*/

    //float waitTime = .5f;
    //bool fullChunkShowing = false;

    //int minChunkDistance = 20, maxChunkDistance = 50;
    //int smallerChunkDistance = 20;

    /*public void showMissingBlocksFromBreaking()
    {
        foreach (int blockIndex in brokenBlocks)
        {
            GameObject.Find("OriginPoint").GetComponent<fixedWorldGen>().reBreakBlocksToShowHiddenOnes(transform.position, blockIndex);
        }
    }*/

    //private bool shouldShow = false;
    //private int showingBlocksCounter = 0;
    //private int showingBlocksEndCounter = 0;

    public void showAllBlocks()
    {
        //Figure out a way to correctly hide the chunk if you are no longer in range of it
        /*if (!shouldShow)
        {
            shouldShow = true;
            StartCoroutine(checkIfChunkShouldShow());
        }
        else
        {
            StopCoroutine(checkIfChunkShouldShow());
            shouldShow = false;
        }*/
        //StopAllCoroutines();
        //StartCoroutine(showChunks());

        foreach(Transform children in transform.GetChild(0))
        {
            //if(children.GetComponent<BlockBreaking>().activeBlock)
            {
                //children.gameObject.SetActive(true);
            }
        }
        //shouldShow = false;
    }
    public void hideAllBlocks()
    {
        foreach (Transform children in transform.GetChild(0))
        {
            //if (children.GetComponent<BlockBreaking>().activeBlock)
            {
                //children.gameObject.SetActive(false);
            }
        }
    }
}
