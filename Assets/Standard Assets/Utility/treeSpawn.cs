using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class treeSpawn : MonoBehaviour
{
    //public GameObject treeBlock;
    private int worldDimensions, squaredWorldDimensions, cubedWorldDimensions;
    void Start()
    {
        ///Finish this so that trees won't spawn their leaves inside of other blocks
        worldDimensions = transform.parent.GetComponentInParent<chunkDistanceCalc>().worldDimensions;
        squaredWorldDimensions = worldDimensions * worldDimensions;
        cubedWorldDimensions = worldDimensions * worldDimensions * worldDimensions;

        foreach(Transform childrenBlocks in transform)
        {
            /*int childToCheck = (int)((transform.parent.position.x + transform.position.x) 
                + (Mathf.Abs(transform.parent.position.z + transform.position.z) * worldDimensions)
                + Mathf.Abs(transform.parent.position.y + transform.position.y) * squaredWorldDimensions);*/

            if (childrenBlocks.position.y < 2)
            {
                int childToCheckX = (int)Mathf.Abs(Mathf.Abs(transform.position.x) - Mathf.Abs(transform.parent.localPosition.x));
                int childToCheckY = (int)Mathf.Abs(Mathf.Abs(transform.position.y) - Mathf.Abs(transform.parent.localPosition.y));
                int childToCheckZ = (int)Mathf.Abs(Mathf.Abs(transform.position.z) - Mathf.Abs(transform.parent.localPosition.z));

                /*int childToCheck = (int)((Mathf.Abs(childrenBlocks.position.x) % worldDimensions)
                    + (Mathf.Abs(childrenBlocks.position.y - 1) * squaredWorldDimensions)
                    + ((Mathf.Abs(childrenBlocks.position.z) % worldDimensions) * worldDimensions));*/
                int childToCheck = (int)(childToCheckX
                    + (childToCheckY * squaredWorldDimensions)
                    + (childToCheckZ * worldDimensions));
                //UnityEngine.Debug.Log(childToCheck + "\t\t" + childrenBlocks.position + "\t\t" + childToCheckX + "," + childToCheckY + "," + childToCheckZ + "\t\t" + transform.parent.localPosition);
                if (childToCheck < cubedWorldDimensions && childToCheck >= 0 && transform.parent.GetComponentInParent<chunkDistanceCalc>().checkChild(childToCheck) == 1)
                {
                    childrenBlocks.gameObject.SetActive(false);
                }
            }
        }

        Destroy(this);
    }
}
