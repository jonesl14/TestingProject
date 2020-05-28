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


    public GameObject worldBlockTemplate;
    public GameObject worldBlockOriginPoint;
    public float originX, originY, originZ;
    private Vector3 originVector;
    public GameObject playerRef;
    //private bool firstRun = true;

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

        //StartCoroutine(spawnOnlyChunksThatAreNeeded());
        StartCoroutine(spawnSpiralChunks());

        //StartCoroutine(globalShowChunks());
        
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

            /*if (tempBlock.position.y >= 0 && tempBlock.gameObject.GetComponent<BlockBreaking>().activeBlock)
            {
                tempBlock.gameObject.SetActive(true);
            }*/
        }
    }

    public GameObject retrieveChunkByPosition(Vector3 chunkPos, int direction)
    {
        switch (direction)
        {
            case 0:
                chunkPos.y += worldDimensions;
                if (transform.Find(chunkPos + ""))//top
                {
                    return transform.Find(chunkPos + "").gameObject;
                }
                chunkPos.y -= worldDimensions;
                break;

            case 1:
                chunkPos.x += worldDimensions;
                if (transform.Find(chunkPos + ""))//right
                {
                    return transform.Find(chunkPos + "").gameObject;
                }
                chunkPos.x -= worldDimensions;
                break;

            case 2:
                chunkPos.y -= worldDimensions;
                if (transform.Find(chunkPos + ""))//bottom
                {
                    return transform.Find(chunkPos + "").gameObject;
                }
                chunkPos.y += worldDimensions;
                break;

            case 3:
                chunkPos.x -= worldDimensions;
                if (transform.Find(chunkPos + ""))//left
                {
                    return transform.Find(chunkPos + "").gameObject;
                }
                chunkPos.x += worldDimensions;
                break;

            case 4:
                chunkPos.z += worldDimensions;
                if (transform.Find(chunkPos + ""))//front
                {
                    return transform.Find(chunkPos + "").gameObject;
                }
                chunkPos.z -= worldDimensions;
                break;

            case 5:
                chunkPos.z -= worldDimensions;
                if (transform.Find(chunkPos + ""))//back
                {
                    return transform.Find(chunkPos + "").gameObject;
                }
                chunkPos.z += worldDimensions;
                break;
        }

        return null;
    }

    //private List<GameObject> blocksToBeBroken = new List<GameObject>();

    /*public void breakChunkBlockByPosition2(Vector3 chunkPos, int blockChildNum)//changeNeed
    {
        Transform chunkBlockToBeBroken = transform.Find(chunkPos + "").transform;

        //top
        if (blockChildNum % (worldDimensions * worldDimensions) >= worldDimensions)
        {
            if (chunkBlockToBeBroken.GetChild(0).transform.GetChild(blockChildNum - worldDimensions).GetComponent<BlockBreaking>().activeBlock)
            {
                chunkBlockToBeBroken.GetChild(0).transform.GetChild(blockChildNum - worldDimensions).gameObject.SetActive(true);
            }
            else
            {
                chunkBlockToBeBroken.GetChild(0).transform.GetChild(blockChildNum - worldDimensions).GetComponent<BlockBreaking>().instaKillBlock2();
            }
        }
        else if (retrieveChunkByPosition(chunkPos, 0))
        {
            if (retrieveChunkByPosition(chunkPos, 0).transform.GetChild(0).transform.GetChild(blockChildNum + (worldDimensions * (worldDimensions - 1))).GetComponent<BlockBreaking>().activeBlock)
            {
                retrieveChunkByPosition(chunkPos, 0).transform.GetChild(0).transform.GetChild(blockChildNum + (worldDimensions * (worldDimensions - 1))).gameObject.SetActive(true);
            }
            else
            {
                retrieveChunkByPosition(chunkPos, 0).transform.GetChild(0).transform.GetChild(blockChildNum + (worldDimensions * (worldDimensions - 1))).GetComponent<BlockBreaking>().instaKillBlock2();
            }
        }

        //right
        if ((blockChildNum + 1) % worldDimensions != 0 || blockChildNum == 0)
        {
            if (chunkBlockToBeBroken.GetChild(0).transform.GetChild(blockChildNum + 1).GetComponent<BlockBreaking>().activeBlock)
            {
                chunkBlockToBeBroken.GetChild(0).transform.GetChild(blockChildNum + 1).gameObject.SetActive(true);
            }
            else
            {
                chunkBlockToBeBroken.GetChild(0).transform.GetChild(blockChildNum + 1).GetComponent<BlockBreaking>().instaKillBlock2();
            }
        }
        else if (retrieveChunkByPosition(chunkPos, 1))
        {
            if (retrieveChunkByPosition(chunkPos, 1).transform.GetChild(0).transform.GetChild(blockChildNum - (worldDimensions - 1)).GetComponent<BlockBreaking>().activeBlock)
            {
                retrieveChunkByPosition(chunkPos, 1).transform.GetChild(0).transform.GetChild(blockChildNum - (worldDimensions - 1)).gameObject.SetActive(true);
            }
            else
            {
                retrieveChunkByPosition(chunkPos, 1).transform.GetChild(0).transform.GetChild(blockChildNum - (worldDimensions - 1)).GetComponent<BlockBreaking>().instaKillBlock2();
            }
        }

        //bottom
        if (blockChildNum % (worldDimensions * worldDimensions) < (worldDimensions * (worldDimensions - 1)))
        {
            if (chunkBlockToBeBroken.GetChild(0).transform.GetChild(blockChildNum + worldDimensions).GetComponent<BlockBreaking>().activeBlock)
            {
                chunkBlockToBeBroken.GetChild(0).transform.GetChild(blockChildNum + worldDimensions).gameObject.SetActive(true);
            }
            else
            {
                chunkBlockToBeBroken.GetChild(0).transform.GetChild(blockChildNum + worldDimensions).GetComponent<BlockBreaking>().instaKillBlock2();
            }
        }
        else if (retrieveChunkByPosition(chunkPos, 2))
        {
            if (retrieveChunkByPosition(chunkPos, 2).transform.GetChild(0).transform.GetChild(blockChildNum - (worldDimensions * (worldDimensions - 1))).GetComponent<BlockBreaking>().activeBlock)
            {
                retrieveChunkByPosition(chunkPos, 2).transform.GetChild(0).transform.GetChild(blockChildNum - (worldDimensions * (worldDimensions - 1))).gameObject.SetActive(true);
            }
            else
            {
                retrieveChunkByPosition(chunkPos, 2).transform.GetChild(0).transform.GetChild(blockChildNum - (worldDimensions * (worldDimensions - 1))).GetComponent<BlockBreaking>().instaKillBlock2();
            }
        }

        //left
        if ((blockChildNum) % worldDimensions != 0 && (blockChildNum - 1) >= 0)
        {
            if (chunkBlockToBeBroken.GetChild(0).transform.GetChild(blockChildNum - 1).GetComponent<BlockBreaking>().activeBlock)
            {
                chunkBlockToBeBroken.GetChild(0).transform.GetChild(blockChildNum - 1).gameObject.SetActive(true);
            }
            else
            {
                chunkBlockToBeBroken.GetChild(0).transform.GetChild(blockChildNum - 1).GetComponent<BlockBreaking>().instaKillBlock2();
            }
        }
        else if (retrieveChunkByPosition(chunkPos, 3))
        {
            if (retrieveChunkByPosition(chunkPos, 3).transform.GetChild(0).transform.GetChild(blockChildNum + worldDimensions - 1).GetComponent<BlockBreaking>().activeBlock)
            {
                retrieveChunkByPosition(chunkPos, 3).transform.GetChild(0).transform.GetChild(blockChildNum + worldDimensions - 1).gameObject.SetActive(true);
            }
            else
            {
                retrieveChunkByPosition(chunkPos, 3).transform.GetChild(0).transform.GetChild(blockChildNum + worldDimensions - 1).GetComponent<BlockBreaking>().instaKillBlock2();
            }
        }

        //front
        if (blockChildNum < (worldDimensions * worldDimensions * (worldDimensions - 1)))
        {
            if (chunkBlockToBeBroken.GetChild(0).transform.GetChild(blockChildNum + (worldDimensions * worldDimensions)).GetComponent<BlockBreaking>().activeBlock)
            {
                chunkBlockToBeBroken.GetChild(0).transform.GetChild(blockChildNum + (worldDimensions * worldDimensions)).gameObject.SetActive(true);
            }
            else
            {
                chunkBlockToBeBroken.GetChild(0).transform.GetChild(blockChildNum + (worldDimensions * worldDimensions)).GetComponent<BlockBreaking>().instaKillBlock2();
            }
        }
        else if (retrieveChunkByPosition(chunkPos, 4))
        {
            if (retrieveChunkByPosition(chunkPos, 4).transform.GetChild(0).transform.GetChild(blockChildNum - (worldDimensions * worldDimensions * (worldDimensions - 1))).GetComponent<BlockBreaking>().activeBlock)
            {
                retrieveChunkByPosition(chunkPos, 4).transform.GetChild(0).transform.GetChild(blockChildNum - (worldDimensions * worldDimensions * (worldDimensions - 1))).gameObject.SetActive(true);
            }
            else
            {
                retrieveChunkByPosition(chunkPos, 4).transform.GetChild(0).transform.GetChild(blockChildNum - (worldDimensions * worldDimensions * (worldDimensions - 1))).GetComponent<BlockBreaking>().instaKillBlock2();
            }
        }

        //back
        if (blockChildNum >= (worldDimensions * worldDimensions))
        {
            if (chunkBlockToBeBroken.GetChild(0).transform.GetChild(blockChildNum - (worldDimensions * worldDimensions)).GetComponent<BlockBreaking>().activeBlock)
            {
                chunkBlockToBeBroken.GetChild(0).transform.GetChild(blockChildNum - (worldDimensions * worldDimensions)).gameObject.SetActive(true);
            }
            else
            {
                chunkBlockToBeBroken.GetChild(0).transform.GetChild(blockChildNum - (worldDimensions * worldDimensions)).GetComponent<BlockBreaking>().instaKillBlock2();
            }
        }
        else if (retrieveChunkByPosition(chunkPos, 5))
        {
            if (retrieveChunkByPosition(chunkPos, 5).transform.GetChild(0).transform.GetChild(blockChildNum + (worldDimensions * worldDimensions * (worldDimensions - 1))).GetComponent<BlockBreaking>().activeBlock)
            {
                retrieveChunkByPosition(chunkPos, 5).transform.GetChild(0).transform.GetChild(blockChildNum + (worldDimensions * worldDimensions * (worldDimensions - 1))).gameObject.SetActive(true);
            }
            else
            {
                retrieveChunkByPosition(chunkPos, 5).transform.GetChild(0).transform.GetChild(blockChildNum + (worldDimensions * worldDimensions * (worldDimensions - 1))).GetComponent<BlockBreaking>().instaKillBlock2();
            }
        }
    }*/

    public void breakChunkBlockRecursive(Vector3 chunkPos, int blockChildNum)//changeNeed
    {
        Transform chunkBlockToBeBroken = transform.Find(chunkPos + "").transform;
        GameObject foundChunk;
        //Recursion may be added to the else if statements that are looking at adjacent chunks

        //top
        if (blockChildNum % (worldDimensions * worldDimensions) >= worldDimensions)
        {
            //blockChildNum - worldDimensions
            if (chunkBlockToBeBroken.GetComponent<chunkDistanceCalc>().checkChild(blockChildNum - worldDimensions) != 0)
            {
                chunkBlockToBeBroken.GetComponent<chunkDistanceCalc>().changeBlockState(blockChildNum - worldDimensions, 1);
            }
        }
        else if (foundChunk = retrieveChunkByPosition(chunkPos, 0))
        {
            //blockChildNum + (worldDimensions * (worldDimensions - 1))
            if (foundChunk.GetComponent<chunkDistanceCalc>().checkChild(blockChildNum + (worldDimensions * (worldDimensions - 1))) != 0)
            {
                foundChunk.GetComponent<chunkDistanceCalc>().changeBlockState(blockChildNum + (worldDimensions * (worldDimensions - 1)), 1);
            }
        }

        //right
        if ((blockChildNum + 1) % worldDimensions != 0)// || blockChildNum == 0)
        {
            //blockChildNum + 1
            if (chunkBlockToBeBroken.GetComponent<chunkDistanceCalc>().checkChild(blockChildNum + 1) != 0)
            {
                chunkBlockToBeBroken.GetComponent<chunkDistanceCalc>().changeBlockState(blockChildNum + 1, 1);
            }
        }
        else if (foundChunk = retrieveChunkByPosition(chunkPos, 1))
        {
            //blockChildNum - (worldDimensions - 1)
            if (foundChunk.GetComponent<chunkDistanceCalc>().checkChild(blockChildNum - (worldDimensions - 1)) != 0)
            {
                foundChunk.GetComponent<chunkDistanceCalc>().changeBlockState(blockChildNum - (worldDimensions - 1), 1);
            }
        }

        //bottom
        if (blockChildNum % (worldDimensions * worldDimensions) < (worldDimensions * (worldDimensions - 1)))
        {
            //blockChildNum + worldDimensions
            if (chunkBlockToBeBroken.GetComponent<chunkDistanceCalc>().checkChild(blockChildNum + worldDimensions) != 0)
            {
                chunkBlockToBeBroken.GetComponent<chunkDistanceCalc>().changeBlockState(blockChildNum + worldDimensions, 1);
            }
        }
        else if (foundChunk = retrieveChunkByPosition(chunkPos, 2))
        {
            //blockChildNum - (worldDimensions * (worldDimensions - 1))
            if (foundChunk.GetComponent<chunkDistanceCalc>().checkChild(blockChildNum - (worldDimensions * (worldDimensions - 1))) != 0)
            {
                foundChunk.GetComponent<chunkDistanceCalc>().changeBlockState(blockChildNum - (worldDimensions * (worldDimensions - 1)), 1);
            }
        }

        //left
        if ((blockChildNum) % worldDimensions != 0 && (blockChildNum - 1) >= 0)
        {
            //blockChildNum - 1
            if (chunkBlockToBeBroken.GetComponent<chunkDistanceCalc>().checkChild(blockChildNum - 1) != 0)
            {
                chunkBlockToBeBroken.GetComponent<chunkDistanceCalc>().changeBlockState(blockChildNum - 1, 1);
            }
        }
        else if (foundChunk = retrieveChunkByPosition(chunkPos, 3))
        {
            //blockChildNum + worldDimensions - 1
            if (foundChunk.GetComponent<chunkDistanceCalc>().checkChild(blockChildNum + worldDimensions - 1) != 0)
            {
                foundChunk.GetComponent<chunkDistanceCalc>().changeBlockState(blockChildNum + worldDimensions - 1, 1);
            }
        }

        //front
        if (blockChildNum < (worldDimensions * worldDimensions * (worldDimensions - 1)))
        {
            //blockChildNum + (worldDimensions * worldDimensions)
            if (chunkBlockToBeBroken.GetComponent<chunkDistanceCalc>().checkChild(blockChildNum + (worldDimensions * worldDimensions)) != 0)
            {
                chunkBlockToBeBroken.GetComponent<chunkDistanceCalc>().changeBlockState(blockChildNum + (worldDimensions * worldDimensions), 1);
            }
        }
        else if (foundChunk = retrieveChunkByPosition(chunkPos, 4))
        {
            //blockChildNum - (worldDimensions * worldDimensions * (worldDimensions - 1))
            if (foundChunk.GetComponent<chunkDistanceCalc>().checkChild(blockChildNum - (worldDimensions * worldDimensions * (worldDimensions - 1))) != 0)
            {
                foundChunk.GetComponent<chunkDistanceCalc>().changeBlockState(blockChildNum - (worldDimensions * worldDimensions * (worldDimensions - 1)), 1);
            }
        }

        //back
        if (blockChildNum >= (worldDimensions * worldDimensions))
        {
            //blockChildNum - (worldDimensions * worldDimensions)
            if (chunkBlockToBeBroken.GetComponent<chunkDistanceCalc>().checkChild(blockChildNum - (worldDimensions * worldDimensions)) != 0)
            {
                chunkBlockToBeBroken.GetComponent<chunkDistanceCalc>().changeBlockState(blockChildNum - (worldDimensions * worldDimensions), 1);
            }
        }
        else if (foundChunk = retrieveChunkByPosition(chunkPos, 5))
        {
            //blockChildNum + (worldDimensions * worldDimensions * (worldDimensions - 1))
            if (foundChunk.GetComponent<chunkDistanceCalc>().checkChild(blockChildNum + (worldDimensions * worldDimensions * (worldDimensions - 1))) != 0)
            {
                foundChunk.GetComponent<chunkDistanceCalc>().changeBlockState(blockChildNum + (worldDimensions * worldDimensions * (worldDimensions - 1)), 1);
            }
        }
    }


    float playerDistanceFromOrigin = 0.0f;
    float lastPlayerDistanceFromOrigin = 0.0f;
    private int distanceSpawnControl = 5;

    //public GameObject centreRef;

    private int spawnOption = 0;
    /// <summary>
    /// Using option1 is now viable as the chunks spawn much faster
    /// </summary>

    private void Update()
    {
        if (spawnOption == 0)
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                loopsToPerform += 1;
                resetChunkNum = false;
                StartCoroutine(spawnSpiralChunks());
                //StartCoroutine(spawnOnlyChunksThatAreNeeded());
            }
        }

        //playerDistanceFromOrigin = Vector3.Distance(transform.position, playerRef.transform.position);
        //playerDistanceFromOrigin = Vector3.Distance(centreRef.transform.position, playerRef.transform.parent.transform.position);
        if (spawnOption == 1)
        {
            playerDistanceFromOrigin = Vector3.Distance(transform.position, playerRef.transform.parent.transform.localPosition);
            if (lastPlayerDistanceFromOrigin <= (playerDistanceFromOrigin - distanceSpawnControl))
            {
                lastPlayerDistanceFromOrigin = playerDistanceFromOrigin;
                loopsToPerform += 1;
                StartCoroutine(spawnOnlyChunksThatAreNeeded());
            }
        }
    }


    
    int loopsPerformed = 0;
    int tempLoopsPerformed = 0;

    System.DateTime startTime, endTime;

    /// <summary>
    /// This method will spawn chunks around the origin point in a spiral pattern that starts
    /// by moving right, then down, then left, then up and finally down to meet the first chunk
    /// '0' is the origin
    /// 6   7   8  
    /// 5   0   1
    /// 4   3   2
    /// This spiral pattern will be built '3' layers with '2' being placed directly under the first layer
    /// </summary>
    /// <returns></returns>
    /// 



    int tempBlockNumbers = 0;
    IEnumerator spawnOnlyChunksThatAreNeeded()
    {
        GameObject tempChunkHolder;// = null;
        void buildChunksDown()
        {
            for (int epochCount = 0; epochCount < depthCount; epochCount++)
            {
                chunkPosition.y -= worldDimensions;
                //tempChunkHolder = null;
                tempChunkHolder = Instantiate(chunkToGen, chunkPosition, transform.rotation, worldBlockOriginPoint.transform);
                //tempChunkHolder.name = chunkPosition + "";
                tempChunkHolder.name = tempBlockNumbers + "";
                tempBlockNumbers++;
//                buildNewListChunkImproved(tempChunkHolder);
            }
            chunkPosition.y += worldDimensions * depthCount;
        }
        chunkPosition = originVector;

        startTime = System.DateTime.Now;

        //UnityEngine.Debug.Log("Start");
        //yield return new WaitForFixedUpdate();
        while (loopsPerformed < loopsToPerform)
        {
            //UnityEngine.Debug.Log(loopsToPerform - loopsPerformed);

            // X = worldDimensions  y = depthCount  z = loopsToPerform      tested at 5 5 10
            //yield return new WaitForEndOfFrame();       // ~10 seconds      ~19 seconds
            yield return new WaitForFixedUpdate();      // ~ 10 seconds   ~12 seconds
            //yield return new WaitForSeconds(0);         // ~10 seconds      ~20 seconds


            //yield return new WaitForSeconds(3);
            


            if (loopsPerformed > 0)
            {
                tempLoopsPerformed = loopsPerformed;
                while (tempLoopsPerformed > 0)
                {
                    chunkPosition.x += worldDimensions;//move right * number of loops along the X axis
                    tempLoopsPerformed--;
                }

                //yield return new WaitForSeconds(.2f);
                tempLoopsPerformed = loopsPerformed;
                while (tempLoopsPerformed > 0)
                {
                    chunkPosition.z -= worldDimensions;//move down * number of loops
                    //tempChunkHolder = null;
                    tempChunkHolder = Instantiate(chunkToGen, chunkPosition, transform.rotation, worldBlockOriginPoint.transform);
                    //tempChunkHolder.name = chunkPosition + "";
                    tempChunkHolder.name = tempBlockNumbers + "";
                    tempBlockNumbers++;

                    //                    buildNewListChunkImproved(tempChunkHolder);
                    tempLoopsPerformed--;

                    buildChunksDown();
                }

                //yield return new WaitForSeconds(.2f);
                tempLoopsPerformed = loopsPerformed*2;
                while (tempLoopsPerformed > 0)
                {
                    chunkPosition.x -= worldDimensions;//move left 1 spot then * number of loops
                    //tempChunkHolder = null;
                    tempChunkHolder = Instantiate(chunkToGen, chunkPosition, transform.rotation, worldBlockOriginPoint.transform);
                    //tempChunkHolder.name = chunkPosition + "";
                    tempChunkHolder.name = tempBlockNumbers + "";
                    tempBlockNumbers++;

                    //                    buildNewListChunkImproved(tempChunkHolder);
                    tempLoopsPerformed--;

                    buildChunksDown();
                }

                //yield return new WaitForSeconds(.2f);
                tempLoopsPerformed = loopsPerformed*2;
                while (tempLoopsPerformed > 0)
                {
                    chunkPosition.z += worldDimensions;//move up 1 spot then * number of loops
                    //tempChunkHolder = null;
                    tempChunkHolder = Instantiate(chunkToGen, chunkPosition, transform.rotation, worldBlockOriginPoint.transform);
                    //tempChunkHolder.name = chunkPosition + "";
                    tempChunkHolder.name = tempBlockNumbers + "";
                    tempBlockNumbers++;

                    //                    buildNewListChunkImproved(tempChunkHolder);
                    tempLoopsPerformed--;

                    buildChunksDown();
                }

                //yield return new WaitForSeconds(.2f);
                tempLoopsPerformed = loopsPerformed*2;
                while (tempLoopsPerformed > 0)
                {
                    chunkPosition.x += worldDimensions;//move right 1 spot then * number of loops
                    //tempChunkHolder = null;
                    tempChunkHolder = Instantiate(chunkToGen, chunkPosition, transform.rotation, worldBlockOriginPoint.transform);
                    //tempChunkHolder.name = chunkPosition + "";
                    tempChunkHolder.name = tempBlockNumbers + "";
                    tempBlockNumbers++;

                    //                    buildNewListChunkImproved(tempChunkHolder);
                    tempLoopsPerformed--;

                    buildChunksDown();
                }

                //yield return new WaitForSeconds(.2f);
                tempLoopsPerformed = loopsPerformed;
                //while (tempLoopsPerformed-1 > 0)
                while (tempLoopsPerformed > 0)
                {
                    chunkPosition.z -= worldDimensions;//move down * number of loops
                    //tempChunkHolder = null;
                    tempChunkHolder = Instantiate(chunkToGen, chunkPosition, transform.rotation, worldBlockOriginPoint.transform);
                    //tempChunkHolder.name = chunkPosition + "";
                    tempChunkHolder.name = tempBlockNumbers + "";
                    tempBlockNumbers++;

                    //                    buildNewListChunkImproved(tempChunkHolder);
                    tempLoopsPerformed--;

                    buildChunksDown();
                }

                //yield return new WaitForSeconds(.2f);
                tempLoopsPerformed = loopsPerformed;
                while (tempLoopsPerformed > 0)
                {
                    chunkPosition.x -= worldDimensions;
                    tempLoopsPerformed--;
                }
            }
            if(loopsPerformed == 0)
            {
                //tempChunkHolder = null;
                tempChunkHolder = Instantiate(chunkToGen, chunkPosition, transform.rotation, worldBlockOriginPoint.transform);
                //tempChunkHolder.name = chunkPosition + "";
                tempChunkHolder.name = tempBlockNumbers + "";
                tempBlockNumbers++;

                //                buildNewListChunkImproved(tempChunkHolder);
                buildChunksDown();
            }
            loopsPerformed++;
        }

        endTime = System.DateTime.Now;
        UnityEngine.Debug.Log(transform.childCount * (worldDimensions * worldDimensions * worldDimensions));

        System.TimeSpan timeTaken = endTime - startTime;
        UnityEngine.Debug.Log(timeTaken.TotalSeconds);
        //UnityEngine.Debug.Log(startTime.ToString() + "__" + endTime.ToString());
        
        /*if (firstRun)
        {
            //1 + (2 * (LoopsToPerform-1))  = slice width of world
            //1 + depthCount                = depth of world
            //UnityEngine.Debug.Log(1 + (2 * (loopsToPerform - 1)));

            firstRun = false;
            //GameObject.Find("GameObjects").SetActive(true);
            playerRef.GetComponent<Camera>().cullingMask = -1;
        }*/
        //UnityEngine.Debug.Log(playerRef.GetComponent<Camera>().cullingMask);
    }

    //int oldLoopsToPerform = -1;
    bool resetChunkNum = true;
    //int tempStoreLoopsToPerform = 0;
    int tempStoreLoopsPerformed = 0;
    IEnumerator spawnSpiralChunks()
    {
        GameObject tempChunkHolder;
        /*void buildChunksDown()
        {
            for (int epochCount = 0; epochCount < depthCount; epochCount++)
            {
                chunkPosition.y -= worldDimensions;
                tempChunkHolder = Instantiate(chunkToGen, chunkPosition, transform.rotation, worldBlockOriginPoint.transform);
                tempChunkHolder.transform.SetSiblingIndex((int)(tempBlockNumbers + (4 * Mathf.Pow(loopsToPerform, 2) - 4 * (loopsToPerform) + 1)));
                tempChunkHolder.name = (int)(tempBlockNumbers + (4 * Mathf.Pow(loopsToPerform, 2) - 4 * (loopsToPerform) + 1)) + "";
                tempBlockNumbers++;
            }
            chunkPosition.y += worldDimensions * depthCount;
        }*/
        chunkPosition = originVector;

        startTime = System.DateTime.Now;
        //tempStoreLoopsToPerform = loopsToPerform;
        tempStoreLoopsPerformed = loopsPerformed;

        for (int epochCount = 0; epochCount <= depthCount; epochCount++)
        {
            while (loopsPerformed < loopsToPerform)
            {
                chunkPosition.z = originVector.z;
                yield return new WaitForFixedUpdate();
                if (loopsPerformed > 0)
                {
                    tempLoopsPerformed = loopsPerformed;
                    while (tempLoopsPerformed > 0)
                    {
                        chunkPosition.x += worldDimensions;//move right * number of loops along the X axis
                        chunkPosition.z -= worldDimensions;
                        tempLoopsPerformed--;
                    }

                    if(!resetChunkNum)
                    {
                        //resetChunkNum = true;
                        int chunkDifference = ((int)(4 * Mathf.Pow(loopsToPerform, 2) - 4 * (loopsToPerform) + 1)) - ((int)(4 * Mathf.Pow(loopsToPerform - 1, 2) - 4 * (loopsToPerform - 1) + 1));
                        tempBlockNumbers = (int)(4 * Mathf.Pow(loopsToPerform-1, 2) - 4 * (loopsToPerform-1) + 1) + ((int)(4 * Mathf.Pow(loopsToPerform - 1, 2) - 4 * (loopsToPerform - 1) + 1) * epochCount) + (chunkDifference*epochCount);
                        //UnityEngine.Debug.Log(tempBlockNumbers + " depth " + epochCount);
                    }
                    /*if(resetChunkNum && tempBlockNumbers == (int)(4 * Mathf.Pow(loopsToPerform, 2) - 4 * (loopsToPerform) + 1))
                    {
                        tempBlockNumbers = (int)(4 * Mathf.Pow(loopsToPerform, 2) - 4 * (loopsToPerform) +1) + (int)(4 * Mathf.Pow(loopsToPerform-1, 2) - 4 * (loopsToPerform-1) +1);
                    }*/

                    tempChunkHolder = Instantiate(chunkToGen, chunkPosition, transform.rotation, worldBlockOriginPoint.transform);//This will make the down-right diagonal
                    tempChunkHolder.transform.SetSiblingIndex(tempBlockNumbers);
                    //tempChunkHolder.name = tempBlockNumbers + "";
                    tempBlockNumbers++;
                    //buildChunksDown();

                    tempLoopsPerformed = loopsPerformed * 2;
                    while (tempLoopsPerformed > 0)
                    {
                        chunkPosition.x -= worldDimensions;//move left 1 spot then * number of loops
                        tempChunkHolder = Instantiate(chunkToGen, chunkPosition, transform.rotation, worldBlockOriginPoint.transform);
                        tempChunkHolder.transform.SetSiblingIndex(tempBlockNumbers);
                        //tempChunkHolder.name = tempBlockNumbers + "";
                        tempBlockNumbers++;
                        tempLoopsPerformed--;
                        //buildChunksDown();
                    }

                    tempLoopsPerformed = loopsPerformed * 2;
                    while (tempLoopsPerformed > 0)
                    {
                        chunkPosition.z += worldDimensions;//move up 1 spot then * number of loops
                        tempChunkHolder = Instantiate(chunkToGen, chunkPosition, transform.rotation, worldBlockOriginPoint.transform);
                        tempChunkHolder.transform.SetSiblingIndex(tempBlockNumbers);
                        //tempChunkHolder.name = tempBlockNumbers + "";
                        tempBlockNumbers++;
                        tempLoopsPerformed--;
                        //buildChunksDown();
                    }

                    tempLoopsPerformed = loopsPerformed * 2;
                    while (tempLoopsPerformed > 0)
                    {
                        chunkPosition.x += worldDimensions;//move right 1 spot then * number of loops
                        tempChunkHolder = Instantiate(chunkToGen, chunkPosition, transform.rotation, worldBlockOriginPoint.transform);
                        tempChunkHolder.transform.SetSiblingIndex(tempBlockNumbers);
                        //tempChunkHolder.name = tempBlockNumbers + "";
                        tempBlockNumbers++;
                        tempLoopsPerformed--;
                        //buildChunksDown();
                    }

                    tempLoopsPerformed = (loopsPerformed * 2) - 1;
                    while (tempLoopsPerformed > 0)
                    {
                        chunkPosition.z -= worldDimensions;//move down * number of loops
                        tempChunkHolder = Instantiate(chunkToGen, chunkPosition, transform.rotation, worldBlockOriginPoint.transform);
                        tempChunkHolder.transform.SetSiblingIndex(tempBlockNumbers);
                        //tempChunkHolder.name = tempBlockNumbers + "";
                        //findChunkLoopNum(tempBlockNumbers, 0);
                        tempBlockNumbers++;
                        tempLoopsPerformed--;
                        //buildChunksDown();
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
                    tempChunkHolder = Instantiate(chunkToGen, chunkPosition, transform.rotation, worldBlockOriginPoint.transform);
                    tempChunkHolder.transform.SetSiblingIndex(tempBlockNumbers);
                    //tempChunkHolder.name = tempBlockNumbers + "";
                    tempBlockNumbers++;
                    //buildChunksDown();
                }
                loopsPerformed++;
            }
            if (depthCount > 0)
            {
                //chunkPosition = originVector;
                //chunkPosition.y = originVector.y;
                if (!resetChunkNum && loopsPerformed < loopsToPerform)
                {
                    resetChunkNum = true;
                    //tempBlockNumbers += (int)(4 * Mathf.Pow(loopsToPerform, 2) - 4 * (loopsToPerform) + 1);
                }

                chunkPosition.y -= worldDimensions;// - (worldDimensions * 2);// epochCount);
                chunkPosition.x = originVector.x;
                chunkPosition.z = originVector.z;
                loopsPerformed = tempStoreLoopsPerformed;
                
                /*
                 * tempBlockNumbers is still not quite right for handling depth as new blocks are being incorrectly placed within the list
                 */
                //tempStoreLoopsPerformed = loopsPerformed;
            }
            /*if (depthCount > 0)
            {
                if (oldLoopsToPerform == -1)
                {
                    tempLoopsPerformed = loopsPerformed;
                    loopsPerformed = 0;
                }
                else
                {
                    loopsPerformed = oldLoopsToPerform;
                }
                //chunkPosition = originVector;
                chunkPosition.x = originVector.x;
                chunkPosition.z = originVector.z;
                chunkPosition.y -= worldDimensions + (worldDimensions * epochCount);
            }*/

        }
        loopsPerformed = loopsToPerform;
        ///Figure out how to correctly generate more chunks outwards(x and z) while also building matching ones down(y)
        ///

        //oldLoopsToPerform = tempLoopsPerformed;
        //loopsPerformed = tempLoopsPerformed;
        //loopsToPerform = tempLoopsPerformed;
        endTime = System.DateTime.Now;
        UnityEngine.Debug.Log(transform.childCount * (worldDimensions * worldDimensions * worldDimensions));

        System.TimeSpan timeTaken = endTime - startTime;
        UnityEngine.Debug.Log(timeTaken.TotalSeconds);
    }

    public int findChunkLoopNum(int nameNum, int direction)
    {
        int depthFactor = 0;
        //For adding new chunks to the parent use trasform.SetSiblingIndex(x);
        //For handling depth, use (4n^2 - 4n +1) where n=numOfLoops-1
        int upChunk = -99, downChunk = -99, leftChunk = -99, rightChunk = -99, aboveChunk = -99, belowChunk = -99;

        int chunkBlockCount = ((int)(4 * Mathf.Pow(loopsToPerform, 2) - 4 * (loopsToPerform) + 1));

        if (nameNum > chunkBlockCount)
        {
            depthFactor = nameNum / chunkBlockCount;
            nameNum -= chunkBlockCount * depthFactor;
        }
        //for (int i = 1; i <= worldDimensions; i++)
        for (int i = 1; i <= loopsToPerform; i++)
        {
            int upLeft = (int)(1 + 4 * Mathf.Pow(i, 2));
            int upRight = (int)(1 + 2 * i + 4 * Mathf.Pow(i, 2));
            
            int downLeft = (int)(1 - 2 * i + 4 * Mathf.Pow(i, 2));
            int downRight = (int)(1 - 4 * i + 4 * Mathf.Pow(i, 2));
            int downRightPlusOne = (int)(1 - 4 * (i+1) + 4 * Mathf.Pow((i + 1), 2));

            //UnityEngine.Debug.Log(i + "i\t" + upLeft + "upLeft\t" + upRight + "upRight\t" + downLeft + "downLeft\t" + downRight + "downRight\t" + downRightPlusOne + "downRightPlusOne\t");

            //aboveChunk = (int)(nameNum - (4 * Mathf.Pow(loopsToPerform - 1, 2) - 4 * (loopsToPerform-1) + 1));
            //belowChunk = (int)(nameNum + (4 * Mathf.Pow(loopsToPerform, 2) - 4 * (loopsToPerform) + 1));
            //aboveChunk = nameNum - ((int)(4 * Mathf.Pow(loopsToPerform, 2) - 4 * (loopsToPerform) + 1) + (int)(4 * Mathf.Pow(loopsToPerform - 1, 2) - 4 * (loopsToPerform - 1) + 1));
            aboveChunk = nameNum - ((int)(4 * Mathf.Pow(loopsToPerform, 2) - 4 * (loopsToPerform) + 1));
            belowChunk = nameNum + ((int)(4 * Mathf.Pow(loopsToPerform, 2) - 4 * (loopsToPerform) + 1));// + (int)(4 * Mathf.Pow(loopsToPerform - 1, 2) - 4 * (loopsToPerform - 1)));
            //if (nameNum == 8)
            //UnityEngine.Debug.Log(aboveChunk + "___" + belowChunk);

            //if (nameNum == 0)
            if(nameNum % ((int)(4 * Mathf.Pow(loopsToPerform, 2) - 4 * (loopsToPerform) + 1)) ==  0)
            {
                upChunk = nameNum + 6;
                downChunk = nameNum + 2;
                leftChunk = nameNum + 4;
                rightChunk = nameNum + 8;
                //UnityEngine.Debug.Log(nameNum + "nameNum\t" + aboveChunk + "aboveChunk\t" + rightChunk + "rightChunk\t" + belowChunk + "belowChunk\t" + leftChunk + "leftChunk\t" + upChunk + "upChunk\t" + downChunk + "downChunk\t");
            }

            else if (nameNum == upLeft)
            {
                upChunk = nameNum + 13 + (8 * (i - 1));
                downChunk = nameNum - 1;
                leftChunk = nameNum + 11 + (8 * (i - 1));
                rightChunk = nameNum + 1;
                //fifth column(T)
                //UnityEngine.Debug.Log("upLeft" + nameNum + "\t\t" + upChunk + "upChunk\t" + downChunk + "downChunk\t" + leftChunk + "leftChunk\t" + rightChunk + "rightChunk\t");
                //UnityEngine.Debug.Log(nameNum + "nameNum\t" + aboveChunk + "aboveChunk\t" + rightChunk + "rightChunk\t" + belowChunk + "belowChunk\t" + leftChunk + "leftChunk\t" + upChunk + "upChunk\t" + downChunk + "downChunk\t");
            }
            else if (nameNum == upRight)
            {
                upChunk = nameNum + 13 + (8 * (i - 1));
                downChunk = nameNum + 1;
                leftChunk = nameNum - 1;
                rightChunk = nameNum + 15 + (8 * (i - 1));
                //sixth column(U)
                //UnityEngine.Debug.Log("upRight" + nameNum + "\t\t" + upChunk + "upChunk\t" + downChunk + "downChunk\t" + leftChunk + "leftChunk\t" + rightChunk + "rightChunk\t");
                //UnityEngine.Debug.Log(nameNum + "nameNum\t" + aboveChunk + "aboveChunk\t" + rightChunk + "rightChunk\t" + belowChunk + "belowChunk\t" + leftChunk + "leftChunk\t" + upChunk + "upChunk\t" + downChunk + "downChunk\t");
            }
            else if (nameNum == downLeft)
            {
                upChunk = nameNum + 1;
                downChunk = nameNum + 9 + (8 * (i - 1));
                leftChunk = nameNum + 11 + (8 * (i - 1));
                rightChunk = nameNum - 1;
                //seventh column(V)
                //UnityEngine.Debug.Log("downLeft" + nameNum + "\t\t" + upChunk + "upChunk\t" + downChunk + "downChunk\t" + leftChunk + "leftChunk\t" + rightChunk + "rightChunk\t");
                //UnityEngine.Debug.Log(nameNum + "nameNum\t" + aboveChunk + "aboveChunk\t" + rightChunk + "rightChunk\t" + belowChunk + "belowChunk\t" + leftChunk + "leftChunk\t" + upChunk + "upChunk\t" + downChunk + "downChunk\t");
            }
            else if (nameNum == downRight)
            {
                if(nameNum == 1)
                {
                    upChunk = nameNum + 8;
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
                //UnityEngine.Debug.Log("downRight" + nameNum + "\t\t" + upChunk + "upChunk\t" + downChunk + "downChunk\t" + leftChunk + "leftChunk\t" + rightChunk + "rightChunk\t");
                //UnityEngine.Debug.Log(nameNum + "nameNum\t" + aboveChunk + "aboveChunk\t" + rightChunk + "rightChunk\t" + belowChunk + "belowChunk\t" + leftChunk + "leftChunk\t" + upChunk + "upChunk\t" + downChunk + "downChunk\t");
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
                //UnityEngine.Debug.Log("column P" + nameNum + "\t\t" + upChunk + "upChunk\t" + downChunk + "downChunk\t" + leftChunk + "leftChunk\t" + rightChunk + "rightChunk\t");
                //UnityEngine.Debug.Log(nameNum + "nameNum\t" + aboveChunk + "aboveChunk\t" + rightChunk + "rightChunk\t" + belowChunk + "belowChunk\t" + leftChunk + "leftChunk\t" + upChunk + "upChunk\t" + downChunk + "downChunk\t");
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
                //UnityEngine.Debug.Log("column Q" + nameNum + "\t\t" + upChunk + "upChunk\t" + downChunk + "downChunk\t" + leftChunk + "leftChunk\t" + rightChunk + "rightChunk\t");
                //UnityEngine.Debug.Log(nameNum + "nameNum\t" + aboveChunk + "aboveChunk\t" + rightChunk + "rightChunk\t" + belowChunk + "belowChunk\t" + leftChunk + "leftChunk\t" + upChunk + "upChunk\t" + downChunk + "downChunk\t");
            }
            else if (nameNum > upRight && nameNum < downRightPlusOne)//Right side
            {//Will require extra work
                upChunk = nameNum - 1;
                if (nameNum + 1 == (int)(1 - 4 * (i + 1) + 4 * Mathf.Pow((i + 1), 2)))
                {
                    //UnityEngine.Debug.Log(i + "****************************************************************");
                    downChunk = downRight;
                    leftChunk = nameNum - (7 + (8*(i-2)))  - (8 * i);
                    if (i == 1)
                        leftChunk -= 1;
                }
                else
                {
                    //UnityEngine.Debug.Log(i + "****************************************************************2");
                    downChunk = nameNum + 1;
                    leftChunk = nameNum - 15 - (8 * (i - 2));
                }
                rightChunk = nameNum + 15 + (8 * (i - 1));
                //third column(R)
                //UnityEngine.Debug.Log("column R" + nameNum + "\t\t" + upChunk + "upChunk\t" + downChunk + "downChunk\t" + leftChunk + "leftChunk\t" + rightChunk + "rightChunk\t");
                //UnityEngine.Debug.Log(nameNum + "nameNum\t" + aboveChunk + "aboveChunk\t" + rightChunk + "rightChunk\t" + belowChunk + "belowChunk\t" + leftChunk + "leftChunk\t" + upChunk + "upChunk\t" + downChunk + "downChunk\t");
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
                //UnityEngine.Debug.Log("column S" + nameNum + "\t\t" + upChunk + "upChunk\t" + downChunk + "downChunk\t" + leftChunk + "leftChunk\t" + rightChunk + "rightChunk\t");
                //UnityEngine.Debug.Log(nameNum + "nameNum\t" + aboveChunk + "aboveChunk\t" + rightChunk + "rightChunk\t" + belowChunk + "belowChunk\t" + leftChunk + "leftChunk\t" + upChunk + "upChunk\t" + downChunk + "downChunk\t");
            }

            //UnityEngine.Debug.Log(upLeft + "upLeft\t" + upRight + "upRight\t" + downLeft + "downLeft\t" + downRight + "downRight");
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
        //UnityEngine.Debug.Log(nameNum);
        return -99;//desired direction wasn't found
    }

    int showLoopsAroundPlayer = 3;//Distance around the player
    int showLoopsPerformed = 1;
    int showTempLoopsPerformed = 0;

    /// <summary>
    /// To properly enable this method:
    /// 1) call it in Start()
    /// 2) comment out the two coroutine calls in chunkDistanceCalc (checkIfShouldShow && checkSurroundingChunks)
    /// 3) comment out the setActive(false) in dynamicChunkBuilder (tempNewBlock.SetActive(false))
    /// </summary>
    /// <returns></returns>
    private IEnumerator globalShowChunks()
    {
        Transform foundChunkToShow;
        Vector3 playerChunkPos = playerRef.transform.position;
        Vector3 oldPlayerPos = new Vector3( -99, -99, -99 );
        List<Vector3> foundChunksToActivate = new List<Vector3>();
        bool playerHasMoved = true;
        /*void buildChunksDown()
        {
            for (int epochCount = 0; epochCount < depthCount; epochCount++)
            {
                playerChunkPos.y -= worldDimensions;
            }
            playerChunkPos.y += worldDimensions * depthCount;
        }*/

        void checkChunkPos()
        {
            playerChunkPos.x = (Mathf.RoundToInt(playerChunkPos.x / worldDimensions) * worldDimensions);
            playerChunkPos.y = 0.0f;// = (Mathf.RoundToInt(playerChunkPos.y / worldDimensions) * worldDimensions);
            playerChunkPos.z = (Mathf.RoundToInt(playerChunkPos.z / worldDimensions) * worldDimensions);
            if (foundChunkToShow = transform.Find(playerChunkPos + ""))
            {
                //foundChunkToShow.name = "these";
                //UnityEngine.Debug.Log(foundChunkToShow.name);
                foundChunksToActivate.Add(playerChunkPos);
                //foundChunkToShow.gameObject.SetActive(true);
                //foundChunkToShow.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
        void cycleChunks()
        {
            /*foreach(Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }*/
            foreach(Vector3 childName in foundChunksToActivate)
            {
                transform.Find(childName + "").gameObject.SetActive(true);
                transform.Find(childName + "").transform.GetChild(0).gameObject.SetActive(true);
            }
            foundChunksToActivate.Clear();
        }

        yield return new WaitForSeconds(2);

        while (true)
        {
            //yield return new WaitForSeconds(1);
            yield return new WaitForFixedUpdate();
            playerChunkPos = playerRef.transform.position;
            playerChunkPos.x = (Mathf.RoundToInt(playerChunkPos.x / worldDimensions) * worldDimensions);
            playerChunkPos.y = 0.0f;
            playerChunkPos.z = (Mathf.RoundToInt(playerChunkPos.z / worldDimensions) * worldDimensions);

            /*if(Vector3.Distance(playerChunkPos, oldPlayerPos) == 0 && playerHasMoved)     //check if player Moved
            {
                UnityEngine.Debug.Log("no change");
                playerHasMoved = false;
            }
            else if(Vector3.Distance(playerChunkPos, oldPlayerPos) != 0 && !playerHasMoved)
            {
                UnityEngine.Debug.Log(Vector3.Distance(playerChunkPos, oldPlayerPos));
                playerHasMoved = true;
            }*/
            showLoopsAroundPlayer = 3;
            showLoopsPerformed = 1;
            showTempLoopsPerformed = 0;
            //UnityEngine.Debug.Log(playerChunkPos + " NEW SET OF CHUNKS\n\n");


            while (showLoopsPerformed < showLoopsAroundPlayer)
            {
                yield return new WaitForFixedUpdate();
                //yield return new WaitForSeconds(1);

                if (showLoopsPerformed > 0)
                {
                    showTempLoopsPerformed = showLoopsPerformed;
                    while (showTempLoopsPerformed > 0)
                    {
                        playerChunkPos.x += worldDimensions;//move right * number of loops along the X axis
                        checkChunkPos();
                        showTempLoopsPerformed--;
                    }

                    showTempLoopsPerformed = showLoopsPerformed;
                    while (showTempLoopsPerformed > 0)
                    {
                        playerChunkPos.z -= worldDimensions;//move down * number of loops
                        checkChunkPos();
                        showTempLoopsPerformed--;

                        //buildChunksDown();
                    }

                    showTempLoopsPerformed = showLoopsPerformed * 2;
                    while (showTempLoopsPerformed > 0)
                    {
                        playerChunkPos.x -= worldDimensions;//move left 1 spot then * number of loops
                        checkChunkPos();
                        showTempLoopsPerformed--;

                        //buildChunksDown();
                    }

                    showTempLoopsPerformed = showLoopsPerformed * 2;
                    while (showTempLoopsPerformed > 0)
                    {
                        playerChunkPos.z += worldDimensions;//move up 1 spot then * number of loops
                        checkChunkPos();
                        showTempLoopsPerformed--;

                        //buildChunksDown();
                    }

                    showTempLoopsPerformed = showLoopsPerformed * 2;
                    while (showTempLoopsPerformed > 0)
                    {
                        playerChunkPos.x += worldDimensions;//move right 1 spot then * number of loops
                        checkChunkPos();
                        showTempLoopsPerformed--;

                        //buildChunksDown();
                    }

                    showTempLoopsPerformed = showLoopsPerformed;
                    while (showTempLoopsPerformed > 0)
                    {
                        playerChunkPos.z -= worldDimensions;//move down * number of loops
                        checkChunkPos();
                        showTempLoopsPerformed--;

                        //buildChunksDown();
                    }
                    showTempLoopsPerformed = showLoopsPerformed;
                    while (showTempLoopsPerformed > 0)
                    {
                        playerChunkPos.x -= worldDimensions;
                        checkChunkPos();
                        showTempLoopsPerformed--;
                    }
                }
                /*if (loopsPerformed == 0)
                {
                    tempChunkHolder = Instantiate(chunkToGen, chunkPosition, transform.rotation, worldBlockOriginPoint.transform);
                    tempChunkHolder.name = chunkPosition + "";
                    buildChunksDown();
                }*/
                showLoopsPerformed++;
            }
            cycleChunks();
            oldPlayerPos = playerChunkPos;
        }
    }
}
