using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class placedBlockSnapping : MonoBehaviour
{
    /// <summary>
    /// Figure out how to place blocks into existing chunks
    /// </summary>
    /// 

    private int numberOfCollisions = 0;

    public List<string> colliderNames;
    public List<GameObject> collidersNotClearing;

    public Material noBlockMat;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 10 || collision.gameObject.layer == 16)
        {
            collidersNotClearing.Add(collision.gameObject);
            numberOfCollisions++;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if((collision.gameObject.layer == 10 || collision.gameObject.layer == 16) && numberOfCollisions > 0)
        {
            collidersNotClearing.Remove(collision.gameObject);
            numberOfCollisions--;
        }
    }

    private IEnumerator clearEmptyCollisions()
    {
        yield return new WaitForSeconds(1);
        int collisionCount = collidersNotClearing.Count;

        for (int collisionIndex = 0; collisionIndex < collisionCount; collisionIndex++)
        {

            if (collisionIndex < collidersNotClearing.Count)
            {
                if ((collidersNotClearing[collisionIndex] == null))
                {
                    collidersNotClearing.Remove(collidersNotClearing[collisionIndex]);
                    numberOfCollisions--;
                }
                else if (collisionIndex < collidersNotClearing.Count && !collidersNotClearing[collisionIndex].activeSelf)
                {
                    collidersNotClearing.Remove(collidersNotClearing[collisionIndex]);
                    numberOfCollisions--;
                }
            }

        }
    }

    private void Start()
    {
        blockTrackerRef = GameObject.Find("blockTracker");
        childOpenPosLocations = new Dictionary<GameObject, Vector3> { };
    }

    private GameObject blockTrackerRef;

    public int placingBlockMaterial = 1;
    /// <summary>
    /// Check what values are generated when pressing Q/E after placing and then mining the last block
    /// </summary>



    public void checkIfBlocksGained(int blockMatNum)
    {
        if(placingBlockMaterial == blockMatNum)
        {
            GetComponent<Renderer>().material = (Material)Resources.Load("BlockArt/4-All/" + placingBlockMaterial);
        }
    }

    private void Update()
    {
        pickNewOpenPos3();
        if (numberOfCollisions > 1)
        {
            StartCoroutine(clearEmptyCollisions());
        }
        transform.position = blockTrackerRef.transform.position;
        transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), Mathf.RoundToInt(transform.position.z));
    }

    private List<Vector3> openPos = new List<Vector3> { };
    private float currentCalcDistance, shortCalcDistance = 99;
    private Vector3 newShortPos;

    private Dictionary<GameObject, Vector3> childOpenPosLocations;
    private bool anyOpenSpots = false;


    private Vector3 errorVector = new Vector3(-.1f, -.1f, -.1f);
    public Vector3 pickNewOpenPos2()
    {
        if(numberOfCollisions == 0)
        {
            return transform.position;
        }
        foreach (Transform childTransform in transform)
        {
            if (!childTransform.gameObject.GetComponent<placedBlockColliders>().currentlyColliding)
            {
                openPos.Add(childTransform.position);
            }
        }
        if(openPos.Count == 0)
        {
            return errorVector;
        }
        foreach (Vector3 childLocations in openPos)
        {
            currentCalcDistance = Vector3.Distance(childLocations, blockTrackerRef.transform.position);
            if (currentCalcDistance < shortCalcDistance)
            {
                shortCalcDistance = currentCalcDistance;
                newShortPos = childLocations;
            }
        }
        openPos.Clear();
        shortCalcDistance = 99;

        return newShortPos;
    }



    private Vector3 resetVector = new Vector3(100f, 100f, 100f);
    private GameObject lockedInChildObject;
    private Vector3 pickNewOpenPos3()
    {
        foreach (Transform childTransform in transform)
        {
            if (childTransform.gameObject != lockedInChildObject)
            {
                childTransform.gameObject.GetComponent<Renderer>().enabled = false;
            }
        }
        if (numberOfCollisions == 0)
        {
            if (lockedInChildObject)
            {
                GetComponent<Renderer>().enabled = true;
                lockedInChildObject.gameObject.GetComponent<Renderer>().enabled = false;
                lockedInChildObject = null;
            }
            return transform.position;
        }
        foreach (Transform childTransform in transform)
        {
            childOpenPosLocations[childTransform.gameObject] = resetVector;
            if (!childTransform.gameObject.GetComponent<placedBlockColliders>().currentlyColliding)
            {
                anyOpenSpots = true;
                childOpenPosLocations[childTransform.gameObject] = childTransform.position;
            }
        }

        if(!anyOpenSpots)
        {
            return errorVector;
        }

        foreach (KeyValuePair<GameObject, Vector3> childPair in childOpenPosLocations)
        {
            currentCalcDistance = Vector3.Distance(childPair.Value, blockTrackerRef.transform.position);
            if (currentCalcDistance < shortCalcDistance)
            {
                shortCalcDistance = currentCalcDistance;
                newShortPos = childPair.Value;
                if(lockedInChildObject)
                {
                    lockedInChildObject.GetComponent<Renderer>().enabled = false;
                    lockedInChildObject.GetComponent<Renderer>().material = GetComponent<Renderer>().material;

                    GetComponent<Renderer>().enabled = false;
                }
                lockedInChildObject = childPair.Key;
                lockedInChildObject.GetComponent<Renderer>().enabled = true;
            }
        }
        anyOpenSpots = false;
        shortCalcDistance = 99;

        return newShortPos;
    }
}
