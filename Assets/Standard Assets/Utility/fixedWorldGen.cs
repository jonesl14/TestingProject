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
    private bool firstRun = true;

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

        StartCoroutine(spawnOnlyChunksThatAreNeeded());
        
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

    public void breakChunkBlockByPosition2(Vector3 chunkPos, int blockChildNum)//changeNeed
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
    }

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

    private void Update()
    {
        if (spawnOption == 0)
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                loopsToPerform += 1;
                StartCoroutine(spawnOnlyChunksThatAreNeeded());
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
                tempChunkHolder.name = chunkPosition + "";
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
                    tempChunkHolder.name = chunkPosition + "";
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
                    tempChunkHolder.name = chunkPosition + "";
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
                    tempChunkHolder.name = chunkPosition + "";
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
                    tempChunkHolder.name = chunkPosition + "";
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
                    tempChunkHolder.name = chunkPosition + "";
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
                tempChunkHolder.name = chunkPosition + "";
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
}
