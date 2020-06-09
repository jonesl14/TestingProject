using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class fixedWorldGen : MonoBehaviour
{
    public int worldDimensions;
    public int depthCount;
    public int loopsToPerform;
    public int numOfBlocksToBreak;


    public GameObject worldBlockTemplate;
    public GameObject worldBlockOriginPoint;
    public float originX, originY, originZ;
    private Vector3 originVector;
    public GameObject playerRef;
    public GameObject fpsControllerRef;

    Vector3 chunkPosition;

    public GameObject chunkToGen;

    Object[] preloadedBlockMaterials;
    Object[] preloadedBlockMaterialsSorted;

    List<int> preloadedBlockMaterialsNamesNumbers = new List<int> { };

    private int blockMaterialsCount = 3;

    int chunkDimensionsLocal = 3;
    int blockTextureLocal;

    private void Awake()
    {
        Physics.IgnoreLayerCollision(11, 12);
        Physics.IgnoreLayerCollision(12, 15);
    }

    private void Start()
    {
        originVector = new Vector3(originX, originY, originZ);
        chunkPosition = new Vector3(originX, originY, originZ);

        preloadedBlockMaterials = Resources.LoadAll("BlockArt/4-All/", typeof(Material));

        preloadedBlockMaterialsSorted = new Object[preloadedBlockMaterials.Length];

        foreach (object blockMat in preloadedBlockMaterials)
        {
            preloadedBlockMaterialsNamesNumbers.Add(int.Parse(blockMat.ToString().Split()[0]));
        }

        preloadedBlockMaterialsNamesNumbers.Sort();
        foreach (int blockNumber in preloadedBlockMaterialsNamesNumbers)
        {
            foreach (object blockMat in preloadedBlockMaterials)
            {
                if (int.Parse(blockMat.ToString().Split()[0]) == blockNumber)
                {
                    preloadedBlockMaterialsSorted[blockNumber] = (Object)blockMat;
                }
            }
        }
        StartCoroutine(spawnSpiralChunks());
    }

    public void buildNewListChunkImproved(GameObject originParent)//, string genDirection)
    {
        float originParentYPos = -originParent.transform.position.y;

        if (originParentYPos < chunkDimensionsLocal - 1)
        {
            blockTextureLocal = 0;
        }

        if (originParentYPos >= chunkDimensionsLocal - 1 && originParentYPos < (chunkDimensionsLocal * chunkDimensionsLocal))
        {
            blockTextureLocal = 1;
        }

        if (originParentYPos >= (chunkDimensionsLocal * chunkDimensionsLocal) && originParentYPos < (chunkDimensionsLocal * chunkDimensionsLocal * chunkDimensionsLocal))
        {
            blockTextureLocal = 2;
        }

        if (originParentYPos >= (chunkDimensionsLocal * chunkDimensionsLocal * chunkDimensionsLocal))
        {
            blockTextureLocal = 3;
        }
        foreach (Transform tempBlock in originParent.transform.GetChild(0).transform)
        {
            tempBlock.gameObject.GetComponent<Renderer>().material = (Material)preloadedBlockMaterialsSorted[((blockMaterialsCount * blockTextureLocal) + Random.Range(0, 3))];
        }
    }

    float playerDistanceFromOrigin = 0.0f;
    float lastPlayerDistanceFromOrigin = 0.0f;
    private int distanceSpawnControl = 5;

    private int spawnOption = 0;

    private void Update()
    {
        if (spawnOption == 0)
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                loopsToPerform += 1;
                resetChunkNum = false;
                StartCoroutine(spawnSpiralChunks());
            }
        }
        if (spawnOption == 1)
        {
            playerDistanceFromOrigin = Vector3.Distance(transform.position, playerRef.transform.parent.transform.localPosition);
            if (lastPlayerDistanceFromOrigin <= (playerDistanceFromOrigin - distanceSpawnControl))
            {
                lastPlayerDistanceFromOrigin = playerDistanceFromOrigin;
                loopsToPerform += 1;
                StartCoroutine(spawnSpiralChunks());
            }
        }
    }


    int loopsPerformed = 0;
    int tempLoopsPerformed = 0;

    System.DateTime startTime, endTime;
    int tempBlockNumbers = 0;
    bool resetChunkNum = true;
    int tempStoreLoopsPerformed = 0;
    IEnumerator spawnSpiralChunks()
    {
        //GameObject tempChunkHolder;
        chunkPosition = originVector;

        startTime = System.DateTime.Now;
        tempStoreLoopsPerformed = loopsPerformed;

        for (int epochCount = 0; epochCount <= depthCount; epochCount++)
        {
            if (!resetChunkNum)
            {
                int chunkDifference = ((int)(4 * Mathf.Pow(loopsToPerform, 2) - 4 * (loopsToPerform) + 1)) - ((int)(4 * Mathf.Pow(loopsToPerform - 1, 2) - 4 * (loopsToPerform - 1) + 1));
                tempBlockNumbers = (int)(4 * Mathf.Pow(loopsToPerform - 1, 2) - 4 * (loopsToPerform - 1) + 1) + ((int)(4 * Mathf.Pow(loopsToPerform - 1, 2) - 4 * (loopsToPerform - 1) + 1) * epochCount) + (chunkDifference * epochCount);
            }
            while (loopsPerformed < loopsToPerform)
            {
                chunkPosition.z = originVector.z;
                //yield return new WaitForFixedUpdate();
                if (loopsPerformed > 0)
                {
                    tempLoopsPerformed = loopsPerformed;
                    while (tempLoopsPerformed > 0)
                    {
                        chunkPosition.x += worldDimensions;//move right * number of loops along the X axis
                        chunkPosition.z -= worldDimensions;
                        tempLoopsPerformed--;
                    }

                    
                    //tempChunkHolder = Instantiate(chunkToGen, chunkPosition, transform.rotation, transform);//This will make the down-right diagonal
                    //tempChunkHolder.transform.SetSiblingIndex(tempBlockNumbers);
                    Instantiate(chunkToGen, chunkPosition, transform.rotation, transform).transform.SetSiblingIndex(tempBlockNumbers);
                    tempBlockNumbers++;

                    tempLoopsPerformed = loopsPerformed * 2;
                    while (tempLoopsPerformed > 0)
                    {
                        chunkPosition.x -= worldDimensions;//move left 1 spot then * number of loops
                        //tempChunkHolder = Instantiate(chunkToGen, chunkPosition, transform.rotation, transform);
                        //tempChunkHolder.transform.SetSiblingIndex(tempBlockNumbers);
                        Instantiate(chunkToGen, chunkPosition, transform.rotation, transform).transform.SetSiblingIndex(tempBlockNumbers);
                        tempBlockNumbers++;
                        tempLoopsPerformed--;
                    }

                    tempLoopsPerformed = loopsPerformed * 2;
                    while (tempLoopsPerformed > 0)
                    {
                        chunkPosition.z += worldDimensions;//move up 1 spot then * number of loops
                        //tempChunkHolder = Instantiate(chunkToGen, chunkPosition, transform.rotation, transform);
                        //tempChunkHolder.transform.SetSiblingIndex(tempBlockNumbers);
                        Instantiate(chunkToGen, chunkPosition, transform.rotation, transform).transform.SetSiblingIndex(tempBlockNumbers);
                        tempBlockNumbers++;
                        tempLoopsPerformed--;
                    }

                    tempLoopsPerformed = loopsPerformed * 2;
                    while (tempLoopsPerformed > 0)
                    {
                        chunkPosition.x += worldDimensions;//move right 1 spot then * number of loops
                        //tempChunkHolder = Instantiate(chunkToGen, chunkPosition, transform.rotation, transform);
                        //tempChunkHolder.transform.SetSiblingIndex(tempBlockNumbers);
                        Instantiate(chunkToGen, chunkPosition, transform.rotation, transform).transform.SetSiblingIndex(tempBlockNumbers);
                        tempBlockNumbers++;
                        tempLoopsPerformed--;
                    }

                    tempLoopsPerformed = (loopsPerformed * 2) - 1;
                    while (tempLoopsPerformed > 0)
                    {
                        chunkPosition.z -= worldDimensions;//move down * number of loops
                        //tempChunkHolder = Instantiate(chunkToGen, chunkPosition, transform.rotation, transform);
                        //tempChunkHolder.transform.SetSiblingIndex(tempBlockNumbers);
                        Instantiate(chunkToGen, chunkPosition, transform.rotation, transform).transform.SetSiblingIndex(tempBlockNumbers);
                        tempBlockNumbers++;
                        tempLoopsPerformed--;
                    }

                    tempLoopsPerformed = loopsPerformed;
                    while (tempLoopsPerformed > 0)
                    {
                        chunkPosition.x -= worldDimensions;
                        tempLoopsPerformed--;
                    }
                }
                if (loopsPerformed == 0)
                {
                    //tempChunkHolder = Instantiate(chunkToGen, chunkPosition, transform.rotation, transform);
                    //tempChunkHolder.transform.SetSiblingIndex(tempBlockNumbers);
                    Instantiate(chunkToGen, chunkPosition, transform.rotation, transform).transform.SetSiblingIndex(tempBlockNumbers);
                    tempBlockNumbers++;
                }
                loopsPerformed++;
            }
            if (depthCount > 0)
            {
                if (!resetChunkNum && loopsPerformed < loopsToPerform)
                {
                    resetChunkNum = true;
                }

                chunkPosition.y -= worldDimensions;// - (worldDimensions * 2);// epochCount);
                chunkPosition.x = originVector.x;
                chunkPosition.z = originVector.z;
                loopsPerformed = tempStoreLoopsPerformed;
            }
        }
        loopsPerformed = loopsToPerform;
        endTime = System.DateTime.Now;
        UnityEngine.Debug.Log(transform.childCount * (worldDimensions * worldDimensions * worldDimensions) + "_____" + transform.childCount);

        System.TimeSpan timeTaken = endTime - startTime;
        UnityEngine.Debug.Log(timeTaken.TotalSeconds);

        yield return new WaitForSeconds(1);
        foreach(Transform childTransform in transform)
        {
            childTransform.gameObject.GetComponent<chunkDistanceCalc>().breakBlocks();
        }
    }

    public int findChunkLoopNum(int nameNum, int direction)
    {
        int depthFactor = 0;
        int upChunk = -99, downChunk = -99, leftChunk = -99, rightChunk = -99, aboveChunk = -99, belowChunk = -99;

        int chunkBlockCount = ((int)(4 * Mathf.Pow(loopsToPerform, 2) - 4 * (loopsToPerform) + 1));

        if (nameNum > chunkBlockCount)
        {
            depthFactor = nameNum / chunkBlockCount;
            nameNum -= chunkBlockCount * depthFactor;
        }
        for (int i = 1; i <= loopsToPerform; i++)
        {
            int upLeft = (int)(1 + 4 * Mathf.Pow(i, 2));
            int upRight = (int)(1 + 2 * i + 4 * Mathf.Pow(i, 2));
            
            int downLeft = (int)(1 - 2 * i + 4 * Mathf.Pow(i, 2));
            int downRight = (int)(1 - 4 * i + 4 * Mathf.Pow(i, 2));
            int downRightPlusOne = (int)(1 - 4 * (i+1) + 4 * Mathf.Pow((i + 1), 2));

            aboveChunk = nameNum - ((int)(4 * Mathf.Pow(loopsToPerform, 2) - 4 * (loopsToPerform) + 1));
            belowChunk = nameNum + ((int)(4 * Mathf.Pow(loopsToPerform, 2) - 4 * (loopsToPerform) + 1));// + (int)(4 * Mathf.Pow(loopsToPerform - 1, 2) - 4 * (loopsToPerform - 1)));

            if(nameNum % ((int)(4 * Mathf.Pow(loopsToPerform, 2) - 4 * (loopsToPerform) + 1)) ==  0)
            {
                upChunk = nameNum + 6;
                downChunk = nameNum + 2;
                leftChunk = nameNum + 4;
                rightChunk = nameNum + 8;
                //chunks running down the middle(0 and such going down)
            }

            else if (nameNum == upLeft)
            {
                upChunk = nameNum + 13 + (8 * (i - 1));
                downChunk = nameNum - 1;
                leftChunk = nameNum + 11 + (8 * (i - 1));
                rightChunk = nameNum + 1;
                //fifth column(T)
            }
            else if (nameNum == upRight)
            {
                upChunk = nameNum + 13 + (8 * (i - 1));
                downChunk = nameNum + 1;
                leftChunk = nameNum - 1;
                rightChunk = nameNum + 15 + (8 * (i - 1));
                //sixth column(U)
            }
            else if (nameNum == downLeft)
            {
                upChunk = nameNum + 1;
                downChunk = nameNum + 9 + (8 * (i - 1));
                leftChunk = nameNum + 11 + (8 * (i - 1));
                rightChunk = nameNum - 1;
                //seventh column(V)
            }
            else if (nameNum == downRight)
            {
                if(nameNum == 1)
                {
                    upChunk = nameNum + 7;//8;
                    rightChunk = nameNum + 15 + (8 * (i));
                }
                else
                {
                    upChunk = nameNum - 1;
                    rightChunk = nameNum + 15 + (8 * (i - 1));
                }
                downChunk = nameNum + 9 + (8 * (i - 1));
                leftChunk = nameNum + 1;
                //eighth column(W)
            }

            else if (nameNum > upLeft && nameNum < upRight)//Top side
            {
                upChunk = nameNum + 13 + (8 * (i - 1));
                downChunk = nameNum - 5 - (8 * (i - 1));
                if (i == 1)
                    downChunk -= 1;
                leftChunk = nameNum - 1;
                rightChunk = nameNum + 1;
                //first column(P)
            }
            else if (nameNum < upLeft && nameNum > downLeft)//Left side
            {
                upChunk = nameNum + 1;
                downChunk = nameNum - 1;
                leftChunk = nameNum + 11 + (8 * (i - 1));
                rightChunk = nameNum - 3 - (8 * (i - 1));
                if (i == 1)
                    rightChunk -= 1;
                //second column(Q)
            }
            else if (nameNum > upRight && nameNum < downRightPlusOne)//Right side
            {//Will require extra work
                upChunk = nameNum - 1;
                if (nameNum + 1 == (int)(1 - 4 * (i + 1) + 4 * Mathf.Pow((i + 1), 2)))
                {
                    downChunk = downRight;
                    leftChunk = nameNum - (7 + (8*(i-2)))  - (8 * i);
                    if (i == 1)
                        leftChunk -= 1;
                }
                else
                {
                    downChunk = nameNum + 1;
                    leftChunk = nameNum - 15 - (8 * (i - 2));
                }
                rightChunk = nameNum + 15 + (8 * (i - 1));
                //third column(R)
            }
            else if (nameNum < downLeft && nameNum > downRight)//Bottom side
            {
                upChunk = nameNum - 1 - (8 * (i-1));
                if (i == 1)
                    upChunk -= 1;
                downChunk = nameNum + 9 + (8 * (i - 1));
                leftChunk = nameNum + 1;
                rightChunk = nameNum - 1;
                //fourth column(S)
            }
        }
        switch (direction)
        {
            case 0://top
                //calculate top chunk
                if (aboveChunk != -99)
                    return aboveChunk + (chunkBlockCount * depthFactor);
                break;
            case 1://right
                if (rightChunk != -99)
                    return rightChunk + (chunkBlockCount * depthFactor);
                break;
            case 2://bottom
                if (belowChunk != -99)
                    return belowChunk + (chunkBlockCount * depthFactor);
                //calculate bottom chunk
                break;
            case 3://left
                if (leftChunk != -99)
                    return leftChunk + (chunkBlockCount * depthFactor);
                break;
            case 4://front
                if (upChunk != -99)
                    return upChunk + (chunkBlockCount * depthFactor);
                break;
            case 5://back
                if (downChunk != -99)
                    return downChunk + (chunkBlockCount * depthFactor);
                break;
        }
        return -99;//desired direction wasn't found
    }
}