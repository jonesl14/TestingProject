using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class blockManager : MonoBehaviour
{
    public Dictionary<int, int> gatheredBlocksValues;
    private int placingBlockMaterial = 1;
    public bool blockFound = false;
    public Material noBlockMat;
    private Vector3 errorVector = new Vector3(-.1f, -.1f, -.1f);
    public GameObject blockTemplateToBePlaced;
    Vector3 newPlacedBlockLocation;
    public GameObject blockTemplate;
    public GameObject tempParentForPlacedBlocks;
    public GameObject torchTemplate;
    public GameObject UIref;

    
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
        }
        else
        {
            gatheredBlocksValues[blockMaterialValue] = 1;
        }
        if(placingBlockMaterial == blockMaterialValue)
        {
            UIref.GetComponent<Text>().text = placingBlockMaterial + "\t" + gatheredBlocksValues[blockMaterialValue];
        }
    }

    public int removeBlock(int blockMaterialValue)
    {
        //Debug.Log("removed block " + blockMaterialValue);
        if (gatheredBlocksValues.ContainsKey(blockMaterialValue))
        {
            if (gatheredBlocksValues[blockMaterialValue] > 0)
            {
                --gatheredBlocksValues[blockMaterialValue];
                if (placingBlockMaterial == blockMaterialValue)
                {
                    UIref.GetComponent<Text>().text = placingBlockMaterial + "\t" + gatheredBlocksValues[blockMaterialValue];
                }
                return gatheredBlocksValues[blockMaterialValue];
            }
        }
        return -1;
    }

    public int getCountOfBlock(int blockTypeNum)
    {
        if(gatheredBlocksValues.ContainsKey(blockTypeNum))
        {
            return gatheredBlocksValues[blockTypeNum];
        }
        else
        {
            return 0;
        }
    }



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && placingBlockMaterial > 0)
        {
            placingBlockMaterial--;
            UIref.GetComponent<Text>().text = placingBlockMaterial + "\t";// + gatheredBlocksValues[placingBlockMaterial];
            if (gatheredBlocksValues.ContainsKey(placingBlockMaterial))
            {
                if (gatheredBlocksValues[placingBlockMaterial] > 0)
                {
                    blockFound = true;
                    blockTemplateToBePlaced.GetComponent<Renderer>().material = (Material)Resources.Load("BlockArt/4-All/" + placingBlockMaterial);
                    UIref.GetComponent<Text>().text += ""+gatheredBlocksValues[placingBlockMaterial];
                    return;
                }
                else
                {
                    blockFound = false;
                    blockTemplateToBePlaced.GetComponent<Renderer>().material = noBlockMat;
                    UIref.GetComponent<Text>().text += "" + 0;
                }
            }
            else
            {
                blockFound = false;
                blockTemplateToBePlaced.GetComponent<Renderer>().material = noBlockMat;
                UIref.GetComponent<Text>().text += "" + 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && placingBlockMaterial < blockTypeCount - 1)
        {
            placingBlockMaterial++;
            UIref.GetComponent<Text>().text = placingBlockMaterial + "\t";
            if (gatheredBlocksValues.ContainsKey(placingBlockMaterial))
            {
                if (gatheredBlocksValues[placingBlockMaterial] > 0)
                {
                    blockFound = true;
                    blockTemplateToBePlaced.GetComponent<Renderer>().material = (Material)Resources.Load("BlockArt/4-All/" + placingBlockMaterial);
                    UIref.GetComponent<Text>().text += "" + gatheredBlocksValues[placingBlockMaterial];
                    return;
                }
                else
                {
                    blockFound = false;
                    blockTemplateToBePlaced.GetComponent<Renderer>().material = noBlockMat;
                    UIref.GetComponent<Text>().text += "" + 0;
                }
            }
            else
            {
                blockFound = false;
                blockTemplateToBePlaced.GetComponent<Renderer>().material = noBlockMat;
                UIref.GetComponent<Text>().text += "" + 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            if (blockFound)
            {
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

                    int removedBlockCount = removeBlock(placingBlockMaterial);

                    GameObject tempObject = Instantiate(blockTemplate);

                    tempObject.transform.position = newPlacedBlockLocation;

                    //tempObject.name = "placedworldBlock";
                    tempObject.gameObject.layer = 16;
                    tempObject.GetComponent<Renderer>().material = blockTemplateToBePlaced.GetComponent<Renderer>().material;
                    tempObject.transform.SetParent(tempParentForPlacedBlocks.transform);

                    if (removedBlockCount == 0)
                    {
                        blockTemplateToBePlaced.GetComponent<Renderer>().material = noBlockMat;
                        blockFound = false;
                    }
                }
            }
        }
    }
}
