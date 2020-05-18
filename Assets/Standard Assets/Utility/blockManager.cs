using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class blockManager : MonoBehaviour
{
    public Dictionary<int, int> gatheredBlocksValues;
    private int placingBlockMaterial = 1;
    public bool blockFound = false;
    //private int lastGoodBlockIndex = 0;
    //private Vector3 properTriggerSize = new Vector3(1, 1, 1);
    public Material noBlockMat;
    private Vector3 errorVector = new Vector3(-.1f, -.1f, -.1f);
    public GameObject blockTemplateToBePlaced;
    Vector3 newPlacedBlockLocation;
    public GameObject blockTemplate;
    public GameObject tempParentForPlacedBlocks;
    public GameObject torchTemplate;

    private bool placingTorch = false;

    //public List<int> blockNames;
    //public List<int> blockCounts;

    private int blockTypeCount = 0;


    private void Start()
    {
        gatheredBlocksValues = new Dictionary<int, int> { };

        blockTypeCount = Resources.LoadAll("BlockArt/4-All/").Length;
    }

    public void addBlock(int blockMaterialValue)
    {
        if (gatheredBlocksValues.ContainsKey(blockMaterialValue))
        {
            gatheredBlocksValues[blockMaterialValue]++;
            //GameObject.Find("placingBlockTemplate").GetComponent<placedBlockSnapping>().checkIfBlocksGained(blockMaterialValue);
        }
        else
        {
            gatheredBlocksValues[blockMaterialValue] = 1;
        }
    }

    private int removeBlock(int blockMaterialValue)
    {
        Debug.Log("removed block " + blockMaterialValue);
        if (gatheredBlocksValues.ContainsKey(blockMaterialValue))
        {
            if (gatheredBlocksValues[blockMaterialValue] > 0)
            {
                --gatheredBlocksValues[blockMaterialValue];
                return gatheredBlocksValues[blockMaterialValue];
            }
        }
        return -1;
    }



    private void Update()
    {
        //blockNames = new List<int>(gatheredBlocksValues.Keys);
        //blockCounts = new List<int>(gatheredBlocksValues.Values);

        if (Input.GetKeyDown(KeyCode.Q) && placingBlockMaterial > 0)
        {
            placingBlockMaterial--;
            if (gatheredBlocksValues.ContainsKey(placingBlockMaterial))
            {
                if (gatheredBlocksValues[placingBlockMaterial] > 0)
                {
                    //lastGoodBlockIndex = placingBlockMaterial;
                    blockFound = true;

                    blockTemplateToBePlaced.GetComponent<Renderer>().material = (Material)Resources.Load("BlockArt/4-All/" + placingBlockMaterial);
                    return;
                }
                else
                {
                    blockFound = false;
                    blockTemplateToBePlaced.GetComponent<Renderer>().material = noBlockMat;
                }
            }
            else
            {
                blockFound = false;
                blockTemplateToBePlaced.GetComponent<Renderer>().material = noBlockMat;
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && placingBlockMaterial < blockTypeCount-1)
        {
            placingBlockMaterial++;
            if (gatheredBlocksValues.ContainsKey(placingBlockMaterial))
            {
                if (gatheredBlocksValues[placingBlockMaterial] > 0)
                {
                    //lastGoodBlockIndex = placingBlockMaterial;
                    blockFound = true;

                    blockTemplateToBePlaced.GetComponent<Renderer>().material = (Material)Resources.Load("BlockArt/4-All/" + placingBlockMaterial);
                    return;
                }
                else
                {
                    blockFound = false;
                    blockTemplateToBePlaced.GetComponent<Renderer>().material = noBlockMat;
                }
            }
            else
            {
                blockFound = false;
                blockTemplateToBePlaced.GetComponent<Renderer>().material = noBlockMat;
            }
        }
        /*
         * Change this to be a function that is called on the placingBlockTemplate
         */
        //transform.position = GameObject.Find("blockTracker").transform.position;
        //transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), Mathf.RoundToInt(transform.position.z));

        //if (CrossPlatformInputManager.GetButtonDown("BuildMode"))
        if(Input.GetKeyDown(KeyCode.T))
        {
            placingTorch = !placingTorch;
            transform.parent.GetChild(2).gameObject.SetActive(placingTorch);
            
            /*newPlacedBlockLocation = blockTemplateToBePlaced.GetComponent<placedBlockSnapping>().pickNewOpenPos2();
            if (Vector3.Equals(newPlacedBlockLocation, errorVector))
            {
                Debug.Log("CAN'T PLACE");
                return;
            }
            
            GameObject tempTorch = Instantiate(torchTemplate);

            tempTorch.transform.position = newPlacedBlockLocation;

            tempTorch.gameObject.layer = 16;
            tempTorch.transform.SetParent(tempParentForPlacedBlocks.transform);*/
        }

        if(Input.GetKeyDown(KeyCode.B))
        {
            if (blockFound)
            {
                //if (blockTrackerRef.GetComponent<gatherBlocks>().blockCounts[blockTrackerRef.GetComponent<gatherBlocks>().blockNames.IndexOf(placingBlockMaterial + " (Instance)")] > 0)
                if (gatheredBlocksValues[placingBlockMaterial] > 0)
                {
                    Debug.Log("placing block " + placingBlockMaterial);
                    newPlacedBlockLocation = blockTemplateToBePlaced.GetComponent<placedBlockSnapping>().pickNewOpenPos2();
                    Debug.Log(newPlacedBlockLocation + " newOpenPos");
                    if (Vector3.Equals(newPlacedBlockLocation, errorVector))
                    {
                        Debug.Log("CAN'T PLACE");
                        return;
                    }

                    //int removedBlockCount = blockTrackerRef.GetComponent<gatherBlocks>().removeBlock(placingBlockMaterial + " (Instance)");
                    int removedBlockCount = removeBlock(placingBlockMaterial);

                    GameObject tempObject = Instantiate(blockTemplate);

                    tempObject.transform.position = newPlacedBlockLocation;

                    tempObject.name = "placedworldBlock";
                    //tempObject.GetComponent<BoxCollider>().size = properTriggerSize;
                    tempObject.gameObject.layer = 16;
                    //tempObject.GetComponent<Renderer>().enabled = true;
                    tempObject.GetComponent<Renderer>().material = blockTemplateToBePlaced.GetComponent<Renderer>().material;
                    //Destroy(tempObject.GetComponent<placedBlockSnapping>());
                    //Destroy(tempObject.GetComponent<Rigidbody>());
                    tempObject.transform.SetParent(tempParentForPlacedBlocks.transform);
//                    tempObject.transform.SetParent(null);
                    /*foreach (Transform placedBlockChildren in tempObject.transform)
                    {
                        Destroy(placedBlockChildren.gameObject);
                    }*/
                    if (removedBlockCount == 0)
                    {
                        blockTemplateToBePlaced.GetComponent<Renderer>().material = noBlockMat;
                        blockFound = false;
                        //lastGoodBlockIndex = 11;
                    }
                }
                /*else
                {
                    Debug.Log("out of blocks" + gatheredBlocksValues[placingBlockMaterial]);
                }*/
            }
            //removedBlockCount
        }
    }
}
