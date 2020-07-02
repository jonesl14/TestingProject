using System.Collections;
using UnityEngine;

public class BlockBreaking : MonoBehaviour
{
    //public bool activeBlock = true;
    private int blockHealth = 1;
    private bool beingBroken = false;
    private int worldDimensions, squaredWorldDimensions, cubedWorldDimensions;
    public int blockStatus = 0;

    private Transform fpsControllerRayRef;

    private void Awake()
    {
        fpsControllerRayRef = transform.parent.GetComponentInParent<chunkDistanceCalc>().fpsControllerRef.transform.GetChild(1);
        //GetComponent<Renderer>().enabled = false;
        if (GetComponentInParent<chunkDistanceCalc>())
        {
            worldDimensions = GetComponentInParent<chunkDistanceCalc>().worldDimensions;
            squaredWorldDimensions = worldDimensions * worldDimensions;
            cubedWorldDimensions = worldDimensions * worldDimensions * worldDimensions;
        }
    }

    bool showingClose = false;
    int tempViewDistance = 10;

    //private void Updated()
    private IEnumerator raycastBlocks()
    {
        yield return new WaitForSeconds(.2f);
        if(blockStatus == 1)
        {
            Vector3 rayDirection = (fpsControllerRayRef.position - transform.position).normalized;
            float rayDistance = Vector3.Distance(fpsControllerRayRef.position, transform.position);

            if (rayDistance >= tempViewDistance)// && rayDistance < 10 * 3)
            {
                int layerMask = 1 << 10;

                if (Physics.Raycast(transform.position, rayDirection, rayDistance, layerMask))
                {
                    if (showingClose)
                    {
                        GetComponent<Renderer>().enabled = false;
                        showingClose = false;
                    }
                }
                else
                {
                    if (!showingClose)
                    {
                        GetComponent<Renderer>().enabled = true;
                        showingClose = true;
                    }
                }
            }
            else if (rayDistance < tempViewDistance)
            {
                if (!showingClose)
                {
                    GetComponent<Renderer>().enabled = true;
                    showingClose = true;
                }
            }
        }
    }


    private IEnumerator BlockDamage()
    {
        beingBroken = true;
        {
            blockHealth--;
            if (blockHealth == 2)
            {
                GetComponent<Renderer>().material.color = Color.white;
            }
            if (blockHealth == 1)
            {
                GetComponent<Renderer>().material.color = Color.black;
            }
            if (blockHealth == 0)
            {
                GetComponent<Renderer>().material.color = Color.yellow;
            }
            yield return new WaitForSeconds(.1f);
        }
        beingBroken = false;

    }

    //remove the Find() calls
    public void killBlock()
    {
        if (!beingBroken)
        {
            StartCoroutine(BlockDamage());
        }
        if (blockHealth < 0)
        {
            if(gameObject.layer == 16)
            {
                Destroy(gameObject);
                GameObject.Find("blockTracker").GetComponent<blockManager>().addBlock(int.Parse(GetComponent<Renderer>().material.name.Split()[0]));
                return;
            }
            if (transform.parent.GetSiblingIndex() == 0)
            {
                transform.parent.transform.parent.GetComponent<chunkDistanceCalc>().changeBlockState(transform.GetSiblingIndex(), 0);
            }
            gameObject.SetActive(false);
            //activeBlock = false;

            GameObject.Find("blockTracker").GetComponent<blockManager>().addBlock(int.Parse(GetComponent<Renderer>().material.name.Split()[0]));
        }
    }

    public void instaKillBlock()
    {
        worldDimensions = GetComponentInParent<chunkDistanceCalc>().worldDimensions;
        squaredWorldDimensions = worldDimensions * worldDimensions;
        cubedWorldDimensions = worldDimensions * worldDimensions * worldDimensions;

        //UnityEngine.Debug.Log("2\t\t" + transform.GetSiblingIndex());
        //if(transform.position.y < -3)
        {
            //UnityEngine.Debug.Log("3\t\t" + transform.GetSiblingIndex());
            transform.parent.transform.parent.GetComponent<chunkDistanceCalc>().changeBlockState(transform.GetSiblingIndex(), 0);
            //int tempBreakSize = UnityEngine.Random.Range(worldDimensions, squaredWorldDimensions);
            //UnityEngine.Debug.Log(worldDimensions + "\t" + squaredWorldDimensions + "\t" + tempBreakSize);
            //breakBlocksAround(tempBreakSize);
            breakBlocksAround(UnityEngine.Random.Range(1, worldDimensions));
            //breakBlocksAround(UnityEngine.Random.Range(worldDimensions, squaredWorldDimensions));
        }
    }

