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
    private Vector3 properTriggerSize = new Vector3(1, 1, 1);
    private Vector3 halfTriggerSize = new Vector3(.5f, .5f, .5f);

    //public GameObject currentlyHittingChunk;

    public List<string> colliderNames;
    public List<GameObject> collidersNotClearing;

    public Material noBlockMat;

    private Dictionary<int, Material> materialStorage;

    //private void OnTriggerStay(Collider other)


    /*private void OnTriggerStay(Collider other)
    {
        if (other.name != "FirstPersonCharacter" && !other.name.Contains("chunk") && other.name != "DeleteTool")
            Debug.Log(other.name);
    }*/

    //private void OnTriggerEnter(Collider other)
    private void OnCollisionEnter(Collision collision)
    {
        //if (other.name != "FirstPersonCharacter" && !other.name.Contains("chunk") && other.name != "DeleteTool")
        //if(collision.gameObject.layer == 16)// && numberOfCollisions < 1)
        if (collision.gameObject.layer == 10 || collision.gameObject.layer == 16)
        //if (collision.gameObject.name != "FirstPersonCharacter" && !collision.gameObject.name.Contains("chunk") && collision.gameObject.name != "DeleteTool")
        {
            //currentlyHittingBlock = collision.gameObject;
            //Debug.Log(other.name);
            //Debug.Log(collision.gameObject.name);
            //colliderNames.Add(collision.gameObject.name);
            collidersNotClearing.Add(collision.gameObject);
            numberOfCollisions++;
            //currentlyHittingChunk = null;
            //calcNewPositionDistance();
        }
    }

    //private void OnTriggerExit(Collider other)
    private void OnCollisionExit(Collision collision)
    {
        //if (other.name != "FirstPersonCharacter" && !other.name.Contains("chunk") && other.name != "DeleteTool")
        //if (collision.gameObject.layer == 16 && numberOfCollisions > 0)
        if((collision.gameObject.layer == 10 || collision.gameObject.layer == 16) && numberOfCollisions > 0)
        //if (collision.gameObject.name != "FirstPersonCharacter" && !collision.gameObject.name.Contains("chunk") && collision.gameObject.name != "DeleteTool")
        {
            //colliderNames.Remove(collision.gameObject.name);
            collidersNotClearing.Remove(collision.gameObject);
            numberOfCollisions--;
            //currentlyHittingBlock = null;
        }
    }

    private IEnumerator clearEmptyCollisions()
    {
        yield return new WaitForSeconds(1);
        int collisionCount = collidersNotClearing.Count;

        for(int collisionIndex = 0; collisionIndex < collisionCount; collisionIndex++)
        {
            //if (collidersNotClearing[collisionIndex] != null)
            {
                //if(GameObject.Find(collidersNotClearing[collisionIndex].name) != null)
                /*if(collidersNotClearing[collisionIndex] == null)
                {
                    collidersNotClearing.Remove(collidersNotClearing[collisionIndex]);
                    numberOfCollisions--;
                }*/
                //Debug.Log(collidersNotClearing.Count + "__" + collisionIndex);
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
                    /*if ((collidersNotClearing[collisionIndex] == null || !collidersNotClearing[collisionIndex].activeSelf) && collisionIndex < collidersNotClearing.Count - 1)
                    {
                        collidersNotClearing.Remove(collidersNotClearing[collisionIndex]);
                        numberOfCollisions--;
                    }*/
                }
            }
        }
    }

    private void Start()
    {
        blockTrackerRef = GameObject.Find("blockTracker");
        childOpenPosLocations = new Dictionary<GameObject, Vector3> { };
        materialStorage = new Dictionary<int, Material> { };
        //StartCoroutine(delayedUpdate());
    }

    private GameObject blockTrackerRef;

    public int placingBlockMaterial = 1;
    /// <summary>
    /// Check what values are generated when pressing Q/E after placing and then mining the last block
    /// </summary>

    public bool blockFound = false;
    private int lastGoodBlockIndex = 0;

    Vector3 newPlacedBlockLocation;

    public void checkIfBlocksGained(int blockMatNum)
    {
        if(placingBlockMaterial == blockMatNum)
        {
            lastGoodBlockIndex = placingBlockMaterial;
            blockFound = true;
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
        transform.position = GameObject.Find("blockTracker").transform.position;
        transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), Mathf.RoundToInt(transform.position.z));
    }

    private List<Vector3> openPos = new List<Vector3> { };
    private float currentCalcDistance, shortCalcDistance = 99;
    private Vector3 newShortPos;
    private Vector3 lockedInPos;
    private bool lockedInPlace = false;

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
            //Debug.Log(childTransform.gameObject.name + childTransform.gameObject.GetComponent<placedBlockColliders>().currentlyColliding);
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
            //Debug.Log(childLocations.x + "__" + childLocations.y + "__" + childLocations.z);
            currentCalcDistance = Vector3.Distance(childLocations, blockTrackerRef.transform.position);
            if (currentCalcDistance < shortCalcDistance)
            {
                shortCalcDistance = currentCalcDistance;
                newShortPos = childLocations;// + transform.position;
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
                //GetComponent<Renderer>().material = (Material)Resources.Load("BlockArt/4-All/" + placingBlockMaterial);
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
