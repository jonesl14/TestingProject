using System.Collections;
using UnityEngine;

public class BlockBreaking : MonoBehaviour
{
    public bool activeBlock = true;
    private int blockHealth = 1;
    private bool beingBroken = false;
    private int worldDimensions, squaredWorldDimensions, cubedWorldDimensions;

    private void Awake()
    {
        worldDimensions = GameObject.Find("OriginPoint").GetComponent<fixedWorldGen>().worldDimensions;
        squaredWorldDimensions = worldDimensions * worldDimensions;
        cubedWorldDimensions = worldDimensions * worldDimensions * worldDimensions;
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
                /*if (transform.parent.GetSiblingIndex() == 0)
                {
                    transform.parent.transform.parent.GetComponent<chunkDistanceCalc>().changeBlockState(transform.GetSiblingIndex(), 0);
                }*/
                //UnityEngine.Debug.Log(gameObject.transform.position + " position");
                //UnityEngine.Debug.Log(gameObject.transform.localPosition + " localPosition");
                Destroy(gameObject);
                GameObject.Find("blockTracker").GetComponent<blockManager>().addBlock(int.Parse(GetComponent<Renderer>().material.name.Split()[0]));
                return;
            }
            if (transform.parent.GetSiblingIndex() == 0)
            {
                transform.parent.transform.parent.GetComponent<chunkDistanceCalc>().changeBlockState(transform.GetSiblingIndex(), 0);
            }
            //UnityEngine.Debug.Log(gameObject.transform.position + " position");
            //UnityEngine.Debug.Log(gameObject.transform.localPosition + " localPosition");
            gameObject.SetActive(false);
            activeBlock = false;

            GameObject.Find("blockTracker").GetComponent<blockManager>().addBlock(int.Parse(GetComponent<Renderer>().material.name.Split()[0]));

            //GameObject.Find("OriginPoint").GetComponent<fixedWorldGen>().breakChunkBlockByPosition2(transform.parent.transform.position, transform.GetSiblingIndex());
            GameObject.Find("OriginPoint").GetComponent<fixedWorldGen>().breakChunkBlockRecursive(transform.parent.transform.position, transform.GetSiblingIndex());
        }
    }

    public void instaKillBlock()
    {
        worldDimensions = GameObject.Find("OriginPoint").GetComponent<fixedWorldGen>().worldDimensions;
        if(transform.position.y < -3)
        {
            transform.parent.transform.parent.GetComponent<chunkDistanceCalc>().changeBlockState(transform.GetSiblingIndex(), 0);

            //gameObject.SetActive(false);
            //activeBlock = false;

            //GetComponent<Renderer>().material.color = Color.black;

            GameObject.Find("OriginPoint").GetComponent<fixedWorldGen>().breakChunkBlockRecursive(transform.parent.transform.position, transform.GetSiblingIndex());
            //breakBlocksAround(UnityEngine.Random.Range(1, worldDimensions-1));
            //breakBlocksAround(UnityEngine.Random.Range(1, squaredWorldDimensions));
            breakBlocksAround(UnityEngine.Random.Range(worldDimensions, squaredWorldDimensions));

        }
    }

    private void breakBlocksAround(int sizeOfBreak)
    {
        int thisChildIndex = transform.GetSiblingIndex();

        for (int breakIndex = 1; breakIndex < sizeOfBreak; breakIndex++)
        {
            if ((thisChildIndex + breakIndex) < cubedWorldDimensions)
            {
                transform.parent.GetChild(thisChildIndex + breakIndex).GetComponent<BlockBreaking>().instaKillBlock2();
            }
            if ((thisChildIndex - breakIndex) > 0)
            {
                transform.parent.GetChild(thisChildIndex - breakIndex).GetComponent<BlockBreaking>().instaKillBlock2();
            }

            if((thisChildIndex + (breakIndex * worldDimensions)) < cubedWorldDimensions)
            {
                transform.parent.GetChild(thisChildIndex + (breakIndex * worldDimensions)).GetComponent<BlockBreaking>().instaKillBlock2();
            }
            if ((thisChildIndex - (breakIndex * worldDimensions)) > 0)
            {
                transform.parent.GetChild(thisChildIndex - (breakIndex * worldDimensions)).GetComponent<BlockBreaking>().instaKillBlock2();
            }

            if ((thisChildIndex + (breakIndex * squaredWorldDimensions)) < cubedWorldDimensions)
            {
                transform.parent.GetChild(thisChildIndex + (breakIndex * squaredWorldDimensions)).GetComponent<BlockBreaking>().instaKillBlock2();

                if ((thisChildIndex + (breakIndex * squaredWorldDimensions + breakIndex)) < cubedWorldDimensions)
                {
                    transform.parent.GetChild(thisChildIndex + (breakIndex * squaredWorldDimensions + breakIndex)).GetComponent<BlockBreaking>().instaKillBlock2();
                }
            }
            if ((thisChildIndex - (breakIndex * squaredWorldDimensions)) > 0)
            {
                transform.parent.GetChild(thisChildIndex - (breakIndex * squaredWorldDimensions)).GetComponent<BlockBreaking>().instaKillBlock2();

                if ((thisChildIndex - (breakIndex * squaredWorldDimensions - breakIndex)) > 0 && (thisChildIndex - (breakIndex * squaredWorldDimensions - breakIndex)) < cubedWorldDimensions)
                {
                    transform.parent.GetChild(thisChildIndex - (breakIndex * squaredWorldDimensions - breakIndex)).GetComponent<BlockBreaking>().instaKillBlock2();
                }
            }
        }
    }

    public void instaKillBlock2()
    {
        transform.parent.transform.parent.GetComponent<chunkDistanceCalc>().changeBlockState(transform.GetSiblingIndex(), 0);

        GameObject.Find("OriginPoint").GetComponent<fixedWorldGen>().breakChunkBlockRecursive(transform.parent.transform.position, transform.GetSiblingIndex());
        //GameObject.Find("OriginPoint").GetComponent<fixedWorldGen>().breakChunkBlockByPosition2(transform.parent.transform.position, transform.GetSiblingIndex());


        //GetComponent<Renderer>().material.color = Color.black;
        gameObject.SetActive(false);
        activeBlock = false;
    }
    
}