    private void breakBlocksAround(int sizeOfBreak)
    {
        int thisChildIndex = transform.GetSiblingIndex();

        for (int breakIndex = 1; breakIndex < sizeOfBreak; breakIndex++)
        {
            if ((thisChildIndex + breakIndex) < cubedWorldDimensions)//moves right
            {
                GetComponentInParent<chunkDistanceCalc>().changeBlockState(thisChildIndex + breakIndex, 0);
                //transform.parent.GetChild(thisChildIndex + breakIndex).GetComponent<BlockBreaking>().instaKillBlock2();
                if ((thisChildIndex + breakIndex + (breakIndex * squaredWorldDimensions)) < cubedWorldDimensions)//moves right and down one
                {
                    GetComponentInParent<chunkDistanceCalc>().changeBlockState(thisChildIndex + breakIndex + (breakIndex * squaredWorldDimensions),0);
                    //transform.parent.GetChild(thisChildIndex + breakIndex + (breakIndex * squaredWorldDimensions)).GetComponent<BlockBreaking>().instaKillBlock2();
                }
                if (((thisChildIndex + breakIndex - (breakIndex * squaredWorldDimensions)) < cubedWorldDimensions) && (thisChildIndex + breakIndex - (breakIndex * squaredWorldDimensions)) > 0)//moves right and up one
                {
                    GetComponentInParent<chunkDistanceCalc>().changeBlockState(thisChildIndex + breakIndex - (breakIndex * squaredWorldDimensions), 0);
                    //transform.parent.GetChild(thisChildIndex + breakIndex - (breakIndex * squaredWorldDimensions)).GetComponent<BlockBreaking>().instaKillBlock2();
                }
            }
            if ((thisChildIndex - breakIndex) > 0)//moves left
            {
                GetComponentInParent<chunkDistanceCalc>().changeBlockState(thisChildIndex - breakIndex, 0);
                //transform.parent.GetChild(thisChildIndex - breakIndex).GetComponent<BlockBreaking>().instaKillBlock2();
                if ((thisChildIndex - breakIndex + (breakIndex * squaredWorldDimensions)) < cubedWorldDimensions)//moves left and down one
                {
                    GetComponentInParent<chunkDistanceCalc>().changeBlockState(thisChildIndex - breakIndex + (breakIndex * squaredWorldDimensions), 0);
                    //transform.parent.GetChild(thisChildIndex - breakIndex + (breakIndex * squaredWorldDimensions)).GetComponent<BlockBreaking>().instaKillBlock2();
                }
                if (((thisChildIndex - breakIndex - (breakIndex * squaredWorldDimensions)) < cubedWorldDimensions) && (thisChildIndex - breakIndex - (breakIndex * squaredWorldDimensions)) > 0)//moves left and up one
                {
                    GetComponentInParent<chunkDistanceCalc>().changeBlockState(thisChildIndex - breakIndex - (breakIndex * squaredWorldDimensions), 0);
                    //transform.parent.GetChild(thisChildIndex - breakIndex - (breakIndex * squaredWorldDimensions)).GetComponent<BlockBreaking>().instaKillBlock2();
                }
            }

            if((thisChildIndex + (breakIndex * worldDimensions)) < cubedWorldDimensions)//moves forward
            {
                GetComponentInParent<chunkDistanceCalc>().changeBlockState(thisChildIndex + (breakIndex * worldDimensions), 0);
                //transform.parent.GetChild(thisChildIndex + (breakIndex * worldDimensions)).GetComponent<BlockBreaking>().instaKillBlock2();
            }
            if ((thisChildIndex - (breakIndex * worldDimensions)) > 0)//moves back
            {
                GetComponentInParent<chunkDistanceCalc>().changeBlockState(thisChildIndex - (breakIndex * worldDimensions), 0);
                //transform.parent.GetChild(thisChildIndex - (breakIndex * worldDimensions)).GetComponent<BlockBreaking>().instaKillBlock2();
            }

            if ((thisChildIndex + (breakIndex * squaredWorldDimensions)) < cubedWorldDimensions)//moves down
            {
                GetComponentInParent<chunkDistanceCalc>().changeBlockState(thisChildIndex + (breakIndex * squaredWorldDimensions), 0);
                //transform.parent.GetChild(thisChildIndex + (breakIndex * squaredWorldDimensions)).GetComponent<BlockBreaking>().instaKillBlock2();
                if ((thisChildIndex + (breakIndex * squaredWorldDimensions) + breakIndex) < cubedWorldDimensions)//moves down and one to the right
                {
                    GetComponentInParent<chunkDistanceCalc>().changeBlockState(thisChildIndex + (breakIndex * squaredWorldDimensions) + breakIndex, 0);
                    //transform.parent.GetChild(thisChildIndex + (breakIndex * squaredWorldDimensions) + breakIndex).GetComponent<BlockBreaking>().instaKillBlock2();
                }
                if ((thisChildIndex + (breakIndex * squaredWorldDimensions) - breakIndex) < cubedWorldDimensions)//moves down and one to the right
                {
                    GetComponentInParent<chunkDistanceCalc>().changeBlockState(thisChildIndex + (breakIndex * squaredWorldDimensions) - breakIndex, 0);
                    //transform.parent.GetChild(thisChildIndex + (breakIndex * squaredWorldDimensions) - breakIndex).GetComponent<BlockBreaking>().instaKillBlock2();
                }

                if ((thisChildIndex + (breakIndex * squaredWorldDimensions) + (breakIndex * worldDimensions)) < cubedWorldDimensions)//moves down and one forward
                {
                    GetComponentInParent<chunkDistanceCalc>().changeBlockState(thisChildIndex + (breakIndex * squaredWorldDimensions) + (breakIndex * worldDimensions), 0);
                    //transform.parent.GetChild(thisChildIndex + (breakIndex * squaredWorldDimensions) + (breakIndex * worldDimensions)).GetComponent<BlockBreaking>().instaKillBlock2();
                }
                if ((thisChildIndex + (breakIndex * squaredWorldDimensions) - (breakIndex * worldDimensions)) < cubedWorldDimensions)//moves down and one back
                {
                    GetComponentInParent<chunkDistanceCalc>().changeBlockState(thisChildIndex + (breakIndex * squaredWorldDimensions) - (breakIndex * worldDimensions), 0);
                    //transform.parent.GetChild(thisChildIndex + (breakIndex * squaredWorldDimensions) - (breakIndex * worldDimensions)).GetComponent<BlockBreaking>().instaKillBlock2();
                }
            }
            if ((thisChildIndex - (breakIndex * squaredWorldDimensions)) > 0)//moves up
            {
                GetComponentInParent<chunkDistanceCalc>().changeBlockState(thisChildIndex - (breakIndex * squaredWorldDimensions), 0);
                //transform.parent.GetChild(thisChildIndex - (breakIndex * squaredWorldDimensions)).GetComponent<BlockBreaking>().instaKillBlock2();
                if ((thisChildIndex - (breakIndex * squaredWorldDimensions) - breakIndex) > 0 && (thisChildIndex - (breakIndex * squaredWorldDimensions) - breakIndex) < cubedWorldDimensions)//moves up and one to the left
                {
                    GetComponentInParent<chunkDistanceCalc>().changeBlockState(thisChildIndex - (breakIndex * squaredWorldDimensions) - breakIndex, 0);
                    //transform.parent.GetChild(thisChildIndex - (breakIndex * squaredWorldDimensions) - breakIndex).GetComponent<BlockBreaking>().instaKillBlock2();
                }
                if ((thisChildIndex - (breakIndex * squaredWorldDimensions) + breakIndex) > 0 && (thisChildIndex - (breakIndex * squaredWorldDimensions) + breakIndex) < cubedWorldDimensions)//moves up and one to the left
                {
                    GetComponentInParent<chunkDistanceCalc>().changeBlockState(thisChildIndex - (breakIndex * squaredWorldDimensions) + breakIndex, 0);
                    //transform.parent.GetChild(thisChildIndex - (breakIndex * squaredWorldDimensions) + breakIndex).GetComponent<BlockBreaking>().instaKillBlock2();
                }

                if ((thisChildIndex - (breakIndex * squaredWorldDimensions) - (breakIndex * worldDimensions)) > 0 
                    && (thisChildIndex - (breakIndex * squaredWorldDimensions) - (breakIndex * worldDimensions)) < cubedWorldDimensions)//moves up and one back
                {
                    GetComponentInParent<chunkDistanceCalc>().changeBlockState(thisChildIndex - (breakIndex * squaredWorldDimensions) - (breakIndex * worldDimensions), 0);
                    //transform.parent.GetChild(thisChildIndex - (breakIndex * squaredWorldDimensions) - (breakIndex * worldDimensions)).GetComponent<BlockBreaking>().instaKillBlock2();
                }
                if ((thisChildIndex - (breakIndex * squaredWorldDimensions) + (breakIndex * worldDimensions)) > 0 
                    && (thisChildIndex - (breakIndex * squaredWorldDimensions) + (breakIndex * worldDimensions)) < cubedWorldDimensions)//moves up and one forward
                {
                    GetComponentInParent<chunkDistanceCalc>().changeBlockState(thisChildIndex - (breakIndex * squaredWorldDimensions) + (breakIndex * worldDimensions), 0);
                    //transform.parent.GetChild(thisChildIndex - (breakIndex * squaredWorldDimensions) + (breakIndex * worldDimensions)).GetComponent<BlockBreaking>().instaKillBlock2();
                }
            }
        }
    }

    public void instaKillBlock2()
    {
        transform.parent.transform.parent.GetComponent<chunkDistanceCalc>().changeBlockState(transform.GetSiblingIndex(), 0);
    }
}
